using System.Linq;
using Dapper;
using System;
using System.Collections.Generic;

namespace Database.DatabaseModels
{
    /// <summary>
    /// A person, either as a key drawer/returner (customer) or a key issuer/receiver (staff).
    /// </summary>
    public class Person : DatabaseModel
    {
        public string NRIC { get; set; }
        public string Name { get; set; }
        public Rank? Rank { get; set; }
        public string ContactNumber { get; set; }

        public override bool IsValid => !(NRIC is null) && !(Name is null) && !(Rank is null);

        public static IEnumerable<Person> GetAll()
        {
            return DbConnection.Query<Person>("SELECT * FROM Personnel");
        }

        /// <summary>
        /// Gets the keys this person is authorized to draw.
        /// </summary>
        public IEnumerable<KeyBunch> AuthorizedKeys =>
            DbConnection.Query<KeyBunch>(@"SELECT kb.* FROM Authorizations AS a
                                            JOIN Personnel AS p ON a.personId = p.id
                                            JOIN KeyBunches as kb ON a.keyBunchId = kb.id
                                            WHERE p.id = @personId",
                                         new { personId = ID });

        public override void Write()
        {
            if (!IsValid) throw new ArgumentException("Person not valid to write to database");
            if (ID is null)
            {
                // The personnel doesn't exist yet, write a new one
                ID = DbConnection.Query<int>(@"INSERT INTO personnel (nric, name, rank, contactNumber)
                                                VALUES (@NRIC, @Name, @Rank, @ContactNumber);
                                               SELECT last_insert_rowid();",
                                             new { NRIC, Name, Rank, ContactNumber }).Single();
            }
            else
            {
                // The personnel exists, update the existing one
                DbConnection.Execute(@"UPDATE TABLE personnel
                                        SET nric = @NRIC, name = @Name, rank = @Rank, contactNumber = @ContactNumber
                                        WHERE id = @ID",
                                     new { NRIC, Name, Rank, ContactNumber });
            }
        }
    }
}
