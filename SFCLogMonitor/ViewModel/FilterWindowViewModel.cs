﻿using System.Collections.ObjectModel;
using SFCLogMonitor.Utils;

namespace SFCLogMonitor.ViewModel
{
    public class FilterWindowViewModel : NotifyPropertyChanged
    {
        #region fields

        private bool _isKeyFilteringEnabled;
        private ObservableCollection<string> _searchList;

        #endregion

        public FilterWindowViewModel(ObservableCollection<string> searchList)
        {
            SearchList = searchList;
        }

        #region properties

        public ObservableCollection<string> SearchList
        {
            get { return _searchList; }
            set { SetField(ref _searchList, value, "SearchList"); }
        }

        public bool IsKeyFilteringEnabled
        {
            get { return _isKeyFilteringEnabled; }
            set { SetField(ref _isKeyFilteringEnabled, value, "IsKeyFilteringEnabled"); }
        }

        #endregion
    }
}