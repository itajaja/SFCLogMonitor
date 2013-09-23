using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using SFCLogMonitor.Model;
using SFCLogMonitor.Properties;
using SFCLogMonitor.Utils;
using SFCLogMonitor.ViewModel;

namespace SFCLogMonitor.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MainWindowViewModel _vm;
        private readonly string _path;

        public MainWindow()
        {
            InitializeComponent();
            _vm = (MainWindowViewModel)Resources["Vm"];
            _path = Directory.GetCurrentDirectory();
            LoadConfiguration();
            InitializeWatcher();
        }

        private void InitializeWatcher()
        {
            var watcher = new FileSystemWatcher(_path)
            {
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                               | NotifyFilters.FileName | NotifyFilters.DirectoryName
            };
            // Add event handlers.
            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        private void LoadConfiguration()
        {
            var settings = Settings.Default;
            if(settings.Exclude != null)
                _vm.ExcludeList = new ObservableCollection<string>(settings.Exclude.Cast<string>().ToList()) { "SFCLogMonitor.exe" };
            if (settings.Filter != null)
                _vm.SearchList = new ObservableCollection<string>(settings.Filter.Cast<string>().ToList());
            foreach (var f in Directory.GetFiles(_path).Select(Path.GetFileName))
            {
                if (!_vm.ExcludeList.Contains(f) )
                    _vm.FileList.Add(new LogFile
                    {
                        FileName = Path.GetFileName(f),
                        LastRow  = new ReverseLineReader(f).FirstOrDefault()
                    });
            }
            _vm.FilteringTime = settings.TimeSpan;
            _vm.FilteringTimeUnit = (TimeUnit)settings.TimeUnit;
            _vm.RowLimit = settings.RowLimit;
            _vm.IsFilteringTimeEnabled = settings.IsTimeFiltering;
        }

        private void CheckAndAddRow(string line, LogFile logFile)
        {
            var r = new Row
            {
                LogFile = logFile,
                Text = line,
                Date = DateTime.Now
            };
            Application.Current.Dispatcher.Invoke((Action) (() => _vm.StringList.Insert(0,r)));
            while (_vm.StringList.Count > _vm.RowLimit)
            {
                Application.Current.Dispatcher.Invoke((Action) (() => _vm.StringList.RemoveAt(_vm.RowLimit)));
            }
        }

        private void CheckFile(string fileName, int counter = 0)
        {
            try
            {
                var reverseReader = new ReverseLineReader(fileName);
                string lastRow = reverseReader.FirstOrDefault();
                LogFile logFile = _vm.FileList.SingleOrDefault(f => f.FileName == fileName);
                if (logFile == null)
                    return;
                foreach (var row in reverseReader)
                {
                    if (row == logFile.LastRow)
                    {
                        break;
                    }
                    CheckAndAddRow(row,logFile);
                }
                logFile.LastRow = lastRow;
            }
            catch (IOException ioException)
            {
                if (counter < 10)
                {
                    Thread.Sleep(500);
                    CheckFile(fileName, counter + 1);
                }
                else
                {
                    MessageBox.Show(ioException.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveSettings()
        {
            var settings = Settings.Default;
            settings.TimeSpan = _vm.FilteringTime;
            settings.Exclude = new StringCollection();
            settings.Exclude.AddRange(_vm.ExcludeList.ToArray());
            settings.Filter = new StringCollection();
            settings.Filter.AddRange(_vm.SearchList.ToArray());
            settings.IsTimeFiltering = _vm.IsFilteringTimeEnabled;
            settings.RowLimit = _vm.RowLimit;
            settings.TimeUnit = (int)_vm.FilteringTimeUnit;
            settings.Save();
        }

        #region events
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            CheckFile(e.Name);
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            string f = e.Name;
            if (!_vm.ExcludeList.Contains(f))
                _vm.FileList.Add(new LogFile
                {
                    FileName = f,
                    LastRow = new ReverseLineReader(f).FirstOrDefault()
                });
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to clear the current session? cannot be undone.", "Clear", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, MessageBoxResult.Cancel) == MessageBoxResult.OK)
            {
                _vm.StringList.Clear();
                _vm.BeginMonitoringTime = DateTime.Now;
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            SaveSettings();
        }
        #endregion

    }
}
