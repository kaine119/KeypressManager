using Dapper;
using Database.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Data;

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

        public IEnumerable<Person> AllPersonnel => Person.All;
        public IEnumerable<KeyBunch> AllKeyBunches => KeyBunch.All;
        public IEnumerable<KeyList> AllKeyLists => KeyList.All;
        public IEnumerable<LogEntry> AllLogEntries => LogEntry.All;
        public IEnumerable<Squadron> AllSquadrons => Squadron.All;

        public IEnumerable<LogEntry> LogsEntriesForUnreturnedKeys => LogEntry.ForUnreturnedKeys;
        public IEnumerable<KeyBunch> UnreturnedKeys => KeyBunch.Unreturned;


        /// <summary>
        /// Book a key out.
        /// </summary>
        /// <param name="key">The key bunch to book out.</param>
        /// <param name="personDrawing">The person drawing the key.</param>
        /// <param name="personIssuing">The person issuing the key.</param>
        /// <exception cref="PersonNotAuthorizedException">Thrown when the person drawing the key is not authorized.</exception>
        public void BookKeyOut(KeyBunch key, Person personDrawing, Person personIssuing)
        {
            LogEntry log = new LogEntry
            {
                KeyBunchDrawn = key,
                PersonDrawingKey = personDrawing,
                PersonIssuingKey = personIssuing
            };
            log.Write();
        }

        

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
