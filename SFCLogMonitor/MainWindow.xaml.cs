using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace SFCLogMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            _vm = (ViewModel)Resources["Vm"];
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
            _vm.ExcludeList = new ObservableCollection<string>(Properties.Resources.Exclude.Split(','));
            _vm.ExcludeList.Add("SFCLogMonitor.exe");
            _vm.SearchList = new ObservableCollection<string>(Properties.Resources.SearchList.Split(','));
            foreach (var f in Directory.GetFiles(_path).Select(Path.GetFileName))
            {
                if (!_vm.ExcludeList.Contains(f) )
                    _vm.FileList.Add(new File
                    {
                        FileName = Path.GetFileName(f),
                        LastRow  = new ReverseLineReader(f).FirstOrDefault()
                    });
            }
        }

        private void CheckAndAddRow(string line, File file)
        {
            if (_vm.SearchList.Any(s => CultureInfo.CurrentCulture.CompareInfo.IndexOf(line, s, CompareOptions.IgnoreCase) >= 0))
            {
                var r = new Row
                {
                    File = file,
                    Text = line,
                    Date = DateTime.Now
                };
                Application.Current.Dispatcher.Invoke((Action) (() => _vm.StringList.Insert(0,r)));
                while (_vm.StringList.Count > 5000) //todo parametrize!
                {
                    _vm.StringList.RemoveAt(4999);
                }
            }
        }

        private void CheckFile(string fileName, int counter = 0)
        {
            try
            {
                var reverseReader = new ReverseLineReader(fileName);
                string lastRow = reverseReader.FirstOrDefault();
                File file = _vm.FileList.SingleOrDefault(f => f.FileName == fileName);
                if (file == null)
                    return;
                foreach (var row in reverseReader)
                {
                    if (row == file.LastRow)
                    {
                        break;
                    }
                    CheckAndAddRow(row,file);
                }
                file.LastRow = lastRow;
            }
            catch (IOException ioException)
            {
                if (counter < 10)
                {
                    Thread.Sleep(1000);
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
            MessageBox.Show("oncreated");
            string f = e.Name;
            if (!_vm.ExcludeList.Contains(f))
                _vm.FileList.Add(new File
                {
                    FileName = f,
                    LastRow = new ReverseLineReader(f).FirstOrDefault()
                });
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

        }
        
        private readonly ViewModel _vm;
        private readonly string _path;

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _vm.StringList.Clear();
        }
    }
}
