using Dapper;
using Database.DatabaseModels;
using System;
using System.Data;
using System.Linq;

namespace Database
{
    public class KeypressDatabase
    {
        public static void Initialize(string databasePath)
        {
            SqlMapper.RemoveTypeMap(typeof(DateTimeOffset));
            SqlMapper.RemoveTypeMap(typeof(DateTimeOffset?));
            SqlMapper.AddTypeHandler(typeof(DateTimeOffset?), new UnixTimestampDateTimeHandler());
            DatabaseModel.EstablishConnection($"Data source={databasePath}");
        }

        /// <summary>
        /// Book a key out.
        /// </summary>
        /// <param name="key">The key bunch to book out.</param>
        /// <param name="personDrawing">The person drawing the key.</param>
        /// <param name="personIssuing">The person issuing the key.</param>
        /// <exception cref="PersonNotAuthorizedException">Thrown when the person drawing the key is not authorized.</exception>
        public void BookKeyOut(KeyBunch key, Person personDrawing, Person personIssuing)
        {
            if (KeyBunch.Unreturned.Contains(key))
            {
                throw new InvalidOperationException("Key is already booked out.");
            }
            LogEntry log = new LogEntry
            {
                TimeIssued = DateTimeOffset.Now,
                KeyBunchDrawn = key,
                PersonDrawingKey = personDrawing,
                PersonIssuingKey = personIssuing
            };
            log.Write();
        }

        /// <summary>
        /// Book a key in.
        /// </summary>
        /// <param name="key">The key bunch to book in.</param>
        /// <param name="personReturning">The person returning the key.</param>
        /// <param name="personReceiving">The person receiving the key.</param>
        /// <exception cref="InvalidOperationException">Thrown when the key wasn't booked out in the first place.</exception>
        /// <exception cref="PersonNotAuthorizedException">Thrown when the person returning the key is not authorized.</exception>
        public void BookKeyIn(KeyBunch key, Person personReturning, Person personReceiving)
        {
            LogEntry log = LogEntry.ForUnreturnedKeys
                                   .Single(log => log.KeyBunchDrawn == key);
            log.TimeIssued = DateTimeOffset.Now;
            log.PersonReturningKey = personReturning;
            log.PersonReceivingKey = personReceiving;
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
    public class PersonNotAuthorizedException : Exception
    {
        public PersonNotAuthorizedException() : base() { }
        public PersonNotAuthorizedException(string message) : base(message) { }
    }
}
