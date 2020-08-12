using Database;
using Database.DatabaseModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace GUI.ViewModels
{
    class DashboardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private KeypressDatabase db;
        public ObservableCollection<DashboardKeyListItem> PresentKeys { get; set; }
        public ObservableCollection<DashboardKeyListItem> BookedOutKeys { get; set; }

        private DashboardKeyListItem _selectedKeyBunch;

        public DashboardKeyListItem SelectedKeyBunch
        {
            get { return _selectedKeyBunch; }
            set
            {
                _selectedKeyBunch = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedKeyBunch"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LogEntryForSelectedBunch"));
            }
        }

        public LogEntry LogEntryForSelectedBunch => LogEntry.LatestForKeyBunch(SelectedKeyBunch?.KeyBunch);

        public ObservableCollection<KeyBunch> SelectedKeyBunches => 
            new ObservableCollection<KeyBunch>(
                PresentKeys
                    .Union(BookedOutKeys)
                    .Where(item => item.IsSelected)
                    .Select(item => item.KeyBunch)
            );

        public bool SelectedKeyBunchesAllBookedIn =>
            SelectedKeyBunches.All(kb => PresentKeys.Select(item => item.KeyBunch).Contains(kb));

        public bool SelectedKeyBunchesAllBookedOut =>
            SelectedKeyBunches.All(kb => BookedOutKeys.Select(item => item.KeyBunch).Contains(kb));

        public DashboardViewModel(string path)
        {
            db = new KeypressDatabase(path);
            PresentKeys = new ObservableCollection<DashboardKeyListItem>(
                KeyBunch.Returned.Select(kb => new DashboardKeyListItem(kb, false))
            );
            foreach (DashboardKeyListItem item in PresentKeys)
            {
                item.PropertyChanged += OnKeyListItemPropertyChanged;
            }

            BookedOutKeys = new ObservableCollection<DashboardKeyListItem>(
                KeyBunch.Unreturned.Select(kb => new DashboardKeyListItem(kb, false))
            );
            foreach (DashboardKeyListItem item in BookedOutKeys)
            {
                item.PropertyChanged += OnKeyListItemPropertyChanged;
            }
        }

        private void OnKeyListItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedKeyBunches"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedKeyBunchesAllBookedIn"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedKeyBunchesAllBookedOut"));
        }
    }

    public class DashboardKeyListItem : INotifyPropertyChanged
    {
        private KeyBunch _keyBunch;
        public KeyBunch KeyBunch
        {
            get { return _keyBunch; }
            set
            {
                _keyBunch = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("KeyBunch"));
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DashboardKeyListItem(KeyBunch keyBunch, bool isSelected)
        {
            KeyBunch = keyBunch;
            IsSelected = isSelected;
        }
    }
}
