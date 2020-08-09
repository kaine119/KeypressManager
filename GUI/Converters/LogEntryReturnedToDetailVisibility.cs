using Database.DatabaseModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GUI.Converters
{
    class LogEntryReturnedToDetailVisibility : IValueConverter
    {
        /// <summary>
        /// If a log entry represents a returned key, return Visible, otherwise return Collapsed.
        /// If a parameter of any non-null value is passed, behaviour is inverted; return Visible if the log entry is NOT returned.
        /// </summary>
        /// <param name="parameter">If passed, will invert converter behaviour.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is LogEntry) && value != null)
                throw new ArgumentException("Value was not a LogEntry");

            LogEntry log = (LogEntry)value;

            if (parameter == null)
            {
                return log == null || log.IsKeyReturned ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return log == null || log.IsKeyReturned ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
