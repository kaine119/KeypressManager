using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Media;

namespace GUI.Converters
{
    class HexTripletToBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string triplet && triplet.Length == 6 && Regex.IsMatch(triplet, "[0-9a-f]{6}"))
            {
                return new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#" + triplet)
                );
            }
            else
            {
                throw new InvalidOperationException($"{value} is not a valid hex triplet");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                return brush.Color.ToString();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
