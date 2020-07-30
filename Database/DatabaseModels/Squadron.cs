using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace Database.DatabaseModels
{
    public class Squadron : DatabaseModel
    {
        public string Name { get; set; }
        public List<Person> Personnel { get; set; } = new List<Person>();

        public override bool IsValid => !(Name is null);

        public static IEnumerable<Squadron> GetAll()
        {
            var results = new Dictionary<int, Squadron>();

            DbConnection.Query<Squadron, Person, Squadron>(
                @"SELECT s.*, p.* FROM Squadrons AS s
                    LEFT OUTER JOIN Personnel AS p ON s.id = p.squadronId;",
                (s, p) =>
                {
                    if (!results.TryGetValue((int)s.ID, out Squadron sqn))
                    {
                        results.Add((int)s.ID, s);
                        sqn = s;
                    }
                    if (!(p is null))
                    {
                        sqn.Personnel.Add(p);
                    }
                    return sqn;
                }
            );
            return results.Values;
        }

        /// <summary>
        /// Saves the squadron and its members. <br />
        /// If a member is newly created, its whole record is inserted to the database,
        /// otherwise only its squadron association is updated.
        /// </summary>
        public override void Write()
        {
            if (ID is null)
            {
                ID = DbConnection.Query<int>(
                    @"INSERT INTO Squadrons (name)
                      VALUES (@Name);
                      SELECT last_insert_rowid();",
                    new { Name }
                ).Single();
            }
            else
            {
                DbConnection.Execute(
                    @"UPDATE Squadrons
                      SET name = @Name
                      WHERE id = @ID",
                    new { Name, ID }
                );
            }

            // TODO: come up with something better than this
            // Remove all squadronId for this squadron
            DbConnection.Execute(
                @"UPDATE personnel
                  SET squadronId = NULL
                  WHERE squadronId = @SquadronID;",
                new { SquadronId = ID }
            );

            // write squadronId for all personnel entries
            foreach (var person in Personnel)
            {
                if (person.ID is null) person.Write(); // Write any new personnel first
                DbConnection.Execute(
                    @"UPDATE personnel
                      SET squadronId = @SquadronId
                      WHERE id = @PersonId",
                    new { SquadronId = ID, PersonId = person.ID }
                );
            }
        }
    }
}
