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

        public bool CanSave =>
            editKeyVM.CmdSave.CanExecute();

        public EditWindow()
        {
            InitializeComponent();
            EditKeyBunchesControl.DataContext = editKeyVM;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (CanSave)
            {
                editKeyVM.CmdSave.Execute(null);
                DialogResult = true;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
