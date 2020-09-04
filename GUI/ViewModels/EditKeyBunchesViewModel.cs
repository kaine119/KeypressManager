using Database.DatabaseModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

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

        private Person _personToAdd;

        /// <summary>
        /// The personnel details currently input in the "Add Personnel" field.
        /// </summary>
        public Person PersonToAdd
        {
            get { return _personToAdd; }
            set
            {
                _personToAdd = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PersonToAdd"));
            }
        }

        /// <summary>
        /// Adds a new key bunch based on a template.
        /// </summary>
        public RelayCommand<TextBox> CmdAddKeyBunch { get; set; }

        /// <summary>
        /// The number of new keybunches added, used for deduping
        /// </summary>
        private int addedKeyBunchesCount = 0;

        /// <summary>
        /// Adds the person in the "Add Personnel" field to the current keybunch's Authorized Personnel.
        /// </summary>
        public RelayCommand<TextBox> CmdAddPerson { get; set; }


        /// <summary>
        /// Save all keybunches.
        /// </summary>
        public RelayCommand<object> CmdSave { get; set; }

        /// <summary>
        /// View model for the Edit Keypress window.
        /// </summary>
        public EditKeyBunchesViewModel()
        {
            AllKeyBunches = new ObservableCollection<KeyBunch>(KeyBunch.All);
            AllKeyLists = new ObservableCollection<KeyList>(KeyList.All);

            PersonToAdd = new Person();

            CmdSave = new RelayCommand<object>(
                execute: (_) => { foreach (KeyBunch kb in AllKeyBunches) { kb.Write(); } },
                canExecute: () => AllKeyBunches.All(kb => kb.IsValid)
            );

            CmdAddPerson = new RelayCommand<TextBox>(
                execute: (focusTarget) => 
                {
                    SelectedKeyBunch.AuthorizedPersonnel.Add(
                        new Person
                        {
                            Name = PersonToAdd.Name,
                            NRIC = PersonToAdd.NRIC,
                            Rank = PersonToAdd.Rank,
                            ContactNumber = PersonToAdd.ContactNumber
                        }    
                    );
                    PersonToAdd = new Person();
                    focusTarget?.Focus();
                },
                canExecute: () => PersonToAdd.IsValid
            );

            CmdAddKeyBunch = new RelayCommand<TextBox>(
                execute: (focusTarget) =>
                {
                    addedKeyBunchesCount += 1;
                    KeyBunch newKeyBunch = new KeyBunch
                    {
                        Name = "New key bunch" + (addedKeyBunchesCount > 1 ? $" ({addedKeyBunchesCount})" : ""),
                        BunchNumber = $"{addedKeyBunchesCount:D2}"
                    };
                    AllKeyBunches.Insert(0, newKeyBunch);
                    
                    SelectedKeyBunch = newKeyBunch;
                    if (focusTarget != null)
                    {
                        focusTarget.Focus();
                        focusTarget.SelectAll();
                    }
                }
            );
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
