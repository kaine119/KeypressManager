using System;
using System.IO;
using System.Windows;

namespace GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string path;
            if (GUI.Properties.Settings.Default.DatabaseLocation != "" && File.Exists(GUI.Properties.Settings.Default.DatabaseLocation))
            {
                path = GUI.Properties.Settings.Default.DatabaseLocation;
            }
            else
            {
                MessageBoxResult newOrExisting = MessageBox.Show("The database hasn't been initialized, or the database file was moved.\n" +
                                                                     "Would you like to create a new database file?\n" +
                                                                     "Select Yes to create a new file, No to open an existing file, or Cancel to close.",
                                                                 "Open existing database or Create new database",
                                                                 MessageBoxButton.YesNoCancel,
                                                                 MessageBoxImage.Exclamation,
                                                                 MessageBoxResult.Yes);
                switch (newOrExisting)
                {
                    case MessageBoxResult.Yes:
                        if (!TryWriteNewDatabase(out path)) Shutdown();
                        break;
                    case MessageBoxResult.No:
                        if (!TryOpenExistingDatabase(out path)) Shutdown();
                        break;
                    case MessageBoxResult.Cancel:
                    default:
                        Shutdown(); // the application! not the whole PC
                        return;
                }
            }
            MainWindow window = new MainWindow(path);
            window.Show();
        }

        private bool TryOpenExistingDatabase(out string path)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".sqlite3",
                Filter = "Keypress Databases|*.sqlite3"
            };
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                GUI.Properties.Settings.Default.DatabaseLocation = dialog.FileName;
                GUI.Properties.Settings.Default.Save();
                path = dialog.FileName;
                return true;
            }
            else
            {
                MessageBox.Show("No database selected, closing application");
                path = null;
                Shutdown();
                return false;
            }
        }

        private bool TryWriteNewDatabase(out string path)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "KeypressDatabase",
                DefaultExt = ".sqlite3",
                Filter = "Keypress Databases (.sqlite3)|*.sqlite3"
            };
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                GUI.Properties.Settings.Default.DatabaseLocation = dialog.FileName;
                GUI.Properties.Settings.Default.Save();
                System.IO.File.Copy(@"Assets\Template.sqlite3", dialog.FileName, overwrite: true);
                path = dialog.FileName;
                return true;
            }
            else
            {
                MessageBox.Show("No database selected, closing application");
                Shutdown();
                path = null;
                return false;
            }
        }
    }
}
