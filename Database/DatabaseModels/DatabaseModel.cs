using System.Data;
using Microsoft.Data.Sqlite;

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
            DbConnection.Open();
        }

        public int? ID { get; protected set; }
        public abstract bool IsValid { get; }

        public void Write()
        {
            IDbTransaction transaction = DbConnection.BeginTransaction();
            try
            {
                Write(transaction);
                transaction.Commit();
            }
            finally
            {
                transaction.Dispose();
            }
        }
        public abstract void Write(IDbTransaction transaction);
    }
}
