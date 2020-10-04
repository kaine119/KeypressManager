using GUI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Forms = System.Windows.Forms;

namespace GUI.EditControls
{
    /// <summary>
    /// Interaction logic for EditKeyListsControl.xaml
    /// </summary>
    public partial class EditKeyListsControl : UserControl
    {
        public EditKeyListsControl()
        {
            InitializeComponent();
        }

        private void PickColorHandler(object sender, RoutedEventArgs e)
        {
            Forms.ColorDialog dialog = new Forms.ColorDialog
            {
                FullOpen = true,
                AnyColor = true,
                Color = System.Drawing.Color.FromArgb(0x00, 0x23, 0x34, 0x40)
            };
            Forms.DialogResult dialogResult = dialog.ShowDialog();
            if (dialogResult == Forms.DialogResult.OK)
            {
                // Get the ARGB representation of the color (0xAARRGGBB),
                // mask off the two most significant digits (0x00ffffff), 
                // then convert it to a string, padding it to 6 digits long where appropriate.
                // Why? Didn't want to deal with WinForms <-> WPF integration properly. xd
                string resultTriplet = (dialog.Color.ToArgb() & 0x00ffffff).ToString("x6");
                (DataContext as EditKeyListsViewModel).SelectedColor = "#" + resultTriplet;
            }
        }
    }
}
