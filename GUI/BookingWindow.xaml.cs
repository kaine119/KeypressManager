using GUI.ViewModels;
using System;
using System.Timers;
using System.Windows;

namespace GUI
{
    /// <summary>
    /// Interaction logic for BookingWindow.xaml
    /// </summary>
    public partial class BookingWindow : Window
    {
        public readonly Timer Timer = new Timer();

        public BookingWindow()
        {
            InitializeComponent();
            // Use a timer to update the time field, and keep it up to date with current time.
            Timer.Elapsed += (_, e) =>
            {
                Dispatcher.Invoke(() => (DataContext as BookingViewModel).TimeBooked = new DateTimeOffset(e.SignalTime));
            };
            Timer.AutoReset = true;
            Timer.Enabled = true;
        }

        public void Submit(bool isBookinSuccessful)
        {
            Timer.Stop();
            Timer.Dispose();
            DialogResult = isBookinSuccessful;
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Submit(false);
        }
    }
}
