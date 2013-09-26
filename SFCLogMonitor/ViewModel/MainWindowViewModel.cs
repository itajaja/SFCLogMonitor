using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using SFCLogMonitor.Model;
using SFCLogMonitor.Properties;
using SFCLogMonitor.Utils;

namespace SFCLogMonitor.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        #region fields

        private readonly CollectionViewSource _stringListViewSource;
        private DateTime _beginMonitoringTime;
        private ObservableCollection<LogFile> _fileList;
        private int _filteringTime;
        private TimeUnit _filteringTimeUnit;
        private bool _isFilteringTimeEnabled;
        private bool _isKeyFilteringEnabled;
        private int _rowLimit;
        private ObservableCollection<string> _searchList;
        private ObservableCollection<Row> _stringList;
        private bool _isPaused;
        private readonly FileSystemWatcher _watcher;
        private readonly string _path;

        #endregion

        public MainWindowViewModel()
        {
            _path = Directory.GetCurrentDirectory();
            _watcher = new FileSystemWatcher(_path)
            {
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                               | NotifyFilters.FileName | NotifyFilters.DirectoryName
            };
            _stringListViewSource = new CollectionViewSource();
            StringListViewSource.Filter += OnStringListViewSourceFilter;
            StringList = new ObservableCollection<Row>();
            FileList = new ObservableCollection<LogFile>();
            SearchList = new ObservableCollection<string>();
            BeginMonitoringTime = DateTime.Now;
        }

        #region events

        /// <summary>
        ///     tracks when the view changes, in order to set the timer for the next refresh
        /// </summary>
        private void ViewOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            var collectionView = (ListCollectionView) sender;
            Row lastDisplayedRow = collectionView.Cast<Row>().LastOrDefault();
            if (lastDisplayedRow == null) return;
            DispatcherTimeout.Timeout(DispatcherPriority.Normal, FilteringTimeSpan - (DateTime.Now - lastDisplayedRow.Date),
                _ => { if (IsFilteringTimeEnabled) StringListViewSource.View.Refresh(); });
        }

        private void OnStringListViewSourceFilter(object sender, FilterEventArgs args)
        {
            args.Accepted = Filter((Row) args.Item);
        }

        private bool Filter(Row row)
        {
            //filter by time
            if (IsFilteringTimeEnabled)
                if ((DateTime.Now - row.Date) > FilteringTimeSpan) return false;
            //filter by keyword
            if (IsKeyFilteringEnabled && !SearchList.Any(s => CultureInfo.CurrentCulture.CompareInfo.IndexOf(row.Text, s, CompareOptions.IgnoreCase) >= 0)) return false;
            //filter by file
            if (row.LogFile.IsExcluded) return false;
            return true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            CheckFile(e.Name);
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            //todo implement
        }

        #endregion

        #region properties

        public ObservableCollection<Row> StringList
        {
            get { return _stringList; }
            set
            {
                SetField(ref _stringList, value, "StringList");
                StringListViewSource.Source = value;
                StringListViewSource.View.CollectionChanged += ViewOnCollectionChanged;
            }
        }

        public CollectionViewSource StringListViewSource
        {
            get { return _stringListViewSource; }
        }

        public ObservableCollection<LogFile> FileList
        {
            get { return _fileList; }
            set
            {
                SetField(ref _fileList, value, "FileList");
                StringListViewSource.View.Refresh();
            }
        }

        public ObservableCollection<string> SearchList
        {
            get { return _searchList; }
            set
            {
                SetField(ref _searchList, value, "SearchList");
                StringListViewSource.View.Refresh();
            }
        }

        public TimeUnit FilteringTimeUnit
        {
            get { return _filteringTimeUnit; }
            set
            {
                SetField(ref _filteringTimeUnit, value, "FilteringTimeUnit");
                StringListViewSource.View.Refresh();
            }
        }

        public int FilteringTime
        {
            get { return _filteringTime; }
            set
            {
                SetField(ref _filteringTime, value, "FilteringTime");
                StringListViewSource.View.Refresh();
            }
        }

        public bool IsFilteringTimeEnabled
        {
            get { return _isFilteringTimeEnabled; }
            set
            {
                SetField(ref _isFilteringTimeEnabled, value, "IsFilteringTimeEnabled");
                StringListViewSource.View.Refresh();
            }
        }

        public DateTime BeginMonitoringTime
        {
            get { return _beginMonitoringTime; }
            set { SetField(ref _beginMonitoringTime, value, "BeginMonitoringTime"); }
        }

        public int RowLimit
        {
            get { return _rowLimit; }
            set { SetField(ref _rowLimit, value, "RowLimit"); }
        }

        public bool IsKeyFilteringEnabled
        {
            get { return _isKeyFilteringEnabled; }
            set
            {
                SetField(ref _isKeyFilteringEnabled, value, "IsKeyFilteringEnabled");
                StringListViewSource.View.Refresh();
            }
        }

        public TimeSpan FilteringTimeSpan
        {
            get
            {
                TimeSpan span;
                switch (FilteringTimeUnit)
                {
                    case TimeUnit.Seconds:
                        span = TimeSpan.FromSeconds(FilteringTime);
                        break;
                    case TimeUnit.Minutes:
                        span = TimeSpan.FromMinutes(FilteringTime);
                        break;
                    case TimeUnit.Hours:
                        span = TimeSpan.FromHours(FilteringTime);
                        break;
                    case TimeUnit.Days:
                        span = TimeSpan.FromDays(FilteringTime);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return span;
            }
        }

        public bool IsPaused
        {
            get { return _isPaused; }
            set
            {
                SetField(ref _isPaused, value, "IsPaused");
                if (IsPaused)
                    StopWatcher();
                else
                    InitializeWatcher();
            }
        }

        #endregion

        #region methods

        public void LoadConfiguration()
        {
            Settings settings = Settings.Default;
            if (settings.Filter != null)
                SearchList = new ObservableCollection<string>(settings.Filter.Cast<string>().ToList());
            foreach (string f in Directory.GetFiles(_path).Select(Path.GetFileName))
            {
                FileList.Add(new LogFile
                {
                    FileName = Path.GetFileName(f),
                    LastRow = new ReverseLineReader(f).FirstOrDefault(),
                    IsExcluded = settings.Exclude != null && settings.Exclude.Contains(Path.GetFileName(f))
                });
            }
            FilteringTime = settings.TimeSpan;
            FilteringTimeUnit = (TimeUnit)settings.TimeUnit;
            RowLimit = settings.RowLimit;
            IsFilteringTimeEnabled = settings.IsTimeFiltering;
            IsKeyFilteringEnabled = settings.IsKeyFiltering;
            InitializeWatcher();
        }

        private void AddRow(string line, LogFile logFile)
        {
            var r = new Row
            {
                LogFile = logFile,
                Text = line,
                Date = DateTime.Now
            };
            Application.Current.Dispatcher.Invoke((Action)(() => StringList.Insert(0, r)));
            while (StringList.Count > RowLimit)
            {
                Application.Current.Dispatcher.Invoke((Action)(() => StringList.RemoveAt(RowLimit)));
            }
        }

        private void CheckFile(string fileName, int counter = 0)
        {
            try
            {
                var reverseReader = new ReverseLineReader(fileName);
                string lastRow = reverseReader.FirstOrDefault();
                LogFile logFile = FileList.SingleOrDefault(f => f.FileName == fileName);
                if (logFile == null)
                    return;
                var newRows = reverseReader.TakeWhile(row => row != logFile.LastRow).ToList();
                newRows.Reverse();
                foreach (string r in newRows)
                {
                    AddRow(r, logFile);
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

        private void InitializeWatcher()
        {
            _watcher.Changed += OnChanged;
            _watcher.Created += OnCreated;
            _watcher.EnableRaisingEvents = true;
            foreach (LogFile file in FileList)
            {
                CheckFile(file.FileName);
            }
        }

        private void StopWatcher()
        {
            _watcher.Changed -= OnChanged;
            _watcher.Created -= OnCreated;
            _watcher.EnableRaisingEvents = false;
        }

        #endregion
    }
}