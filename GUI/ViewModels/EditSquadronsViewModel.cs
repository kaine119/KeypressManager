using Database.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Linq;

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
        public ObservableCollection<Squadron> AllSquadrons =>
            new ObservableCollection<Squadron>(Squadron.All);

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

        public RelayCommand<object> CmdSave { get; set; }

        public EditSquadronsViewModel()
        {
            PersonToAdd = new Person();
            SelectedSquadron = AllSquadrons.FirstOrDefault();

            CmdSave = new RelayCommand<object>(
                execute: (_) => { foreach (var sqn in AllSquadrons) { sqn.Write(); } },
                canExecute: () => AllSquadrons.All(sqn => sqn.IsValid)
            );
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
