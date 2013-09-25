using System.Collections.ObjectModel;
using System.Windows;
using SFCLogMonitor.Model;
using SFCLogMonitor.ViewModel;

namespace SFCLogMonitor.View
{
    /// <summary>
    /// Interaction logic for ExcludeWindow.xaml
    /// </summary>
    public partial class ExcludeWindow
    {

        #region fields

        private readonly ExcludeWindowViewModel _vm;

        #endregion

        public ExcludeWindow(ObservableCollection<LogFile> fileList)
        {
            InitializeComponent();
            _vm = (ExcludeWindowViewModel)Resources["Vm"];
            _vm.FileList = fileList;
        }

        #region properties

        public ExcludeWindowViewModel Vm
        {
            get { return _vm; }
        }

        #endregion

        #region events

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        #endregion

    }
}
