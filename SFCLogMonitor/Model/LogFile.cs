using SFCLogMonitor.Utils;

namespace SFCLogMonitor.Model
{
    public class LogFile : NotifyPropertyChanged
    {
        #region fields

        private string _fileName;
        private string _lastRow;

        #endregion

        #region properties

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

        #endregion
    }
}