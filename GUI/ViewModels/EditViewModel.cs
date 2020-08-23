using Database.DatabaseModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GUI.ViewModels
{
    class EditViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// All key bunches in the list.
        /// </summary>
        public ObservableCollection<KeyBunch> AllKeyBunches { get; set; }

        private KeyBunch _selectedKeyBunch;

        /// <summary>
        /// The currently selected keybunch.
        /// </summary>
        public KeyBunch SelectedKeyBunch
        {
            get { return _selectedKeyBunch; }
            set
            {
                _selectedKeyBunch = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedKeyBunch"));
            }
        }

        public RelayCommand<EditWindow> CmdSaveAndClose { get; set; }

        /// <summary>
        /// View model for the Edit Keypress window.
        /// </summary>
        public EditViewModel()
        {
            AllKeyBunches = new ObservableCollection<KeyBunch>(KeyBunch.All);
            CmdSaveAndClose = new RelayCommand<EditWindow>(
                (window) =>
                {
                    SelectedKeyBunch.Write();
                    window.DialogResult = true;
                }
            );
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
