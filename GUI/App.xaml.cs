﻿using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Database;
using Microsoft.Data.Sqlite;

namespace GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Runs on application startup. Checks if there is a database file specified by 
        /// local settings, verifies that it exists and that we can write to it,
        /// then runs the application. 
        /// If any of those steps fails, ask the user whether they want to locate an existing
        /// database, or begin a new one.
        /// </summary>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string path;
            if (GUI.Properties.Settings.Default.DatabaseLocation != ""
                && File.Exists(GUI.Properties.Settings.Default.DatabaseLocation)
                && HasWritePermissionOnFile(GUI.Properties.Settings.Default.DatabaseLocation))
            {
                path = GUI.Properties.Settings.Default.DatabaseLocation;
            }
            else
            {
                const string MessageBoxText = "The database file hasn't been initialized, was moved, or is inaccessible due to permissions.\n"
                                              + "Would you like to create a new database file?\n"
                                              + "Select Yes to create a new file, No to open an existing file, or Cancel to close.";

                MessageBoxResult newOrExisting = MessageBox.Show(
                    MessageBoxText,
                    "Open existing database or Create new database",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Exclamation,
                    MessageBoxResult.Yes
                );
                switch (newOrExisting)
                {
                    case MessageBoxResult.Yes:
                        if (!TryWriteNewDatabase(out path))
                        {
                            Shutdown();
                            return;
                        }
                        break;
                    case MessageBoxResult.No:
                        if (!TryOpenExistingDatabase(out path))
                        {
                            Shutdown();
                            return;
                        }
                        break;
                    case MessageBoxResult.Cancel:
                    default:
                        Shutdown(); // the application! not the whole PC
                        return;
                }
            }
            KeypressDatabase.Initialize(path);
            MainWindow window = new MainWindow();
            window.Show();
        }

        /// <summary>
        /// Prompts the user to locate an existing database, then checks if we can 
        /// write to it.
        /// </summary>
        /// <param name="path">Will be set to the location of the selected file.</param>
        /// <return>Whether the operation was successful.</return>
        private bool TryOpenExistingDatabase(out string path)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".sqlite3",
                Filter = "Keypress Databases (.sqlite3)|*.sqlite3"
            };
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                if (!HasWritePermissionOnFile(dialog.FileName))
                {
                    MessageBox.Show("Can't open the file specified, please check the file.",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    path = null;
                    return false;
                }

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

        /// <summary>
        /// Prompts the user to select a location for a new database, then tries to copy
        /// the template database to that location.
        /// </summary>
        /// <param name="path">Will be set to the location of the selected file.</param>
        /// <return>Whether the operation was successful.</return>
        private bool TryWriteNewDatabase(out string path)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "KeypressDatabase",
                DefaultExt = ".sqlite3",
                Filter = "Keypress Databases|*.sqlite3"
            };
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                try
                {
                    string currentAssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string sourcePath = Path.Join(currentAssemblyDirectory, @"Assets\Template.sqlite3");
                    File.Copy(sourcePath, dialog.FileName, overwrite: true);
                    GUI.Properties.Settings.Default.DatabaseLocation = dialog.FileName;
                    GUI.Properties.Settings.Default.Save();
                    path = dialog.FileName;
                    return true;
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Couldn't create the new Database file, please check permissions for the target folder.",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    path = null;
                    return false;
                }
            }
            else
            {
                MessageBox.Show("No database selected, closing application");
                Shutdown();
                path = null;
                return false;
            }
        }

        /// <summary>
        /// Returns whether the running application can write to the specified database file.
        /// </summary>
        /// <param name="path">The full path to the file.</param>
        public static bool HasWritePermissionOnFile(string path)
        {
            // Establish a temporary connection to the specified database, and try to write a dummy value.
            try
            {
                using (SqliteConnection conn = new SqliteConnection($"Data Source={path}"))
                {
                    conn.Open();

                    // If the target database file is old and doesn't have a WriteTest table, we'll create it.
                    // Newer template databases have a dummy WriteTest table, so we can upsert to that.
                    // Between the two of them, a write is guaranteed.

                    // Create a dummy WriteTest table, if it doesn't exist.
                    SqliteCommand createTable = conn.CreateCommand();
                    createTable.CommandText = "CREATE TABLE IF NOT EXISTS WriteTest(id INTEGER PRIMARY KEY, value TEXT);";

                    // Upsert the dummy value in the database.
                    SqliteCommand writeTest = conn.CreateCommand();
                    writeTest.CommandText = $"INSERT INTO WriteTest(id) VALUES (1) ON CONFLICT(id) DO UPDATE SET value='{Guid.NewGuid()}'";
                    createTable.ExecuteNonQuery();
                    writeTest.ExecuteNonQuery();
                }
                return true;
            }
            catch (SqliteException)
            {
                return false;
            }
        }
    }
}
