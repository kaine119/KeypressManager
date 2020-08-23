using Database.DatabaseModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GUI.ViewModels
{
    class EditKeyBunchesViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// All key bunches in all key lists.
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

        /// <summary>
        /// All key lists in the database.
        /// </summary>
        public ObservableCollection<KeyList> AllKeyLists { get; set; }



        public RelayCommand<EditWindow> CmdSaveAndClose { get; set; }

        /// <summary>
        /// View model for the Edit Keypress window.
        /// </summary>
        public EditKeyBunchesViewModel()
        {
            AllKeyBunches = new ObservableCollection<KeyBunch>(KeyBunch.All);
            AllKeyLists = new ObservableCollection<KeyList>(KeyList.All);
            
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
