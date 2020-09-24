using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Database.DatabaseModels
{
    /// <summary>
    /// Represents a master list of key bunches.<br />
    /// For simplicity, this class only handles the name of the list;
    /// to modify the keys held by the list, assign a KeyList
    /// directly to a KeyBunch, i.e.
    /// keyBunch.KeyList = keyList;
    /// </summary>
    public class KeyList : DatabaseModel
    {
        public string Name { get; set; }

        public override bool IsValid => !(Name is null);

        public override bool Equals(object obj)
        {
            return obj is KeyList list &&
                   ID == list.ID &&
                   Name == list.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, Name);
        }

        /// <summary>
        /// All the keylists in the database.
        /// </summary>
        public static IEnumerable<KeyList> All =>
            DbConnection.Query<KeyList>(
                @"SELECT * FROM KeyLists;"
            );

        /// <summary>
        /// All the key bunches belonging to this keylist.
        /// </summary>
        public IEnumerable<KeyBunch> Keys =>
            DbConnection.Query<KeyBunch>(
                @"SELECT * FROM KeyBunches WHERE keyListId = @ID AND isDeleted = 0;",
                new { ID }
            );

        /// <summary>
        /// All key bunches belonging to this keylist that haven't been returned.
        /// </summary>
        public IEnumerable<KeyBunch> UnreturnedKeys =>
            Keys.Intersect(KeyBunch.Unreturned);

        /// <summary>
        /// Saves the name of the keylist. DOES NOT update the keys in the list.
        /// </summary>
        public override void Write(IDbTransaction transaction)
        {
            if (!IsValid) throw new ArgumentException("KeyList not valid to write to database");
            if (ID is null)
            {
                // Write a new record, save off its ID
                ID = DbConnection.Query<int>(
                        @"INSERT INTO KeyLists (name) VALUES (@Name); 
                          SELECT last_insert_rowid()",
                        new { Name },
                        transaction
                     ).Single();
            }
            else
            {
                // Update the existing record
                DbConnection.Execute(
                    @"UPDATE KeyLists SET name = @Name WHERE id = @ID",
                    new { Name, ID },
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
