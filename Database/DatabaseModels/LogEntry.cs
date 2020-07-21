using System;
using System.Collections.Generic;
using System.Text;

namespace Database.DatabaseModels
{
    /// <summary>
    /// A line in the keypress log.
    /// </summary>
    public class LogEntry: DatabaseModel
    {
        /// <summary>
        /// When the key was issued.
        /// </summary>
        public DateTime? DateTimeIssued { get; set; }
        /// <summary>
        /// Who drew the key.
        /// </summary>
        public Person? PersonDrawingKey { get; set; }
        /// <summary>
        /// Who (as a staff member) issued the key.
        /// </summary>
        public Person? PersonIssuingKey { get; set; }
        /// <summary>
        /// When the key was returned.
        /// </summary>
        public DateTime? DateTimeReturned { get; set; }
        /// <summary>
        /// Who returned the key.
        /// </summary>
        public Person? PersonReturningKey { get; set; }
        /// <summary>
        /// Who (as a staff member) received the key.
        /// </summary>
        public Person? PersonReceivingKey { get; set; }

        public override bool IsValid { 
            get
            {
                // If the key hasn't been returned yet, only all three of the issuing fields are required
                // If the key has been returned, both the issuing and the returning fields are required
                // Therefore, either the first three fields are required or all six fields are required.
                return (!(DateTimeIssued is null) && !(PersonDrawingKey is null) && !(PersonIssuingKey is null)) // first 3 fields
                    || (!(DateTimeIssued is null) && !(PersonDrawingKey is null) && !(PersonIssuingKey is null)  // all 6 fields
                        && !(DateTimeReturned is null) && !(PersonReturningKey is null) && !(PersonReceivingKey is null));
            } 
        }

        /// <summary>
        /// Whether the key has been returned.
        /// </summary>
        public bool IsKeyReturned
        {
            get
            {
                return !(DateTimeReturned is null);
            }
        }

    }
}
