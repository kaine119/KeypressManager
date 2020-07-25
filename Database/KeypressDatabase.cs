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
            SqlMapper.RemoveTypeMap(typeof(DateTimeOffset));
            SqlMapper.RemoveTypeMap(typeof(DateTimeOffset?));
            SqlMapper.AddTypeHandler(typeof(DateTimeOffset?), new UnixTimestampDateTimeHandler());
            DatabaseModel.EstablishConnection($"Data source={databasePath}");
        }

        public IEnumerable<Person> AllPersonnel => Person.GetAll();
        public IEnumerable<KeyBunch> AllKeyBunches => KeyBunch.GetAll();
        public IEnumerable<KeyList> AllKeyLists => KeyList.GetAll();
        public IEnumerable<LogEntry> AllLogEntries => LogEntry.GetAll();

        /// <summary>
        /// Maps unix timestamps to DateTimeOffsets.
        /// </summary>
        public class UnixTimestampDateTimeHandler : SqlMapper.TypeHandler<DateTimeOffset?>
        {
            public UnixTimestampDateTimeHandler() { }

            public override DateTimeOffset? Parse(object value)
            {
                if (value is null)
                {
                    return null;
                }
                else if (value is long time)
                {
                    return DateTimeOffset.FromUnixTimeSeconds(time).ToLocalTime();
                }
                else
                {
                    throw new InvalidOperationException($"Could not parse value {value} of type {value.GetType()} as DateTime");
                }
            }

            public override void SetValue(IDbDataParameter parameter, DateTimeOffset? value)
            {
                parameter.Value = (int)(value?.ToUnixTimeSeconds() ?? null);
            }
        }
    }

    /// <summary>
    /// Thrown when personnel is not authorized to draw or return a key.
    /// </summary>
    public class PersonNotAuthorizedException : Exception { }
}
