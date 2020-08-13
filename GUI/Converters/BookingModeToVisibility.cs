using GUI.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GUI.Converters
{
    class BookingModeToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is BookingViewModel.BookingMode mode))
                throw new ArgumentException("Value is not a Booking mode");
            if (!(parameter is string targetMode))
                throw new ArgumentException("Parameter is not a string");
            if (targetMode == "In")
                return mode == BookingViewModel.BookingMode.In ? Visibility.Visible : Visibility.Collapsed;
            else
                return mode == BookingViewModel.BookingMode.Out ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
