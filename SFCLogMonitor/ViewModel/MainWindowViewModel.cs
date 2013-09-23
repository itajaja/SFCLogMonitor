using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using SFCLogMonitor.Model;
using SFCLogMonitor.Utils;

namespace SFCLogMonitor.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        private ObservableCollection<Row> _stringList;
        private readonly CollectionViewSource _stringListViewSource;
        private ObservableCollection<LogFile> _fileList;
        private ObservableCollection<string> _excludeList;
        private ObservableCollection<string> _searchList;
        private TimeUnit _filteringTimeUnit;
        private int _filteringTime;
        private bool _isFilteringTimeEnabled;
        private DateTime _beginMonitoringTime;
        private int _rowLimit;

        public MainWindowViewModel()
        {
            _stringListViewSource = new CollectionViewSource();
            StringListViewSource.Filter += (sender, args) =>
            {
                args.Accepted = true;
                var row = (Row) args.Item;
                //filter by time
                if (IsFilteringTimeEnabled)
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
                    if ((DateTime.Now - row.Date) > span)
                    {
                        args.Accepted = false;
                        return;
                    }
                }
                //filter by file
                if (ExcludeList.Contains(row.LogFile.FileName))
                {
                    args.Accepted = false;
                }
            };
            StringList = new ObservableCollection<Row>();
            FileList = new ObservableCollection<LogFile>();
            ExcludeList = new ObservableCollection<string>();
            SearchList = new ObservableCollection<string>();
            BeginMonitoringTime = DateTime.Now;
        }

        public ObservableCollection<Row> StringList
        {
            get { return _stringList; }
            set
            {
                SetField(ref _stringList, value, "StringList");
                StringListViewSource.Source = value;
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
            set { SetField(ref _excludeList, value, "ExcludeList"); }
        }

        public ObservableCollection<string> SearchList
        {
            get { return _searchList; }
            set { SetField(ref _searchList, value, "SearchList"); }
        }

        public TimeUnit FilteringTimeUnit
        {
            get { return _filteringTimeUnit; }
            set { SetField(ref _filteringTimeUnit, value, "FilteringTimeUnit"); }
        }

        public int FilteringTime
        {
            get { return _filteringTime; }
            set { SetField(ref _filteringTime, value, "FilteringTime"); }
        }

        public bool IsFilteringTimeEnabled
        {
            get { return _isFilteringTimeEnabled; }
            set { SetField(ref _isFilteringTimeEnabled, value, "IsFilteringTimeEnabled"); }
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
    }
}