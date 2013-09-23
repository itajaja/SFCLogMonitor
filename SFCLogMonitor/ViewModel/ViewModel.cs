using System;
using System.Collections.ObjectModel;
using SFCLogMonitor.Utils;

namespace SFCLogMonitor.ViewModel
{
    public class ViewModel : NotifyPropertyChanged
    {
        private ObservableCollection<Row> _stringList;
        private ObservableCollection<LogFile> _fileList;
        private ObservableCollection<string> _excludeList;
        private ObservableCollection<string> _searchList;
        private TimeUnit _filteringTimeUnit;
        private int _filteringTime;
        private bool _isFilteringTimeEnabled;
        private DateTime _beginMonitoringTime;
        private int _rowLimit;

        public ViewModel()
        {
            StringList = new ObservableCollection<Row>();
            FileList = new ObservableCollection<LogFile>();
            ExcludeList = new ObservableCollection<string>();
            SearchList = new ObservableCollection<string>();
            _beginMonitoringTime = DateTime.Now;
        }

        public ObservableCollection<Row> StringList
        {
            get { return _stringList; }
            set { SetField(ref _stringList, value, "StringList"); }
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

    public class LogFile
    {
        public string FileName { get; set; }
        public string LastRow { get; set; }
    }
    
    public class Row
    {
        public LogFile LogFile { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}