using Database;
using Database.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace GUI.ViewModels
{
    public class BookingViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<KeyBunch> _pendingKeys;
        /// <summary>
        /// The keys pending to be booked in/out.
        /// </summary>
        public ObservableCollection<KeyBunch> PendingKeys
        {
            get { return _pendingKeys; }
            set
            {
                _pendingKeys = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PendingKeys"));
            }
        }

        /// <summary>
        /// Whether the window is for booking in or booking out.
        /// </summary>
        public BookingMode Mode { get; set; }

        /// <summary>
        /// Any personnel authorized to book in/out ALL of the pending keys.
        /// </summary>
        public ObservableCollection<Person> AuthorizedPersonnel =>
            new ObservableCollection<Person>(
                PendingKeys.Skip(1)
                    .Aggregate(
                        new HashSet<Person>(PendingKeys.First().AllAuthorizedPersonnel),
                        (hash, keybunch) => { hash.IntersectWith(keybunch.AllAuthorizedPersonnel); return hash; }
                    )
            );

        private string _searchTerm;

        public string SearchTerm
        {
            get { return _searchTerm; }
            set
            {
                _searchTerm = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SearchTerm"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DisplayedAuthorizedPersonnel"));
            }
        }

        public ObservableCollection<Person> DisplayedAuthorizedPersonnel =>
            new ObservableCollection<Person>(
                AuthorizedPersonnel.Where(person => person.Match(SearchTerm))
            );

        /// <summary>
        /// Staff members availbale to issue the key.
        /// </summary>
        public ObservableCollection<Person> Staff =>
            new ObservableCollection<Person>(Person.AllStaff);

        private Person _selectedPersonBooking;

        public Person SelectedPersonBooking
        {
            get { return _selectedPersonBooking; }
            set
            {
                _selectedPersonBooking = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedPersonBooking"));
            }
        }

        private Person _selectedStaff;

        public Person SelectedStaff
        {
            get { return _selectedStaff; }
            set
            {
                _selectedStaff = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedStaff"));
            }
        }

        private DateTimeOffset _timeBooked;

        public DateTimeOffset TimeBooked
        {
            get { return _timeBooked; }
            set
            {
                _timeBooked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TimeBooked"));
            }
        }

        /// <summary>
        /// The command run when the "Book In" button is pressed.
        /// </summary>
        public RelayCommand<BookingWindow> CmdBook { get; set; }

        /// <summary>
        /// Attempt to book the selected keys in/out, depending on the rest of the viewmodel's parameters.
        /// </summary>
        /// <returns>Whether or not the operation succeeded.</returns>
        private bool TryBook()
        {
            if (Mode == BookingMode.In)
            {
                IEnumerable<LogEntry> latestLogs = PendingKeys.Select(kb => LogEntry.LatestForKeyBunch(kb));
                foreach (LogEntry log in latestLogs)
                {
                    log.TimeReturned = TimeBooked;
                    log.PersonReturningKey = SelectedPersonBooking;
                    log.PersonReceivingKey = SelectedStaff;
                    log.Write();
                }
                return true;
            }
            else
            {
                IEnumerable<LogEntry> newLogs = PendingKeys.Select(kb => new LogEntry { KeyBunchDrawn = kb });
                foreach (LogEntry log in newLogs)
                {
                    log.TimeIssued = TimeBooked;
                    log.PersonDrawingKey = SelectedPersonBooking;
                    log.PersonIssuingKey = SelectedStaff;
                    log.Write();
                }
                return true;
            }
        }

        /// <summary>
        /// View model for booking window. Used for booking keys in or out.
        /// </summary>
        /// <param name="pendingKeys">A list of keys to be booked in or out.</param>
        /// <param name="mode">Whether the keys are to be booked in or out.</param>
        /// <exception cref="InvalidOperationException">Thrown when no staff is configured.</exception>
        public BookingViewModel(ObservableCollection<KeyBunch> pendingKeys, BookingMode mode, Person selectedStaff)
        {
            PendingKeys = pendingKeys;
            Mode = mode;
            SelectedStaff = selectedStaff ?? throw new InvalidOperationException("No staff configured");
            SearchTerm = "";

            CmdBook = new RelayCommand<BookingWindow>(
                (window) =>
                {
                    TryBook();
                    window.Submit(true);
                },
                () => SelectedPersonBooking != null
            );

            if (AuthorizedPersonnel.Count == 0)
            {
                throw new PersonNotAuthorizedException("No personnel is authorized to draw all keys specified");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public enum BookingMode
        {
            In, Out
        }
    }
}
