﻿using Database;
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
                PendingKeys.Aggregate(
                    PendingKeys.First().AuthorizedPersonnel,
                    (list, person) => new List<Person>(list.Intersect(person.AuthorizedPersonnel))
                )
            );

        /// <summary>
        /// The latest log entries for each pending key.
        /// </summary>
        public ObservableCollection<LogEntry> LogEntriesForPendingKeys => new ObservableCollection<LogEntry>(
            PendingKeys.Select(kb => LogEntry.LatestForKeyBunch(kb))
        );

        /// <summary>
        /// View model for booking window. Used for booking keys in or out.
        /// </summary>
        /// <param name="pendingKeys">A list of keys to be booked in or out.</param>
        /// <param name="mode">Whether the keys are to be booked in or out.</param>
        public BookingViewModel(ObservableCollection<KeyBunch> pendingKeys, BookingMode mode)
        {
            PendingKeys = pendingKeys;
            Mode = mode;
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
