using GUI.ViewModels;
using System.Windows;
using Database;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DashboardViewModel vm;
        public MainWindow()
        {
            InitializeComponent();
            // TODO: not hardcode this
            vm = new DashboardViewModel(@"E:\Documents\code\KeypressManager\GUI\main.sqlite3");
            DataContext = vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BookingViewModel newVm;
            try
            {
                // if the keys are all booked out, book them in; if they are all booked in, book them out.
                newVm = new BookingViewModel(vm.SelectedKeyBunches, vm.SelectedKeyBunchesAllBookedOut ? BookingViewModel.BookingMode.In : BookingViewModel.BookingMode.Out);
                BookingWindow bookingWindow = new BookingWindow
                {
                    Owner = this,
                    DataContext = newVm
                };
                bookingWindow.ShowDialog();
                MessageBox.Show($"{bookingWindow.DialogResult}", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (PersonNotAuthorizedException)
            {
                MessageBox.Show($"Nobody is authorized to book {(vm.SelectedKeyBunchesAllBookedOut ? "out" : "in")} all selected keys together. Please try again with a different selection.",
                                "Unauthorized",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
        }
    }
}
