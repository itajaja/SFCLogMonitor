using System;
using SFCLogMonitor.Utils;

namespace SFCLogMonitor.Model
{
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