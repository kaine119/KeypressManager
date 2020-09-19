using System.Linq;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data;

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

        public override bool Equals(object obj)
        {
            if (!(obj is Person)) return false;
            var person = (Person)obj;
            return NRIC == person.NRIC && Name == person.Name && Rank == person.Rank && ContactNumber == person.ContactNumber;
        }

        public override bool IsValid => !(NRIC is null) && !(Name is null) && !(Rank is null);

        /// <summary>
        /// All the squadrons that this personnel is in.
        /// </summary>
        public IEnumerable<Squadron> Squadrons
        {
            get
            {
                var results = new Dictionary<int, Squadron>();

                DbConnection.Query<Squadron, Person, Squadron>(
                    @"SELECT s.*, p.* FROM PersonnelSquadrons as p_s
                      LEFT OUTER JOIN squadrons AS s ON p_s.squadronId = s.id
                      LEFT OUTER JOIN personnel AS p ON p_s.personId = p.id
                      WHERE p.id = @PersonID;",
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
                    },
                    new { PersonID = ID }
                );

                return results.Values;
            }
        }

        /// <summary>
        /// All the personnel in the database.
        /// </summary>
        public static IEnumerable<Person> All =>
            DbConnection.Query<Person>(
                @"SELECT p.* FROM Personnel AS p;"
            );

        /// <summary>
        /// All the configured staff in the database.
        /// </summary>
        public static IEnumerable<Person> AllStaff =>
            DbConnection.Query<Person>(
                @"SELECT p.* FROM Staff AS st
                  JOIN Personnel AS p ON st.personId = p.id;"
            );

        /// <summary>
        /// Write a list of personnel to the Staff list. 
        /// </summary>
        /// <param name="staff">The list of personnel to be written as Staff.</param>
        public static void WriteStaff(IEnumerable<Person> staff)
        {
            foreach (Person staffMember in staff) staffMember.Write();
            IDbTransaction transaction = DbConnection.BeginTransaction();
            DbConnection.Execute(
                @"DELETE FROM Staff;",
                transaction
            );
            foreach (Person staffMember in staff)
            {
                DbConnection.Execute(
                    @"INSERT INTO Staff (personId) VALUES (@StaffId)",
                    new { StaffId = staffMember.ID },
                    transaction
                );
            }
            transaction.Commit();
        }

        /// <summary>
        /// Checks if a string matches this person's NRIC or name. <br/>
        /// If the search term matches the end of an NRIC (i.e. 1~3 digits followed by a letter),
        /// the term is matched against the person's NRIC; otherwise, it is matched against
        /// the person's name.
        /// </summary>
        /// <param name="searchTerm">The search term to match against.</param>
        /// <returns>Whether searchTerm matches the person.</returns>
        public bool Match(string searchTerm) =>
            Regex.IsMatch(
                Regex.IsMatch(searchTerm, @"\d{1,3}[A-Za-z]?$") ? NRIC : Name,
                searchTerm,
                RegexOptions.IgnoreCase
            );

        /// <summary>
        /// Gets the keys this person is authorized to draw.
        /// </summary>
        public IEnumerable<KeyBunch> AuthorizedKeys =>
            DbConnection.Query<KeyBunch>(@"SELECT kb.* FROM Authorizations AS a
                                            JOIN Personnel AS p ON a.personId = p.id
                                            JOIN KeyBunches as kb ON a.keyBunchId = kb.id
                                            WHERE p.id = @personId",
                                         new { personId = ID });

        /// <summary>
        /// Writes the person to the database.
        /// Does NOT update squadron associations; use Squadron.Write().
        /// </summary>
        public override void Write(IDbTransaction transaction)
        {
            if (!IsValid) throw new ArgumentException("Person not valid to write to database");
            if (ID is null)
            {
                // The personnel doesn't exist yet, write a new one
                ID = 
                    DbConnection.Query<int>(
                        @"INSERT INTO personnel (nric, name, rank, contactNumber)
                        VALUES (@NRIC, @Name, @Rank, @ContactNumber);
                        SELECT last_insert_rowid();",
                        new { NRIC, Name, Rank, ContactNumber },
                        transaction
                    ).Single();
            }
            else
            {
                // The personnel exists, update the existing one
                DbConnection.Execute(
                    @"UPDATE personnel 
                      SET nric = @NRIC, name = @Name, rank = @Rank, contactNumber = @ContactNumber 
                      WHERE id = @ID",
                    new { NRIC, Name, Rank, ContactNumber, ID },
                    transaction
                );
            }
        }
    }
}
