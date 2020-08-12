using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GUI.Converters
{
    class MultipleToInvis : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IList))
                throw new NotSupportedException("value was not a list");
            if ((parameter as string) == "Invert")
                return (value as IList).Count == 1 ? Visibility.Collapsed : Visibility.Visible;
            else
                return (value as IList).Count == 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
