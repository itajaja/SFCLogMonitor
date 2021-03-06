﻿using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace SFCLogMonitor.Utils
{
    public sealed class TrulyObservableCollection<T> : ObservableCollection<T>
        where T : INotifyPropertyChanged
    {
        public TrulyObservableCollection()
        {
            CollectionChanged += TrulyObservableCollection_CollectionChanged;
        }

        private void TrulyObservableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (T item in e.NewItems)
                {
                    item.PropertyChanged += item_PropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (T item in e.OldItems)
                {
                    item.PropertyChanged -= item_PropertyChanged;
                }
            }
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var a = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(a);
        }
    }
}
