using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Write the object to the database.
        /// </summary>
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

        /// <summary>
        /// Deletes the current object from the database with a new transaction.
        /// Deletion is done with <see cref="Delete(IDbTransaction)"/>.
        /// </summary>
        public void Delete()
        {
            if (ID is int)
            {
                IDbTransaction transaction = DbConnection.BeginTransaction();
                try
                {
                    Delete(transaction);
                    transaction.Commit();
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            else
            {
                throw new System.InvalidOperationException($"Can't delete a non-written record of type {GetType()}");
            }
        }

        public abstract void Delete(IDbTransaction transaction);

        public static void DeleteMultiple(IEnumerable<DatabaseModel> objects)
        {
            IDbTransaction transaction = DbConnection.BeginTransaction();
            try
            {
                foreach (var obj in objects)
                {
                    obj.Delete(transaction);
                }
                transaction.Commit();
            }
            finally
            {
                transaction.Dispose();
            }
        }
    }
}
