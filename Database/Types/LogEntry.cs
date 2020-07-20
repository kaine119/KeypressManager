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
        public DateTime DateTimeIssued { get; set; }
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
        public DateTime DateTimeReturned { get; set; }
        /// <summary>
        /// Who returned the key.
        /// </summary>
        public Person PersonReturningKey { get; set; }
        /// <summary>
        /// Who (as a staff member) received the key.
        /// </summary>
        public Person PersonReceivingKey { get; set; }
    }
}
