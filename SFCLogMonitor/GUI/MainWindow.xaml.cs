using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using SFCLogMonitor.ViewModel;

namespace SFCLogMonitor.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly ViewModel.ViewModel _vm;
        private readonly string _path;

        public MainWindow()
        {
            InitializeComponent();
            _vm = (ViewModel.ViewModel)Resources["Vm"];
            _path = Directory.GetCurrentDirectory();
            LoadConfiguration();
            //            _vm.StringList.CollectionChanged += (sender, args) => StringListView.ScrollIntoView(_vm.StringList.LastOrDefault());
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
            _vm.ExcludeList = new ObservableCollection<string>(Properties.Resources.Exclude.Split(',')) {"SFCLogMonitor.exe"};
            _vm.SearchList = new ObservableCollection<string>(Properties.Resources.SearchList.Split(','));
            foreach (var f in Directory.GetFiles(_path).Select(Path.GetFileName))
            {
                if (!_vm.ExcludeList.Contains(f) )
                    _vm.FileList.Add(new LogFile
                    {
                        FileName = Path.GetFileName(f),
                        LastRow  = new ReverseLineReader(f).FirstOrDefault()
                    });
            }
        }

        private void CheckAndAddRow(string line, LogFile logFile)
        {
            if (_vm.SearchList.Any(s => CultureInfo.CurrentCulture.CompareInfo.IndexOf(line, s, CompareOptions.IgnoreCase) >= 0))
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

        // Define the event handlers. 
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
    }
}
