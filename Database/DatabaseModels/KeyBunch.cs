using Dapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Database.DatabaseModels
{
    public class KeyBunch : DatabaseModel, INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }

        private string _bunchNumber;
        public string BunchNumber
        {
            get { return _bunchNumber; }
            set
            {
                _bunchNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BunchNumber"));
            }
        }

        private int? _numberOfKeys;

        public int? NumberOfKeys
        {
            get { return _numberOfKeys; }
            set
            {
                _numberOfKeys = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NumberOfKeys"));
            }
        }

        public ObservableCollection<Person> AuthorizedPersonnel { get; set; } = new ObservableCollection<Person>();
        public List<Squadron> AuthorizedSquadrons { get; set; } = new List<Squadron>();

        private KeyList _keyList;

        public KeyList KeyList
        {
            get { return _keyList; }
            set
            {
                _keyList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("KeyList"));
            }
        }

        public override bool IsValid => !(Name is null) && !(BunchNumber is null) && !(NumberOfKeys is null);

        /// <returns>All the keybunches in the database.</returns>
        public static IEnumerable<KeyBunch> All
        {
            get
            {
                var results = new Dictionary<int, KeyBunch>();

                // Join KeyBunch to Person via Authorizations
                // LEFT OUTER JOIN to include any bunches that don't have any auths
                DbConnection.Query<KeyBunch, Person, KeyList, KeyBunch>(
                    @"SELECT kb.*, p.*, kl.* FROM KeyBunches AS kb
                      LEFT OUTER JOIN Authorizations AS a ON a.keyBunchId = kb.id
                      LEFT OUTER JOIN Personnel AS p  ON a.personId = p.id
                      LEFT OUTER JOIN KeyLists AS kl ON kb.keyListId = kl.id
                      ORDER BY kb.keyListId, kb.bunchNumber ASC;",
                    (kb, p, kl) =>
                    {
                        KeyBunch bunch;
                        if (!results.TryGetValue((int)kb.ID, out bunch))
                        {
                            results.Add((int)kb.ID, kb);
                            bunch = kb;
                        }
                        if (!(p is null))
                            bunch.AuthorizedPersonnel.Add(p);
                        bunch.KeyList = kl;
                        return bunch;
                    }
                 );

                // Add the squadrons to the relevant keybunches
                DbConnection.Query<KeyBunch, Squadron, KeyBunch>(
                    @"SELECT kb.*, sqn.* FROM KeyBunches AS kb
                  INNER JOIN SquadronAuthorizations AS sqn_a ON sqn_a.keyBunchId = kb.id
                  INNER JOIN Squadrons AS sqn ON sqn_a.squadronId = sqn.id",
                    (kb, sqn) =>
                    {
                        KeyBunch bunch;
                        if (!results.TryGetValue((int)kb.ID, out bunch))
                        {
                            results.Add((int)kb.ID, kb);
                            bunch = kb;
                        }
                        bunch.AuthorizedSquadrons.Add(Squadron.ById(sqn.ID.Value));
                        return bunch;
                    }
                );

                return results.Values;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Match(string searchTerm)
        {
            return Regex.IsMatch(BunchNumber, searchTerm, RegexOptions.IgnoreCase) || Regex.IsMatch(Name, searchTerm, RegexOptions.IgnoreCase);
        }

        public static KeyBunch ById(int id)
        {
            KeyBunch result = DbConnection.Query<KeyBunch, KeyList, KeyBunch>(
                @"SELECT kb.*, kl.* FROM KeyBunches AS kb
                  LEFT OUTER JOIN KeyLists AS kl ON kb.keyListId = kl.id
                  WHERE kb.id = @ID;",
                (kb, kl) =>
                {
                    kb.KeyList = kl;
                    return kb;
                },
                new { ID = id }
            ).SingleOrDefault();

            // Join KeyBunch to Person via Authorizations
            // LEFT OUTER JOIN to include any bunches that don't have any auths
            DbConnection.Query<KeyBunch,Person, KeyBunch>(
                @"SELECT kb.*, p.* FROM KeyBunches as kb
                  LEFT OUTER JOIN Authorizations AS a ON a.keyBunchId = kb.id
                  LEFT OUTER JOIN Personnel AS p  ON a.personId = p.id
                  WHERE kb.id = @ID",
                (kb, p) =>
                {
                    result.AuthorizedPersonnel.Add(p);
                    return result;
                },
                new { ID = id }
             );

            // Add the squadrons to the relevant keybunches
            DbConnection.Query<KeyBunch, Squadron, KeyBunch>(
                @"SELECT kb.*, sqn.* FROM KeyBunches AS kb
                  INNER JOIN SquadronAuthorizations AS sqn_a ON sqn_a.keyBunchId = kb.id
                  INNER JOIN Squadrons AS sqn ON sqn_a.squadronId = sqn.id",
                (kb, sqn) =>
                {
                    result.AuthorizedSquadrons.Add(Squadron.ById(sqn.ID.Value));
                    return result;
                }
            );

            return result;
        }

        /// <summary>
        /// All the keys in the database that haven't been returned.
        /// </summary>
        public static IEnumerable<KeyBunch> Unreturned => LogEntry.ForUnreturnedKeys.Select(log => log.KeyBunchDrawn);

        public static IEnumerable<KeyBunch> Returned => All.Except(Unreturned);

        public override bool Equals(object obj)
        {
            return obj is KeyBunch bunch &&
                   ID == bunch.ID &&
                   Name == bunch.Name &&
                   BunchNumber == bunch.BunchNumber;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, Name, BunchNumber);
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

        public bool IsPersonAuthorized(Person person) =>
            AuthorizedPersonnel.Contains(person) || AuthorizedSquadrons.Any(sqn => sqn.Personnel.Contains(person));

        /// <summary>
        /// Save the keybunch to the database, and updates authorizations for personnel and squadrons.
        /// Also inserts/updates personnel records for any new/modified personnel, but not for squadrons.
        /// </summary>
        public override void Write(IDbTransaction transaction)
        {
            if (!IsValid) throw new ArgumentException("Keybunch not valid to write to database");
            if (ID is null)
            {
                // Write the KeyBunch record, and get the (new) KeyBunch ID.
                ID = DbConnection.Query<int>(
                        @"INSERT INTO KeyBunches (name, bunchNumber, numberOfKeys, keyListId) 
                          VALUES (@Name, @BunchNumber, @NumberOfKeys, @KeyListId); 
                          SELECT last_insert_rowid()",
                        new { Name, BunchNumber, NumberOfKeys, KeyListId = KeyList.ID },
                        transaction
                     ).Single();
            }
            else
            {
                // Update the existing KeyBunch record.
                DbConnection.Execute(
                    @"UPDATE KeyBunches
                      SET name = @Name,
                      bunchNumber = @BunchNumber,
                      numberOfKeys = @NumberOfKeys,
                      keyListId = @KeyListId
                      WHERE id = @ID",
                    new { ID, Name, BunchNumber, NumberOfKeys, KeyListId = KeyList.ID },
                    transaction
                    );
            }

            // i'm probably gonna regret this, but...
            // Delete all the key bunch's authorizations,
            // to purge any newly denied authorizations and eliminate
            // collision errors.
            DbConnection.Execute(
                @"DELETE FROM Authorizations
                  WHERE keyBunchId = @KeyBunchId",
                new { KeyBunchId = ID },
                transaction
            );
            DbConnection.Execute(
                @"DELETE FROM SquadronAuthorizations
                  WHERE keyBunchId = @KeyBunchId",
                new { KeyBunchId = ID },
                transaction
            );

            // Write the authorizations in the Join table.
            foreach (var person in AuthorizedPersonnel)
            {
                person.Write(transaction);
                DbConnection.Execute(
                    @"INSERT INTO Authorizations (keyBunchId, personId)
                      VALUES (@KeyBunchId, @PersonId)",
                    new { KeyBunchId = ID, PersonId = person.ID },
                    transaction
                );
            }

            // Write squadron authorizations in the join table.
            foreach (var squadron in AuthorizedSquadrons)
            {
                DbConnection.Execute(
                    @"INSERT INTO SquadronAuthorizations (keyBunchId, squadronId)
                      VALUES (@KeyBunchId, @SquadronId)",
                    new { KeyBunchId = ID, SquadronId = squadron.ID },
                    transaction
                );
            }
        }
    }
}
