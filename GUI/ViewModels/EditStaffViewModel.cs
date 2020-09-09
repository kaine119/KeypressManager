using Database.DatabaseModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace GUI.ViewModels
{
    class EditStaffViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// All the staff in the database.
        /// </summary>
        public ObservableCollection<Person> AllStaff { get; set; }

        private Person _selectedStaff;

        /// <summary>
        /// The currently selected staff member.
        /// </summary>
        public Person SelectedStaff
        {
            get { return _selectedStaff; }
            set
            {
                _selectedStaff = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedStaff"));
            }
        }

        private Person _staffToAdd;
        /// <summary>
        /// The staff currently input in the "Add Staff" field.
        /// </summary>
        public Person StaffToAdd
        {
            get { return _staffToAdd; }
            set
            {
                _staffToAdd = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StaffToAdd"));
            }
        }


        /// <summary>
        /// Adds a new staff member based on a template.
        /// </summary>
        public RelayCommand<TextBox> CmdAddStaff { get; set; }

        /// <summary>
        /// Removes the currently selected staff member.
        /// </summary>
        public RelayCommand<TextBox> CmdRemoveSelectedStaff { get; set; }

        /// <summary>
        /// View model for the Edit Staff window.
        /// </summary>
        public EditStaffViewModel()
        {
            AllStaff = new ObservableCollection<Person>(Person.AllStaff);
            SelectedStaff = AllStaff.FirstOrDefault();
            StaffToAdd = new Person();

            CmdAddStaff = new RelayCommand<TextBox>(
                execute: (focusTarget) =>
                {
                    AllStaff.Add(
                        new Person
                        {
                            NRIC = StaffToAdd.NRIC,
                            Rank = StaffToAdd.Rank,
                            Name = StaffToAdd.Name,
                            ContactNumber = StaffToAdd.ContactNumber
                        }
                    );
                    StaffToAdd = new Person();
                    focusTarget?.Focus();
                },
                canExecute: () => StaffToAdd.IsValid
            );

            CmdRemoveSelectedStaff = new RelayCommand<TextBox>(
                execute: (focusTarget) =>
                {
                    AllStaff.Remove(SelectedStaff);
                    SelectedStaff = null;
                    focusTarget?.Focus();
                },
                canExecute: () => SelectedStaff != null
            );
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
