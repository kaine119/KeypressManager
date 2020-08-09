using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;

namespace Database.DatabaseModels
{
    /// <summary>
    /// A line in the keypress log.
    /// </summary>
    public class LogEntry : DatabaseModel
    {
        /// <summary>
        /// The bunch that was drawn.
        /// </summary>
        public KeyBunch KeyBunchDrawn { get; set; }

        /// <summary>
        /// When the key was issued.
        /// </summary>
        public DateTimeOffset? TimeIssued { get; set; }

        /// <summary>
        /// Who drew the key.
        /// </summary>
        public Person PersonDrawingKey { get; set; }

        /// <summary>
        /// Who (as a staff member) issued the key.
        /// </summary>
        public Person PersonIssuingKey { get; set; }

        /// <summary>
        /// When the key was returned.
        /// </summary>
        public DateTimeOffset? TimeReturned { get; set; }

        /// <summary>
        /// Who returned the key.
        /// </summary>
        public Person PersonReturningKey { get; set; }

        /// <summary>
        /// Who (as a staff member) received the key.
        /// </summary>
        public Person PersonReceivingKey { get; set; }

        /// <summary>
        /// Whether the LogEntry is valid. <br />
        /// For the entry to be valid, either only the issuing fields are present, or all the fields are present.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                // If the key hasn't been returned yet, only all three of the issuing fields are required
                // If the key has been returned, both the issuing and the returning fields are required
                // Therefore, either the first three fields are required or all six fields are required.
                return !(KeyBunchDrawn is null)
                    && (!(TimeIssued is null) && !(PersonDrawingKey is null) && !(PersonIssuingKey is null)) // first 3 fields
                    || (!(TimeIssued is null) && !(PersonDrawingKey is null) && !(PersonIssuingKey is null)  // all 6 fields
                        && !(TimeReturned is null) && !(PersonReturningKey is null) && !(PersonReceivingKey is null));
            }
        }

        /// <summary>
        /// Whether the key has been returned.
        /// </summary>
        public bool IsKeyReturned => !(TimeReturned is null);

        public static IEnumerable<LogEntry> All =>
            DbConnection.Query<LogEntry, KeyBunch, Person, Person, Person, Person, LogEntry>(
                @"SELECT log.*, kb.*, personDrawing.*, personIssuing.*, personReturning.*, personReceiving.* FROM LogEntries AS log
                  INNER JOIN KeyBunches AS kb ON log.keyBunchDrawnId = kb.id
                  INNER JOIN Personnel AS personDrawing ON log.personDrawingKeyId = personDrawing.id
                  INNER JOIN Personnel AS personIssuing ON log.personIssuingKeyId = personIssuing.id
                  LEFT OUTER JOIN Personnel AS personReturning ON log.personReturningKeyId = personReturning.id
                  LEFT OUTER JOIN Personnel AS personReceiving ON log.personReceivingKeyId = personReceiving.id",
                (log, kb, personDrawing, personIssuing, personReturning, personReceiving) =>
                {
                    log.KeyBunchDrawn = KeyBunch.ById(kb.ID.Value);
                    log.PersonDrawingKey = personDrawing;
                    log.PersonIssuingKey = personIssuing;
                    if (log.IsKeyReturned)
                    {
                        log.PersonReturningKey = personReturning;
                        log.PersonReceivingKey = personReceiving;
                    }
                    return log;
                }
            );

        /// <summary>
        /// Returns log entries for unreturned keys, i.e. all log entries without a time returned.
        /// </summary>
        /// <returns>All log entries without a timeReturned.</returns>
        public static IEnumerable<LogEntry> ForUnreturnedKeys =>
            DbConnection.Query<LogEntry, KeyBunch, Person, Person, LogEntry>(
                @"SELECT log.*, kb.*, personDrawing.*, personIssuing.* FROM LogEntries AS log
                  INNER JOIN KeyBunches AS kb ON log.keyBunchDrawnId = kb.id
                  INNER JOIN Personnel AS personDrawing ON log.personDrawingKeyId = personDrawing.id
                  INNER JOIN Personnel AS personIssuing ON log.personIssuingKeyId = personIssuing.id
                  WHERE log.timeReturned IS NULL;",
                (log, kb, personDrawing, personIssuing) =>
                {
                    log.KeyBunchDrawn = KeyBunch.ById(kb.ID.Value);
                    log.PersonDrawingKey = personDrawing;
                    log.PersonIssuingKey = personIssuing;
                    return log;
                }
            );

        public static LogEntry LatestForKeyBunch(KeyBunch bunch)
        {
            if (bunch == null) return null;
            return DbConnection.Query<LogEntry, KeyBunch, Person, Person, Person, Person, LogEntry>(
                @"SELECT log.*, kb.*, personDrawing.*, personIssuing.*, personReturning.*, personReceiving.* FROM LogEntries AS log
                  INNER JOIN KeyBunches AS kb ON log.keyBunchDrawnId = kb.id
                  INNER JOIN Personnel AS personDrawing ON log.personDrawingKeyId = personDrawing.id
                  INNER JOIN Personnel AS personIssuing ON log.personIssuingKeyId = personIssuing.id
                  LEFT OUTER JOIN Personnel AS personReturning ON log.personReturningKeyId = personReturning.id
                  LEFT OUTER JOIN Personnel AS personReceiving ON log.personReceivingKeyId = personReceiving.id
                  WHERE log.keyBunchDrawnId = @KeyBunchID                  
                  ORDER BY log.timeIssued DESC LIMIT 1;",
                (log, kb, personDrawing, personIssuing, personReturning, personReceiving) =>
                {
                    log.KeyBunchDrawn = KeyBunch.ById(kb.ID.Value);
                    log.PersonDrawingKey = personDrawing;
                    log.PersonIssuingKey = personIssuing;
                    if (log.IsKeyReturned)
                    {
                        log.PersonReturningKey = personReturning;
                        log.PersonReceivingKey = personReceiving;
                    }
                    return log;
                },
                new { KeyBunchId = bunch.ID }
            ).FirstOrDefault();
        }

        /// <summary>
        /// Writes the log entry to the database.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the entry is not valid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if any of the personnel does not exist.</exception>
        /// <exception cref="PersonNotAuthorizedException">Thrown if any of the personnel is not authorized.</exception>
        public override void Write()
        {
            if (!IsValid) throw new InvalidOperationException("LogEntry object is not valid.");
            if (PersonDrawingKey.ID is null || PersonIssuingKey.ID is null ||
                (IsKeyReturned && (PersonReturningKey.ID is null || PersonReceivingKey.ID is null)))
            {
                throw new InvalidOperationException("One of the personnel in the entry does not exist.");
            }
            if (!KeyBunchDrawn.IsPersonAuthorized(PersonDrawingKey) || (IsKeyReturned && !KeyBunchDrawn.IsPersonAuthorized(PersonReturningKey)))
            {
                throw new PersonNotAuthorizedException();
            }
            if (ID is null)
            {
                ID = DbConnection.Query<int>(
                    @"INSERT INTO LogEntries (
                        keyBunchDrawnId,
                        timeIssued, personDrawingKeyId, personIssuingKeyId,
                        timeReturned, personReturningKeyId, personReceivingKeyId
                    ) VALUES (
                        @KeyBunchDrawnId,
                        @TimeIssued, @PersonDrawingKeyId, @PersonIssuingKeyId,
                        @TimeReturned, @PersonReturningKeyId, @PersonReceivingKeyId
                    ); SELECT last_insert_rowid();",
                    new
                    {
                        KeyBunchDrawnId = KeyBunchDrawn.ID,
                        TimeIssued,
                        PersonDrawingKeyId = PersonDrawingKey.ID,
                        PersonIssuingKeyId = PersonIssuingKey.ID,
                        TimeReturned,
                        PersonReturningKeyId = PersonReturningKey?.ID,
                        PersonReceivingKeyId = PersonReceivingKey?.ID
                    }
                ).Single();
            }
            else
            {
                DbConnection.Execute(
                    @"UPDATE LogEntries
                    SET keyBunchDrawnId = @KeyBunchDrawnId,
                        timeIssued = @TimeIssued, personDrawingKeyId = @PersonDrawingKeyId, personIssuingKeyId = @PersonIssuingKeyId,
                        timeReturned = @TimeReturned, personReturningKeyId = @PersonReturningKeyId, personReceivingKeyId = @PersonReceivingKeyId
                    WHERE id = @ID",
                    new
                    {
                        ID,
                        KeyBunchDrawId = KeyBunchDrawn.ID,
                        TimeIssued,
                        PersonDrawingKeyId = PersonDrawingKey.ID,
                        PersonIssuingKeyId = PersonIssuingKey.ID,
                        TimeReturned,
                        PersonReturningKeyId = PersonReturningKey.ID,
                        PersonReceivingKeyId = PersonReceivingKey.ID
                    });
            }
        }

        /// <summary>
        /// Writes the log entry to the database, optionall creating/updating the personnel in the entry.
        /// </summary>
        /// <param name="updatePersonnel">Whether to create/update the personnel involved in the entry.</param>
        public void Write(bool updatePersonnel)
        {
            if (updatePersonnel)
            {
                PersonDrawingKey.Write();
                PersonIssuingKey.Write();
                PersonReturningKey.Write();
                PersonReceivingKey.Write();
            }

            Write();
        }
    }
}
