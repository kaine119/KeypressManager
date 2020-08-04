using Database;
using Database.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace GUI.ViewModels
{
    class DashboardViewModel
    {
        private KeypressDatabase db;
        public ObservableCollection<KeyBunch> PresentKeys { get; set; }
        public ObservableCollection<KeyBunch> BookedOutKeys { get; set; }

        public DashboardViewModel(string path)
        {
            db = new KeypressDatabase(path);
            PresentKeys = new ObservableCollection<KeyBunch>(KeyBunch.Returned);
            BookedOutKeys = new ObservableCollection<KeyBunch>(KeyBunch.Unreturned);
        }
    }
}
