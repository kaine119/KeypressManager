using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Database.DatabaseModels
{
    public class Squadron : DatabaseModel
    {
        public string Name { get; set; }
        public IEnumerable<Person> Personnel =>
            DbConnection.Query<Person>(
                @"SELECT * FROM Personnel AS p
                  WHERE p.squadronId = @ID",
                new { ID }
            );

        public override bool IsValid => !(Name is null);

        public static IEnumerable<Squadron> All =>
            DbConnection.Query<Squadron>(
                @"SELECT * FROM Squadrons;"
            );

        public static Squadron ById(int id)
        {
            return DbConnection.Query<Squadron>(
                @"SELECT * FROM Squadrons
                  WHERE id = @ID",
                new { ID = id }
            ).Single();
        }

        /// <summary>
        /// Associates a person to the squadron, and saves the Person record.
        /// </summary>
        /// <param name="person">The person to associate with the squadron.</param>
        public void AddPersonnel(Person person)
        {
            IDbTransaction transaction = DbConnection.BeginTransaction();
            if (person.ID is null)
            {
                person.Write(transaction);
            }
            DbConnection.Execute(
                @"UPDATE personnel
                  SET squadronId = @SquadronId
                  WHERE id = @PersonId",
                new { SquadronId = ID, PersonId = person.ID },
                transaction
            );
            transaction.Commit();
        }

        /// <summary>
        /// Associates personnel to the squadron, and saves the Person records.
        /// </summary>
        /// <param name="personnel">The personnel to associate with the squadron.</param>
        public void AddPersonnel(IEnumerable<Person> personnel)
        {
            foreach (var person in personnel)
            {
                AddPersonnel(person);
            }
        }

        /// <summary>
        /// Saves the squadron. Does not write members; use <see cref="AddPersonnel"/> to add personnel to the squadron.
        /// </summary>
        public override void Write(IDbTransaction transaction)
        {
            if (!IsValid) throw new InvalidOperationException("Squadron not valid to write to database");
            if (ID is null)
            {
                ID =
                    DbConnection.Query<int>(
                        @"INSERT INTO Squadrons (name)
                          VALUES (@Name);
                          SELECT last_insert_rowid();",
                        new { Name },
                        transaction
                    ).Single();
            }
            else
            {
                DbConnection.Execute(
                    @"UPDATE Squadrons
                      SET name = @Name
                      WHERE id = @ID",
                    new { Name, ID },
                    transaction
                );
            }
        }
    }
}
