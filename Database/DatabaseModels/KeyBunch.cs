using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Database.DatabaseModels
{
    public class KeyBunch: DatabaseModel
    {
        public string Name { get; set; }
        public string BunchNumber { get; set; }
        public int? NumberOfKeys { get; set; }
        public List<Person> AuthorizedPersonnel { get; set; } = new List<Person>();
        public KeyList KeyList { get; set; }

        public override bool IsValid => !(Name is null) && !(BunchNumber is null) && !(NumberOfKeys is null);

        /// <returns>All the keybunches in the database.</returns>
        public static IEnumerable<KeyBunch> GetAll()
        {
            var results = new Dictionary<int, KeyBunch>();
            // Join KeyBunch to Person via Authorizations
            // LEFT OUTER JOIN to include any bunches that don't have any auths
            DbConnection.Query<KeyBunch, Person, KeyList, KeyBunch>(@"
                SELECT kb.*, p.*, kl.* FROM KeyBunches AS kb
                LEFT OUTER JOIN Authorizations AS a ON a.keyBunchId = kb.id
                LEFT OUTER JOIN Personnel  AS p  ON a.personId = p.id
                LEFT OUTER JOIN KeyLists AS kl ON kb.keyListId = kl.id;",
                (keyBunch, person, keyList) =>
                {
                    KeyBunch bunch;
                    if (!results.TryGetValue((int) keyBunch.ID, out bunch))
                        results.Add((int) keyBunch.ID, bunch = keyBunch);
                    if (!(person is null)) 
                        bunch.AuthorizedPersonnel.Add(person);
                    bunch.KeyList = keyList;
                    return bunch;
                }
             );

            return results.Values;
        }

        /// <summary>
        /// Looks up this keybunch's Authorized Personnel list for the NRIC or name given.
        /// Purely numeric queries, or queries matching the last 3 digits + letter of an NRIC, 
        /// will be treated as an NRIC.
        /// </summary>
        /// <param name="query">A name or NRIC</param>
        /// <returns>Any matching personnel.</returns>
        public IEnumerable<Person> FindPersonnel(string query)
        {
            bool queryIsNRIC = Regex.IsMatch(query, @"\d{1,3}[A-Za-z]?$");
            if (queryIsNRIC)
            {
                return AuthorizedPersonnel.Where(personnel => Regex.IsMatch(personnel.NRIC, query, RegexOptions.IgnoreCase));
            }
            else
            {
                return AuthorizedPersonnel.Where(personnel => Regex.IsMatch(personnel.Name, query, RegexOptions.IgnoreCase));
            }
        }

        /// <summary>
        /// Save the keybunch to the database.
        /// Also inserts/updates personnel records for any AuthorizedPersonnel.
        /// </summary>
        public override void Write()
        {
            if (!IsValid) throw new ArgumentException("Keybunch not valid to write to database");
            if (ID is null)
            {
                // Write the KeyBunch record, and get the (new) KeyBunch ID.
                ID = DbConnection.Query<int>(@"
                        INSERT INTO KeyBunches (name, bunchNumber, numberOfKeys, keyListId)
                        VALUES (@Name, @BunchNumber, @NumberOfKeys, @KeyListId);
                        SELECT last_insert_rowid()",
                        new { Name, BunchNumber, NumberOfKeys, KeyListId = KeyList.ID }).Single();
            }
            else
            {
                // Update the existing KeyBunch record.
                // TODO: not hardcode the keyListId
                DbConnection.Execute(@"
                    UPDATE KeyBunches
                    SET name = @Name,
                        bunchNumber = @BunchNumber,
                        numberOfKeys = @NumberOfKeys,
                        keyListId = @KeyListId
                    WHERE id = @ID",
                    new { ID, Name, BunchNumber, NumberOfKeys, KeyListId = KeyList.ID });
            }

            // i'm probably gonna regret this, but...
            // Delete all the key bunch's authorizations,
            // to purge any newly denied authorizations and eliminate
            // collision errors.
            DbConnection.Execute(@"
                    DELETE FROM authorizations
                    WHERE keyBunchId = @KeyBunchId",
                    new { KeyBunchId = ID });

            // Write the authorizations in the Join table.
            foreach (var person in AuthorizedPersonnel)
            {
                person.Write();
                DbConnection.Execute(@"
                    INSERT INTO authorizations (keyBunchId, personId)
                    VALUES (@keyBunchID, @personID)",
                    new { keyBunchId = ID, personID = person.ID }
                );
            }
        }
    }
}
