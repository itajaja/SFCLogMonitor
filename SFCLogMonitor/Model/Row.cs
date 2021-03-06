﻿using System;
using SFCLogMonitor.Utils;

namespace SFCLogMonitor.Model
{
    public class Row : NotifyPropertyChanged, IDeepCloneable<Row>
    {
        #region fields

        private DateTime _date;
        private LogFile _logFile;
        private string _text;

        #endregion

        #region properties

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

        #endregion

        #region methods

        public Row DeepClone()
        {
            var newLog = (Row)MemberwiseClone();
            newLog.LogFile = LogFile.DeepClone();
            return newLog;
        }

        #endregion
    }
}