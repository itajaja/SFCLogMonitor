using System;
using System.Collections.ObjectModel;

namespace SFCLogMonitor
{
    public class ViewModel
    {
        public ViewModel()
        {
            StringList = new ObservableCollection<Row>();
            FileList = new ObservableCollection<File>();
            ExcludeList = new ObservableCollection<string>();
            SearchList = new ObservableCollection<string>();
        }

        public ObservableCollection<Row> StringList { get; private set; }
        public ObservableCollection<File> FileList { get; set; }
        public ObservableCollection<string> ExcludeList { get; set; }
        public ObservableCollection<string> SearchList { get; set; }
    }

    public class File
    {
        public string FileName { get; set; }
        public string LastRow { get; set; }
    }
    
    public class Row
    {
        public File File { get; set; }
        public string Text { get; set; }
        public int RowNumber { get; set; }
        public DateTime Date { get; set; }
    }
}