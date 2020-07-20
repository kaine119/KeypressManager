using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Types
{
    /// <summary>
    /// A line in the keypress log.
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// When the key was issued.
        /// </summary>
        public DateTime DateTimeIssued { get; }
        /// <summary>
        /// Who drew the key.
        /// </summary>
        public Person PersonDrawingKey { get; }
        /// <summary>
        /// Who (as a staff member) issued the key.
        /// </summary>
        public Person PersonIssuingKey { get; }
        /// <summary>
        /// When the key was returned.
        /// </summary>
        public DateTime DateTimeReturned { get; }
        /// <summary>
        /// Who returned the key.
        /// </summary>
        public Person PersonReturningKey { get; }
        /// <summary>
        /// Who (as a staff member) received the key.
        /// </summary>
        public Person PersonReceivingKey { get; }
    }
}
