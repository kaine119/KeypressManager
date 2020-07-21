using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using System.Text;

namespace Database.DatabaseModels
{
    /// <summary>
    /// Represents a row in the database. Should only be used to write to the database if IsValid.
    /// </summary>
    public abstract class DatabaseModel
    {
        public static IDbConnection DbConnection { get; set; }
        public static void EstablishConnection(string connectionString)
        {
            DbConnection = new SQLiteConnection(connectionString);
        }

        public int ID { get; set; }
        public abstract bool IsValid { get; }
        public abstract void Write();
    }
}
