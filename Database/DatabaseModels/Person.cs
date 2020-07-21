
using Dapper;
using System;
using System.Collections.Generic;

namespace Database.DatabaseModels
{
    /// <summary>
    /// A person, either as a key drawer/returner (customer) or a key issuer/receiver (staff).
    /// </summary>
    public class Person: DatabaseModel
    {
        public string? NRIC { get; set; }
        public string? Name { get; set; }
        public Rank? Rank { get; set; }
        public string? ContactNumber { get; set; }

        public override bool IsValid => !(NRIC is null) && !(Name is null) && !(Rank is null);

        public static IEnumerable<Person> GetAll()
        {
            return DbConnection.Query<Person>("SELECT * FROM Personnel");
        }

        public override void Write()
        {
            if (!IsValid) throw new ArgumentException("Object not valid to write to database");
            throw new System.NotImplementedException();
        }
    }
}
