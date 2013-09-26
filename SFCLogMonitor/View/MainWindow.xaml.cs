using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using SFCLogMonitor.Model;
using SFCLogMonitor.Properties;
using SFCLogMonitor.ViewModel;

namespace SFCLogMonitor.View
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region fields

        private readonly MainWindowViewModel _vm;

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            _vm = (MainWindowViewModel)Resources["Vm"];
            _vm.LoadConfiguration();
        }

        #region methods

        private void SaveSettings()
        {
            Settings settings = Settings.Default;
            settings.TimeSpan = _vm.FilteringTime;
            settings.Exclude = new StringCollection();
            settings.Exclude.AddRange(_vm.FileList.Where(f => f.IsExcluded).Select(f => f.FileName).ToArray());
            settings.Filter = new StringCollection();
            settings.Filter.AddRange(_vm.SearchList.ToArray());
            settings.IsTimeFiltering = _vm.IsFilteringTimeEnabled;
            settings.RowLimit = _vm.RowLimit;
            settings.TimeUnit = (int)_vm.FilteringTimeUnit;
            settings.IsKeyFiltering = _vm.IsKeyFilteringEnabled;
            settings.Save();
        }

        #endregion

        #region events

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to clear the current session? cannot be undone.", "Clear", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, MessageBoxResult.Cancel) == MessageBoxResult.OK)
            {
                _vm.StringList.Clear();
                _vm.BeginMonitoringTime = DateTime.Now;
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            SaveSettings();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var filterWindow = new FilterWindow(_vm.IsKeyFilteringEnabled, new ObservableCollection<string>(_vm.SearchList));
            if (filterWindow.ShowDialog() ?? false)
            {
                _vm.IsKeyFilteringEnabled = filterWindow.Vm.IsKeyFilteringEnabled;
                _vm.SearchList = filterWindow.Vm.SearchList;
            }
        }

        private void ExcludeButton_Click(object sender, RoutedEventArgs e)
        {
            var excludeWindow = new ExcludeWindow(new ObservableCollection<LogFile>(_vm.FileList.Select(f => f.DeepClone())));
            if (excludeWindow.ShowDialog() ?? false)
            {
                foreach (LogFile file in _vm.FileList)
                {
                    file.IsExcluded = excludeWindow.Vm.FileList.Single(o => o.FileName == file.FileName).IsExcluded;
                }
                _vm.StringListViewSource.View.Refresh();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _vm.IsPaused = !_vm.IsPaused;
        }
        #endregion
    }
}