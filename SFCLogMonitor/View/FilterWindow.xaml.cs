using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        protected void AddKey()
        {
            string s = AddKeyTextBox.Text;
            if (!String.IsNullOrEmpty(s) && _vm.SearchList.All(o => String.Compare(o, s, StringComparison.OrdinalIgnoreCase) != 0))
            {
                _vm.SearchList.Add(s);
            }
        }

        #endregion

        #region events

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (AddKeyTextBox.IsFocused) return;
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
            AddKey();
        }

        private void AddKeyTextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            AddKey();
            e.Handled = true;
        }
        #endregion


    }
}