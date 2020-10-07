using Database.DatabaseModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AvailableSquadrons"));
            }
        }

        private ObservableCollection<KeyBunch> _keyBunchesToDelete;
        /// <summary>
        /// The key bunches to be deleted upon saving.
        /// </summary>
        public ObservableCollection<KeyBunch> KeyBunchesToDelete
        {
            get { return _keyBunchesToDelete; }
            set
            {
                _keyBunchesToDelete = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("KeyBunchesToDelete"));
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
        /// The squadrons that can be authorized to draw the selected bunch (i.e. not already authorized)
        /// </summary>
        public IEnumerable<Squadron> AvailableSquadrons => Squadron.All.Except(SelectedKeyBunch?.AuthorizedSquadrons ?? Enumerable.Empty<Squadron>());

        private Squadron _selectedSquadron;
        /// <summary>
        /// The currently selected squadron in the "Add Squadron" group.
        /// </summary>
        public Squadron SelectedSquadron
        {
            get { return _selectedSquadron; }
            set
            {
                _selectedSquadron = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedSquadron"));
            }
        }

        /// <summary>
        /// Adds a new key bunch based on a template.
        /// </summary>
        public RelayCommand<TextBox> CmdAddKeyBunch { get; set; }

        /// <summary>
        /// Removes a given key bunch from the current list, and adds it to a list to be deleted.
        /// </summary>
        public RelayCommand<KeyBunch> CmdRemoveKeyBunch { get; set; }

        /// <summary>
        /// Adds the person in the "Add Personnel" field to the current keybunch's Authorized Personnel.
        /// </summary>
        public RelayCommand<TextBox> CmdAddPerson { get; set; }

        /// <summary>
        /// Removes a person from the currently selected keybunch's Authorized Personnel.
        /// </summary>
        public RelayCommand<Person> CmdRemovePerson { get; set; }

        /// <summary>
        /// Adds the selected squadron to the currently keybunch's Authorized Squadrons.
        /// </summary>
        public RelayCommand<object> CmdAddSquadron { get; set; }

        /// <summary>
        /// Removes a squadron from the currently selected keybunch's Authorized Squadrons.
        /// </summary>
        public RelayCommand<Squadron> CmdRemoveSquadron { get; set; }

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

            KeyBunchesToDelete = new ObservableCollection<KeyBunch>();

            PersonToAdd = new Person();

            CmdSave = new RelayCommand<object>(
                execute: (_) =>
                {
                    foreach (KeyBunch kb in AllKeyBunches) kb.Write();
                    KeyBunch.DeleteMultiple(KeyBunchesToDelete);
                },
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

            CmdRemovePerson = new RelayCommand<Person>(
                execute: (personToRemove) =>
                {
                    SelectedKeyBunch.AuthorizedPersonnel.Remove(personToRemove);
                }
            );

            CmdAddSquadron = new RelayCommand<object>(
                execute: (_) =>
                {
                    SelectedKeyBunch.AuthorizedSquadrons.Add(SelectedSquadron);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AvailableSquadrons"));
                },
                canExecute: () => SelectedSquadron != null
            );

            CmdRemoveSquadron = new RelayCommand<Squadron>(
                execute: (squadronToRemove) =>
                {
                    SelectedKeyBunch.AuthorizedSquadrons.Remove(squadronToRemove);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AvailableSquadrons"));
                }
            );

            CmdAddKeyBunch = new RelayCommand<TextBox>(
                execute: (focusTarget) =>
                {
                    int keyBunchNumber = AllKeyBunches.Select(bunch =>
                    {
                        bool result = int.TryParse(bunch.BunchNumber, out int ret);
                        return result ? ret : 0;
                    }).DefaultIfEmpty(0).Max() + 1;
                    KeyBunch newKeyBunch = new KeyBunch
                    {
                        Name = $"New key bunch {keyBunchNumber}",
                        BunchNumber = $"{keyBunchNumber:D2}",
                        KeyList = AllKeyLists.First()
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

            CmdRemoveKeyBunch = new RelayCommand<KeyBunch>(
                execute: (keyBunch) =>
                {
                    KeyBunchesToDelete.Add(keyBunch);
                    AllKeyBunches.Remove(keyBunch);
                }
            );
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
