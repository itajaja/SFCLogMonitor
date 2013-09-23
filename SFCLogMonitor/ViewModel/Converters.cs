using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SFCLogMonitor.ViewModel
{
    public class NullToFalseConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }

    public class NullToTrueConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }

    public class DateToShortDateStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = value as DateTime?;
            if (date != null)
            {
                return ((DateTime) date).ToShortDateString();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }

    public class DateToShortTimeStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = value as DateTime?;
            if (date != null)
            {
                return ((DateTime) date).ToShortTimeString();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }

    /// <summary>
    ///     returns true if value.ToString is contained in the parameter string
    /// </summary>
    public class StringToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<String> strings = ((string) parameter).Split(',').ToList();
            return strings.Cast<object>().Contains(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }

    /// <summary>
    ///     returns true if the (int)value equals the (int)parameter
    /// </summary>
    public class IntToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (int) value;
            int par;
            bool parseResult = int.TryParse(parameter.ToString(), out par);
            return (parseResult && val == par);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }

        #endregion
    }

    /// <summary>
    ///     returns false if the (int)value equals the (int)parameter
    /// </summary>
    public class IntToFalseConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (int) value;
            int par;
            bool parseResult = int.TryParse(parameter.ToString(), out par);
            return !(parseResult && val == par);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }

        #endregion
    }

    public class BooleanToOpacity : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(bool) value)
            {
                return parameter;
            }
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }

    /// <summary>
    ///     returns true if value.Equals(parameter) is true
    /// </summary>
    public class EqualityToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return value.Equals(parameter);
            }
            //if the value is null, check if the parameter is null as well, in this case return true
            return parameter == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }


    /// <summary>
    ///     returns !(value)
    /// </summary>
    public class InvertBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool) value;
        }

        #endregion
    }
}