using GUI.ViewModels;
using System.Windows;
using Database;
using System;
using System.Windows.Input;
using System.Reflection;

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
            vm = new DashboardViewModel();
            DataContext = vm;
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            VersionBox.Text = $"v{version.Major}.{version.Minor}.{version.Build}";
        }

        private void StartBookingWindow(object sender, RoutedEventArgs e)
        {
            BookingViewModel newVm;
            try
            {
                // if the keys are all booked out, book them in; if they are all booked in, book them out.
                newVm = new BookingViewModel(vm.SelectedKeyBunches,
                                             vm.SelectedKeyBunchesAllBookedOut ? BookingViewModel.BookingMode.In : BookingViewModel.BookingMode.Out,
                                             vm.SelectedStaff);
                BookingWindow bookingWindow = new BookingWindow
                {
                    Owner = this,
                    DataContext = newVm
                };
                bookingWindow.ShowDialog();
                if (bookingWindow.DialogResult == true)
                {
                    vm.RefreshViewModel();
                }
            }
            catch (PersonNotAuthorizedException)
            {
                MessageBox.Show($"Nobody is authorized to book {(vm.SelectedKeyBunchesAllBookedOut ? "in" : "out")} all selected keys together. Please try again with a different selection.",
                                "Unauthorized",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Staff not configured, please add staff members in Edit Keypress.",
                                "No staff configured",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handle Home (focuses the search bar) and End (opens the booking window).
        /// </summary>
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Home:
                    SearchBox?.Focus();
                    e.Handled = true;
                    break;
                case Key.End:
                    if (vm.SelectedKeyBunches.Count > 0)
                    {
                        StartBookingWindow(sender, e);
                    }
                    e.Handled = true;
                    break;
                case Key.Escape:
                    vm.SelectedKeyBunch = null;
                    vm.SelectedKeyBunches.Clear();
                    vm.SearchTerm = "";
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }

        private void StartEditWindow(object sender, RoutedEventArgs e)
        {
            EditWindow window = new EditWindow
            {
                Owner = this
            };
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                vm.RefreshViewModel();
            }
        }
    }
}
