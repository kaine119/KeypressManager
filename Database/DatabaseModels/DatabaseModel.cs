using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
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

        /// <summary>
        /// Establish a new SQL connection object that all models use.
        /// </summary>
        /// <param name="connectionString">A IDbConnection connection string. See documentation for <seealso cref="IDbConnection"/>.</param>
        public static void EstablishConnection(string connectionString)
        {
            DbConnection = new SqliteConnection(connectionString);
        }

        public int? ID { get; protected set; }
        public abstract bool IsValid { get; }
        public abstract void Write();
    }
}
