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

        private string _searchTerm;

        public string SearchTerm
        {
            get { return _searchTerm; }
            set
            {
                _searchTerm = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SearchTerm"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DisplayedPresentKeys"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DisplayedBookedOutKeys"));
            }
        }

        public ObservableCollection<DashboardKeyListItem> DisplayedPresentKeys => 
            SearchTerm == ""
                    ? PresentKeys
                    : new ObservableCollection<DashboardKeyListItem>(PresentKeys.Where(item => item.KeyBunch.Match(SearchTerm)));

        public ObservableCollection<DashboardKeyListItem> DisplayedBookedOutKeys =>
            SearchTerm == ""
                    ? BookedOutKeys
                    : new ObservableCollection<DashboardKeyListItem>(BookedOutKeys.Where(item => item.KeyBunch.Match(SearchTerm)));


        public LogEntry LogEntryForSelectedBunch => LogEntry.LatestForKeyBunch(SelectedKeyBunch?.KeyBunch);

        public ObservableCollection<KeyBunch> SelectedKeyBunches =>
            new ObservableCollection<KeyBunch>(
                PresentKeys
                    .Union(BookedOutKeys)
                    .Where(item => item.IsSelected)
                    .Select(item => item.KeyBunch)
            );

        /// <summary>
        /// Whether all the keys selected are present.
        /// </summary>
        public bool SelectedKeyBunchesAllBookedIn =>
            SelectedKeyBunches.All(kb => PresentKeys.Select(item => item.KeyBunch).Contains(kb));

        /// <summary>
        /// Whether all the keys selected are booked out.
        /// </summary>
        public bool SelectedKeyBunchesAllBookedOut =>
            SelectedKeyBunches.All(kb => BookedOutKeys.Select(item => item.KeyBunch).Contains(kb));

        /// <summary>
        /// All staff available.
        /// </summary>
        public ObservableCollection<Person> AllStaff =>
            new ObservableCollection<Person>(Person.AllStaff);

        private Person _selectedStaff;
        /// <summary>
        /// The currently selected staff member. Passed to the booking window as the default selection.
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

        public DashboardViewModel(string path)
        {
            db = new KeypressDatabase(path);

            SearchTerm = "";

            PresentKeys = new ObservableCollection<DashboardKeyListItem>();
            foreach (KeyBunch key in KeyBunch.Returned)
            {
                DashboardKeyListItem item = new DashboardKeyListItem(key, false);
                item.PropertyChanged += OnKeyListItemPropertyChanged;
                PresentKeys.Add(item);
            }

            BookedOutKeys = new ObservableCollection<DashboardKeyListItem>();
            foreach (KeyBunch key in KeyBunch.Unreturned)
            {
                DashboardKeyListItem item = new DashboardKeyListItem(key, false);
                item.PropertyChanged += OnKeyListItemPropertyChanged;
                BookedOutKeys.Add(item);
            }

            SelectedStaff = AllStaff.FirstOrDefault();
        }

        public void RefreshViewModel()
        {
            SearchTerm = "";
            PresentKeys.Clear();
            foreach (KeyBunch key in KeyBunch.Returned)
            {
                DashboardKeyListItem item = new DashboardKeyListItem(key, false);
                item.PropertyChanged += OnKeyListItemPropertyChanged;
                PresentKeys.Add(item);
            }

            BookedOutKeys.Clear();
            foreach (KeyBunch key in KeyBunch.Unreturned)
            {
                DashboardKeyListItem item = new DashboardKeyListItem(key, false);
                item.PropertyChanged += OnKeyListItemPropertyChanged;
                BookedOutKeys.Add(item);
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllStaff"));
            if (SelectedStaff is null)
            {
                SelectedStaff = AllStaff.FirstOrDefault();
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
