using Dapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace Database.DatabaseModels
{
    public class Squadron : DatabaseModel
    {
        public string Name { get; set; }
        public ObservableCollection<Person> Personnel { get; set; } = new ObservableCollection<Person>();

        public override bool Equals(object obj)
        {
            return obj is Squadron squadron &&
                   Name == squadron.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        public override bool IsValid => !(Name is null);

        public static IEnumerable<Squadron> All
        {
            get
            {
                var results = new Dictionary<int, Squadron>();

                DbConnection.Query<Squadron, Person, Squadron>(
                    @"SELECT s.*, p.* FROM Squadrons AS s
                      LEFT OUTER JOIN PersonnelSquadrons AS p_s ON s.id = p_s.squadronId
                      LEFT OUTER JOIN personnel AS p ON p_s.personId = p.id;",
                    (s, p) =>
                    {
                        Squadron squadron;
                        if (!results.TryGetValue((int)s.ID, out squadron))
                        {
                            results.Add((int)s.ID, s);
                            squadron = s;
                        }
                        if (!(p is null))
                           squadron.Personnel.Add(p);
                        return squadron;
                    }
                );

                return results.Values;
            }
        }


        public static Squadron ById(int id)
        {
            Squadron result = null;

            DbConnection.Query<Squadron, Person, Squadron>(
                @"SELECT s.*, p.* FROM PersonnelSquadrons as p_s
                      LEFT OUTER JOIN squadrons AS s ON p_s.squadronId = s.id
                      LEFT OUTER JOIN personnel AS p ON p_s.personId = p.id
                  WHERE s.id = @SquadronID;",
                (s, p) =>
                {
                    if (result == null)
                    {
                        result = s;
                    }
                    if (!(p is null))
                        result.Personnel.Add(p);
                    return result;
                },
                new { SquadronID = id });

            return result;
        }

        /// <summary>
        /// Saves the squadron and its members.
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

            // First delete all associations to reflect any deleted personnel...
            DbConnection.Execute(
                @"DELETE FROM PersonnelSquadrons
                  WHERE squadronId = @SquadronID",
                new { SquadronID = ID },
                transaction
            );

            // ...then add them back.
            foreach (Person person in Personnel)
            {
                person.Write(transaction);
                DbConnection.Execute(
                    @"INSERT INTO PersonnelSquadrons (personId, squadronId)
                        VALUES (@PersonID, @SquadronID)",
                    new { PersonID = person.ID, SquadronID = ID },
                    transaction
                );
            }
        }

        public override void Delete(IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
