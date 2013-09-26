using System;
using SFCLogMonitor.Utils;

namespace SFCLogMonitor.Model
{
    public class LogFile : NotifyPropertyChanged, IDeepCloneable<LogFile>
    {
        #region fields

        private string _fileName;
        private string _lastRow;
        private bool _isExcluded;

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

        public bool IsExcluded
        {
            get { return _isExcluded; }
            set { SetField(ref _isExcluded, value, "IsExcluded"); }
        }
        #endregion

        #region methods

        public LogFile DeepClone()
        {
            var newLog = (LogFile)MemberwiseClone();
            return newLog;
        }

        #endregion

    }
}