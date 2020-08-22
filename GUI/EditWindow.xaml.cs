using GUI.ViewModels;
using System.Windows;

namespace GUI
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        public EditWindow()
        {
            DataContext = new EditViewModel();
            InitializeComponent();
        }
    }
}
