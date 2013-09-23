using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;
using SFCLogMonitor.Model;
using SFCLogMonitor.Utils;

namespace SFCLogMonitor.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        #region fields

        private readonly CollectionViewSource _stringListViewSource;
        private DateTime _beginMonitoringTime;
        private ObservableCollection<string> _excludeList;
        private ObservableCollection<LogFile> _fileList;
        private int _filteringTime;
        private TimeUnit _filteringTimeUnit;
        private bool _isFilteringTimeEnabled;
        private bool _isKeyFilteringEnabled;
        private int _rowLimit;
        private ObservableCollection<string> _searchList;
        private ObservableCollection<Row> _stringList;

        #endregion

        public MainWindowViewModel()
        {
            _stringListViewSource = new CollectionViewSource();
            StringListViewSource.Filter += OnStringListViewSourceFilter;
            StringList = new ObservableCollection<Row>();
            FileList = new ObservableCollection<LogFile>();
            ExcludeList = new ObservableCollection<string>();
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
            args.Accepted = true;
            var row = (Row) args.Item;
            //filter by time
            if (IsFilteringTimeEnabled)
            {
                if ((DateTime.Now - row.Date) > FilteringTimeSpan)
                {
                    args.Accepted = false;
                    return;
                }
            }
            //filter by keyword
            if (!SearchList.Any(s => CultureInfo.CurrentCulture.CompareInfo.IndexOf(row.Text, s, CompareOptions.IgnoreCase) >= 0))
            {
                args.Accepted = false;
                return;
            }
            //filter by file
            if (ExcludeList.Contains(row.LogFile.FileName))
            {
                args.Accepted = false;
            }
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
            set { SetField(ref _fileList, value, "FileList"); }
        }

        public ObservableCollection<string> ExcludeList
        {
            get { return _excludeList; }
            set
            {
                SetField(ref _excludeList, value, "ExcludeList");
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
            set { SetField(ref _isKeyFilteringEnabled, value, "IsKeyFilteringEnabled"); }
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

        #endregion
    }
}