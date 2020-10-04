using System.ComponentModel;

namespace GUI.ViewModels
{
    /// <summary>
    /// Parent view model for the Edit Keypress window. Manages several
    /// Edit component view models, and calls each component's Save command.
    /// </summary>
    class EditViewModel : INotifyPropertyChanged
    {
        private EditStaffViewModel _editStaffVM;

        public EditStaffViewModel EditStaffVM
        {
            get { return _editStaffVM; }
            set
            {
                _editStaffVM = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EditStaffVM"));
            }
        }

        private EditKeyBunchesViewModel _editKeysVM;

        public EditKeyBunchesViewModel EditKeyBunchesVM
        {
            get { return _editKeysVM; }
            set
            {
                _editKeysVM = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EditKeysVM"));
            }
        }

        private EditSquadronsViewModel _editSquadronsVM;

        public EditSquadronsViewModel EditSquadronsVM
        {
            get { return _editSquadronsVM; }
            set
            {
                _editSquadronsVM = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EditSquadronsVM"));
            }
        }

        private EditKeyListsViewModel _editKeyListsVM;

        public EditKeyListsViewModel EditKeyListsVM
        {
            get { return _editKeyListsVM; }
            set
            {
                _editKeyListsVM = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EditKeyListsVM"));
            }
        }


        public RelayCommand<EditWindow> CmdSave { get; set; }

        public EditViewModel()
        {
            EditStaffVM = new EditStaffViewModel();
            EditKeyBunchesVM = new EditKeyBunchesViewModel();
            EditSquadronsVM = new EditSquadronsViewModel();
            EditKeyListsVM = new EditKeyListsViewModel();

            CmdSave = new RelayCommand<EditWindow>(
                execute: (window) =>
                {
                    EditKeyBunchesVM.CmdSave.Execute(null);
                    EditStaffVM.CmdSave.Execute(null);
                    EditSquadronsVM.CmdSave.Execute(null);
                    EditKeyListsVM.CmdSave.Execute(null);
                    if (window != null)
                    {
                        window.DialogResult = true;
                    }
                },
                canExecute: () => EditKeyBunchesVM.CmdSave.CanExecute()
                                  && EditStaffVM.CmdSave.CanExecute()
                                  && EditSquadronsVM.CmdSave.CanExecute()
                                  && EditKeyListsVM.CmdSave.CanExecute()
            );

            EditKeyBunchesVM.CmdSave.CanExecuteChanged += (_, __) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CmdSave"));
            EditStaffVM.CmdSave.CanExecuteChanged      += (_, __) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CmdSave"));
            EditSquadronsVM.CmdSave.CanExecuteChanged  += (_, __) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CmdSave"));
            EditKeyListsVM.CmdSave.CanExecuteChanged += (_, __) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CmdSave"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
