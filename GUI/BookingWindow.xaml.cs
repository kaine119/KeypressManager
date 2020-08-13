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
        private readonly Timer timer = new Timer();

        public BookingWindow()
        {
            InitializeComponent();
            // Use a timer to update the time field, and keep it up to date with current time.
            timer.Elapsed += (_, e) =>
            {
                Dispatcher.Invoke(() => (DataContext as BookingViewModel).TimeIssued = new DateTimeOffset(e.SignalTime));
            };
            timer.AutoReset = true;
            timer.Enabled = true;
        }


        private void Submit(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            timer.Dispose();
            DialogResult = true;
        }
    }
}
