using System.Collections.ObjectModel;
using SFCLogMonitor.Model;
using SFCLogMonitor.Utils;

namespace SFCLogMonitor.ViewModel
{
    public class ExcludeWindowViewModel : NotifyPropertyChanged
    {
        private ObservableCollection<LogFile> _fileList;

        public ObservableCollection<LogFile> FileList
        {
            get { return _fileList; }
            set { SetField(ref _fileList, value, "FileList"); }
        }
    }
}
