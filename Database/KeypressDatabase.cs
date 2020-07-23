using Dapper;
using Database.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Database
{
    public class KeypressDatabase
    {
        public KeypressDatabase(string databasePath)
        {
            DatabaseModel.EstablishConnection($"Data source={databasePath}");
        }

        public IEnumerable<Person> AllPersonnel => Person.GetAll();
        public IEnumerable<KeyBunch> AllKeyBunches => KeyBunch.GetAll();
        public IEnumerable<KeyList> AllKeyLists => KeyList.GetAll();
    }
}
