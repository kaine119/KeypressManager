using Database;
using Database.DatabaseModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DatabaseTest
{
    [TestClass, TestCategory("Models")]
    public class KeyBunchTest
    {
        private KeypressDatabase db = DatabaseTestHelper.TestDatabase;

        [TestMethod]
        public void KeyBunch_InstantiatesCorrectly()
        {
            KeyBunch mess = db.AllKeyBunches.First();
            Assert.AreEqual("Mess", mess.Name);
            Assert.AreEqual("01", mess.BunchNumber);
            Assert.AreEqual("Alice Tan", mess.AuthorizedPersonnel.First().Name);
            Assert.AreEqual("Main Keypress", mess.KeyList.Name);

            KeyBunch hq = db.AllKeyBunches.ElementAt(2);
            Assert.AreEqual("111 SQN", hq.AuthorizedSquadrons.First().Name);
        }

        [TestMethod]
        public void Unreturned_GetsUnreturnedKeys()
        {
            List<KeyBunch> keys = db.UnreturnedKeys.ToList();
            Assert.AreEqual(1, keys.Count());
            Assert.AreEqual("Office", keys.First().Name);
        }

        [TestMethod]
        public void Write_InsertsNewRecordForExistingPersonnel()
        {
            int initialAllPersonnelCount = db.AllPersonnel.Count();
            ObservableCollection<Person> personnel = new ObservableCollection<Person>(db.AllPersonnel.Take(2));
            KeyBunch newBunch = new KeyBunch
            {
                Name = "New key bunch",
                BunchNumber = "B1001",
                AuthorizedPersonnel = personnel,
                NumberOfKeys = 25,
                KeyList = db.AllKeyLists.First()
            };
            newBunch.Write();
            // Assert no new personnel is written
            Assert.AreEqual(initialAllPersonnelCount, db.AllPersonnel.Count());

            // Compare what's written to what was supposed to be written
            KeyBunch writtenBunch = db.AllKeyBunches.Single(bunch => bunch.Name == "New key bunch");
            Assert.AreEqual(newBunch.BunchNumber, writtenBunch.BunchNumber);
            Assert.IsTrue(newBunch.AuthorizedPersonnel.SequenceEqual(writtenBunch.AuthorizedPersonnel));
            Assert.AreEqual(newBunch.NumberOfKeys, writtenBunch.NumberOfKeys);
        }

        [TestMethod]
        public void Write_InsertsNewRecordAndWritesNewPersonnel()
        {
            int initialAllPersonnelCount = db.AllPersonnel.Count();
            var personnel = new ObservableCollection<Person>
            {
                new Person { Name = "KeyBunchTestWrite New Person 1", NRIC = "909Z", Rank = Rank.PTE },
                new Person { Name = "KeyBunchTestWrite New Person 2", NRIC = "910Z", Rank = Rank.PTE }
            };
            KeyBunch newBunch = new KeyBunch
            {
                Name = "New key bunch 2",
                BunchNumber = "B1002",
                AuthorizedPersonnel = personnel,
                NumberOfKeys = 25,
                KeyList = db.AllKeyLists.First()
            };

            newBunch.Write();

            // Assert the new personnel were written
            Assert.AreEqual(initialAllPersonnelCount + 2, db.AllPersonnel.Count());
        }

        [TestMethod]
        public void Write_InsertsNewRecordAndWritesSquadron()
        {
            ObservableCollection<Squadron> squadrons = new ObservableCollection<Squadron>(db.AllSquadrons);
            KeyBunch newBunch = new KeyBunch
            {
                Name = "New key bunch 3",
                BunchNumber = "B1003",
                AuthorizedSquadrons = squadrons,
                NumberOfKeys = 25,
                KeyList = db.AllKeyLists.First()
            };

            newBunch.Write();

            KeyBunch writtenBunch = db.AllKeyBunches.Single(bunch => bunch.BunchNumber == "B1003");
            Assert.AreEqual("111 SQN", writtenBunch.AuthorizedSquadrons.First().Name);
        }

        [TestMethod]
        public void Write_EditsExistingBunch()
        {
            KeyBunch target = db.AllKeyBunches.Single(bunch => bunch.Name == "Edit target");
            target.Name = "Edit target (edited)";
            target.Write();
            Assert.AreEqual(db.AllKeyBunches.Count(bunch => bunch.Name == "Edit target"), 0);
            Assert.AreEqual(db.AllKeyBunches.Count(bunch => bunch.Name == "Edit target (edited)"), 1);
        }

        [TestMethod]
        public void Delete_DeletesKeyBunches()
        {
            KeyBunch target = db.AllKeyBunches.Single(bunch => bunch.Name == "HQ");
            target.Delete();
            Assert.IsFalse(db.AllKeyBunches.Any(bunch => bunch.Name == "HQ"));
        }

        [TestMethod]
        public void IsPersonnelAuthorized_ReturnsCorrectly()
        {
            KeyBunch mess = db.AllKeyBunches.First();
            Person alice = db.AllPersonnel.First();
            Person bob = db.AllPersonnel.ElementAt(1);
            Assert.IsTrue(mess.IsPersonAuthorized(alice));
            Assert.IsFalse(mess.IsPersonAuthorized(bob));
        }

        [TestMethod]
        public void IsPersonnelAuthorized_ReturnsThroughSquadrons()
        {
            KeyBunch hq = db.AllKeyBunches.ElementAt(2);
            Person bob = db.AllPersonnel.ElementAt(1);
            Assert.IsTrue(hq.IsPersonAuthorized(bob));
        }
    }
}
