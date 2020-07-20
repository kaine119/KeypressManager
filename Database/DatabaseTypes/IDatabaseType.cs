using System;
using System.Collections.Generic;
using System.Text;

namespace Database.DatabaseTypes
{
    /// <summary>
    /// Represents a row in the database. Should only be used to write to the database if IsValid.
    /// </summary>
    interface IDatabaseType
    {
        public bool IsValid { get; }
    }
}
