using GUI.ViewModels;
using System.Windows;

namespace GUI
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private EditKeyBunchesViewModel editKeyVM = new EditKeyBunchesViewModel();
        private EditStaffViewModel editStaffVM = new EditStaffViewModel();

        public bool CanSave =>
            editKeyVM.CmdSave.CanExecute();

        public EditWindow()
        {
            InitializeComponent();
            EditKeyBunchesControl.DataContext = editKeyVM;
            EditStaffControl.DataContext = editStaffVM;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (CanSave)
            {
                editKeyVM.CmdSave.Execute(null);
                editStaffVM.CmdSave.Execute(null);
                DialogResult = true;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
