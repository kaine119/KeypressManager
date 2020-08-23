using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GUI.Converters
{
    class NullToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)parameter == "Invert")
                return value != null ? Visibility.Collapsed : Visibility.Visible;
            else
                return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
