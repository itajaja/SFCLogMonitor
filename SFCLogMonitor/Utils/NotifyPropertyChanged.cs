using System.Collections.Generic;
using System.ComponentModel;

namespace SFCLogMonitor.Utils
{
    /// <summary>
    /// Abstract base class that implements the INotifyPropertyChanged interface with some utility methods to write properties
    /// </summary>
    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Set the private field with the passed value, and fires OnPropertyChanged with the given property name
        /// </summary>
        /// <param name="field">the private field to set</param>
        /// <param name="value">the value for the field</param>
        /// <param name="propertyName">the name of the property for firing OnPropertyChanged</param>
        /// <returns></returns>
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}