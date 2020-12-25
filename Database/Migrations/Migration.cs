using Dapper;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Database
{
    /// <summary>
    /// Add this attribute to any method under Database.Migrations to use it as a migration.
    /// A migration method must take in only one argument, the IDbConnection that
    /// SQL commands will be sent to, in order to effect the migration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MigrationAttribute : Attribute
    {
        /// <summary>
        /// The schema version that the migration upgrades to.
        /// </summary>
        public int schemaVersion { get; protected set; }

        /// <param name="schemaVersion">The schema version that the migration upgrades to.</param>
        public MigrationAttribute(int schemaVersion)
        {
            this.schemaVersion = schemaVersion;
        }
    }

    /// <summary>
    /// The helper class for all migrations.
    /// </summary>
    public class Migration
    {
        /// <summary>
        /// Apply all migrations in Database.Migrations to a database.
        /// </summary>
        /// <param name="db">The connection to the database.</param>
        public static void ApplyMigrations(IDbConnection db)
        {
            // Get all methods in this assembly within the Database.Migrations namespace.
            string ns = "Database.Migrations";

            // First get all the methods in the namespace
            var meths = Assembly.GetExecutingAssembly().GetTypes()
                        .Where(t => t.Namespace == ns)
                        .SelectMany(t => t.GetMethods());

            // Then, for each of the types in the namespace, get all the methods
            // with the attribute.
            var migrationMethods
                = from m in meths
                  where m.GetCustomAttributes<MigrationAttribute>().Any()
                  select m;

            Console.WriteLine(string.Join(',', migrationMethods.Select(m => m.Name)));

            // Check if the migration methods take in the correct parameters.
            if (!migrationMethods.All(
                    meth => meth.GetParameters()
                            .SingleOrDefault()?.ParameterType == typeof(IDbConnection)
               ))
            {
                throw new ArgumentException("Migrations must only take in an IDbConnection as a parameter.");
            }

            // get the current version number for the database;
            // if there's no version number, error out
            //db.Query<int>(@"SELECT DatabaseVersion FROM meta");

            // filter out only the migrations with versions greater than current,
            // and sort by version number
            

            // run the migrations in order



        }
    }
}
