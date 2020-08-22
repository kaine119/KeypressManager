using Database.DatabaseModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GUI.ViewModels
{
    class EditViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<KeyBunch> _allKeyBunches;

        public ObservableCollection<KeyBunch> AllKeyBunches
        {
            get { return _allKeyBunches; }
            set
            {
                _allKeyBunches = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllKeyBunches"));
            }
        }

        public EditViewModel()
        {
            AllKeyBunches = new ObservableCollection<KeyBunch>(KeyBunch.All);
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
