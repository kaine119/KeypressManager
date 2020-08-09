using Database;
using Database.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GUI.ViewModels
{
    class DashboardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private KeypressDatabase db;
        public ObservableCollection<KeyBunch> PresentKeys { get; set; }
        public ObservableCollection<KeyBunch> BookedOutKeys { get; set; }

        private KeyBunch _selectedKeyBunch;

        public KeyBunch SelectedKeyBunch
        {
            get { return _selectedKeyBunch; }
            set 
            { 
                _selectedKeyBunch = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedKeyBunch"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LogEntryForSelectedBunch"));
            }
        }

        public LogEntry LogEntryForSelectedBunch => LogEntry.LatestForKeyBunch(SelectedKeyBunch);

        public DashboardViewModel(string path)
        {
            db = new KeypressDatabase(path);
            PresentKeys = new ObservableCollection<KeyBunch>(KeyBunch.Returned);
            BookedOutKeys = new ObservableCollection<KeyBunch>(KeyBunch.Unreturned);
        }
    }
}
