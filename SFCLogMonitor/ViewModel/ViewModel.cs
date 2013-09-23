using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using SFCLogMonitor.Utils;

namespace SFCLogMonitor.ViewModel
{
    public class ViewModel : NotifyPropertyChanged
    {
        private ObservableCollection<Row> _stringList;
        private CollectionViewSource _stringListViewSource;
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

        public CollectionViewSource StringListViewSource
        {
            get { return _stringListViewSource; }
            set { SetField(ref _stringListViewSource, value, "StringListViewSource"); }
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

    public class LogFile : NotifyPropertyChanged
    {
        private string _fileName;
        private string _lastRow;

        public string FileName
        {
            get { return _fileName; }
            set { SetField(ref _fileName, value, "FileName"); }
        }

        public string LastRow
        {
            get { return _lastRow; }
            set { SetField(ref _lastRow, value, "LastRow"); }
        }
    }

    public class Row : NotifyPropertyChanged
    {
        private LogFile _logFile;
        private string _text;
        private DateTime _date;

        public LogFile LogFile
        {
            get { return _logFile; }
            set { SetField(ref _logFile, value, "LogFile"); }
        }

        public string Text
        {
            get { return _text; }
            set { SetField(ref _text, value, "Text"); }
        }

        public DateTime Date
        {
            get { return _date; }
            set { SetField(ref _date, value, "Date"); }
        }
    }
}