using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using SFCLogMonitor.ViewModel;

namespace SFCLogMonitor.View
{
    /// <summary>
    ///     Interaction logic for FilterWindow.xaml
    /// </summary>
    public partial class FilterWindow
    {
        #region fields

        private readonly FilterWindowViewModel _vm;

        #endregion

        public FilterWindow(bool isKeyFilteringEnabled, ObservableCollection<string> searchList)
        {
            InitializeComponent();
            _vm = (FilterWindowViewModel) Resources["Vm"];
            Vm.IsKeyFilteringEnabled = isKeyFilteringEnabled;
            Vm.SearchList = searchList;
        }

        #region properties

        public FilterWindowViewModel Vm
        {
            get { return _vm; }
        }

        #endregion

        #region methods

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

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            string s = AddKeyTextBox.Text;
            if (String.IsNullOrEmpty(s) || _vm.SearchList.Any(o => String.Compare(o, s, StringComparison.OrdinalIgnoreCase) == 0)) return;
            _vm.SearchList.Add(s);
        }
        
        #endregion

    }
}