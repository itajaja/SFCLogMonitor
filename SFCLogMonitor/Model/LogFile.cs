using SFCLogMonitor.Utils;
using SFCLogMonitor.ViewModel;

namespace SFCLogMonitor.Model
{
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
}