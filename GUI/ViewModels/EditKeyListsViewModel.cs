using Database.DatabaseModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;

namespace GUI.ViewModels
{
    class EditKeyListsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<KeyList> _allKeyLists;

        public ObservableCollection<KeyList> AllKeyLists
        {
            get { return _allKeyLists; }
            set
            {
                _allKeyLists = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllKeyLists"));
            }
        }

        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedIndex"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedKeyList"));
            }
        }

        public KeyList SelectedKeyList => AllKeyLists[SelectedIndex];

        private int newKeylistCount = 0;

        public RelayCommand<TextBox> CmdAddKeyList { get; set; }

        public RelayCommand<object> CmdSave { get; set; }

        public EditKeyListsViewModel()
        {
            AllKeyLists = new ObservableCollection<KeyList>(KeyList.All);

            CmdAddKeyList = new RelayCommand<TextBox>(
                execute: (focusTarget) =>
                {
                    newKeylistCount++;
                    KeyList newList = new KeyList
                    {
                        Name = $"New Keylist {newKeylistCount}"
                    };
                    AllKeyLists.Add(newList);
                }
            );

            CmdSave = new RelayCommand<object>(
                execute: (_) => { foreach (var list in AllKeyLists) { list.Write(); } },
                canExecute: () => AllKeyLists.All(list => list.IsValid)
            );
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
