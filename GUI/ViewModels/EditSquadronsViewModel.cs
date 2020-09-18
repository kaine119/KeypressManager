using Database.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Linq;
using System.Windows.Controls;

namespace GUI.ViewModels
{
    /// <summary>
    /// The view model in charge of editing squadrons.
    /// </summary>
    class EditSquadronsViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// All squadrons in the database.
        /// </summary>
        public ObservableCollection<Squadron> AllSquadrons { get; set; }

        private Squadron _selectedSquadron;
        /// <summary>
        /// The currently selected squadron.
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

        private Person _personToAdd;
        /// <summary>
        /// The personnel currently input in the "Add personnel to squadron" box.
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

        private int newSquadronCount = 0;

        /// <summary>
        /// Adds a new squadron to the list, optionally focusing the provided text field.
        /// </summary>
        public RelayCommand<TextBox> CmdAddSquadron { get; set; }

        /// <summary>
        /// Adds a new person to the currently selected squadron, optionally focusing the provided text field.
        /// </summary>
        public RelayCommand<TextBox> CmdAddPerson { get; set; }

        /// <summary>
        /// Removes the provided person from the currently selected squadron.
        /// </summary>
        public RelayCommand<Person> CmdRemovePerson { get; set; }

        /// <summary>
        /// Saves all squadrons.
        /// </summary>
        public RelayCommand<object> CmdSave { get; set; }

        public EditSquadronsViewModel()
        {
            AllSquadrons = new ObservableCollection<Squadron>(Squadron.All);
            PersonToAdd = new Person();

            CmdAddSquadron = new RelayCommand<TextBox>(
                execute: (focusTarget) =>
                {
                    newSquadronCount++;
                    Squadron squadronToAdd = new Squadron
                    {
                        Name = $"New Squadron ({newSquadronCount})"
                    };
                    AllSquadrons.Add(squadronToAdd);
                    SelectedSquadron = squadronToAdd;

                    focusTarget?.Focus();
                    focusTarget?.SelectAll();
                }
            );

            CmdAddPerson = new RelayCommand<TextBox>(
                execute: (focusTarget) =>
                {
                    SelectedSquadron.Personnel.Add(new Person
                    {
                        NRIC = PersonToAdd.NRIC,
                        Rank = PersonToAdd.Rank,
                        Name = PersonToAdd.Name,
                        ContactNumber = PersonToAdd.ContactNumber
                    });
                    PersonToAdd = new Person();
                    focusTarget?.Focus();
                },
                canExecute: () => PersonToAdd.IsValid
            );

            CmdRemovePerson = new RelayCommand<Person>(
                execute: (person) => SelectedSquadron.Personnel.Remove(person)
            );

            CmdSave = new RelayCommand<object>(
                execute: (_) => { foreach (var sqn in AllSquadrons) { sqn.Write(); } },
                canExecute: () => AllSquadrons.All(sqn => sqn.IsValid)
            );
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
