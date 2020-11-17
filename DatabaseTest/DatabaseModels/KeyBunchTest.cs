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
        [TestMethod]
        public void KeyBunch_InstantiatesCorrectly()
        {
            KeyBunch mess = KeyBunch.All.Single(bunch => bunch.Name == "Mess");
            Assert.AreEqual("Mess", mess.Name);
            Assert.AreEqual("01", mess.BunchNumber);
            Assert.AreEqual("Alice Tan", mess.AuthorizedPersonnel.First().Name);
            Assert.AreEqual("Main Keypress", mess.KeyList.Name);

            KeyBunch hq = KeyBunch.All.Single(bunch => bunch.Name == "HQ");
            Assert.AreEqual("111 SQN", hq.AuthorizedSquadrons.First().Name);
        }

        [TestMethod]
        public void Unreturned_GetsUnreturnedKeys()
        {
            List<KeyBunch> keys = KeyBunch.Unreturned.ToList();
            Assert.AreEqual(1, keys.Count());
            Assert.AreEqual("Office", keys.First().Name);
        }

        [TestMethod]
        public void Write_InsertsNewRecordForExistingPersonnel()
        {
            int initialAllPersonnelCount = Person.All.Count();
            ObservableCollection<Person> personnel = new ObservableCollection<Person>(Person.All.Take(2));
            KeyBunch newBunch = new KeyBunch
            {
                Name = "New key bunch",
                BunchNumber = "B1001",
                AuthorizedPersonnel = personnel,
                NumberOfKeys = 25,
                KeyList = KeyList.All.First()
            };
            newBunch.Write();
            // Assert no new personnel is written
            Assert.AreEqual(initialAllPersonnelCount, Person.All.Count());

            // Compare what's written to what was supposed to be written
            KeyBunch writtenBunch = KeyBunch.All.Single(bunch => bunch.Name == "New key bunch");
            Assert.AreEqual(newBunch.BunchNumber, writtenBunch.BunchNumber);
            Assert.IsTrue(newBunch.AuthorizedPersonnel.SequenceEqual(writtenBunch.AuthorizedPersonnel));
            Assert.AreEqual(newBunch.NumberOfKeys, writtenBunch.NumberOfKeys);
        }

        [TestMethod]
        public void Write_InsertsNewRecordAndWritesNewPersonnel()
        {
            int initialAllPersonnelCount = Person.All.Count();
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
                KeyList = KeyList.All.First()
            };

            newBunch.Write();

            // Assert the new personnel were written
            Assert.AreEqual(initialAllPersonnelCount + 2, Person.All.Count());
        }

        [TestMethod]
        public void Write_InsertsNewRecordAndWritesSquadron()
        {
            ObservableCollection<Squadron> squadrons = new ObservableCollection<Squadron>(Squadron.All);
            KeyBunch newBunch = new KeyBunch
            {
                Name = "New key bunch 3",
                BunchNumber = "B1003",
                AuthorizedSquadrons = squadrons,
                NumberOfKeys = 25,
                KeyList = KeyList.All.First()
            };

            newBunch.Write();

            KeyBunch writtenBunch = KeyBunch.All.Single(bunch => bunch.BunchNumber == "B1003");
            Assert.AreEqual("111 SQN", writtenBunch.AuthorizedSquadrons.First().Name);
        }

        [TestMethod]
        public void Write_EditsExistingBunch()
        {
            KeyBunch target = KeyBunch.All.Single(bunch => bunch.Name == "Edit target");
            target.Name = "Edit target (edited)";
            target.Write();
            Assert.AreEqual(KeyBunch.All.Count(bunch => bunch.Name == "Edit target"), 0);
            Assert.AreEqual(KeyBunch.All.Count(bunch => bunch.Name == "Edit target (edited)"), 1);
        }

        [TestMethod]
        public void Delete_DeletesKeyBunches()
        {
            KeyBunch target = KeyBunch.All.Single(bunch => bunch.Name == "Office");
            target.Delete();
            Assert.IsFalse(KeyBunch.All.Any(bunch => bunch.Name == "Office"));
        }

        [TestMethod]
        public void DeleteMultiple_DeletesMultipleKeyBunches()
        {
            IEnumerable<KeyBunch> targets = KeyBunch.All.Where(bunch => bunch.KeyList.Name == "Keypress to delete");
            KeyBunch.DeleteMultiple(targets);
            Assert.AreEqual(0, KeyBunch.All.Where(bunch => bunch.KeyList.Name == "Keypress to delete").Count());
        }

        [TestMethod]
        public void IsPersonnelAuthorized_ReturnsCorrectly()
        {
            KeyBunch mess = KeyBunch.All.First();
            Person alice = Person.All.First();
            Person bob = Person.All.ElementAt(1);
            Assert.IsTrue(mess.IsPersonAuthorized(alice));
            Assert.IsFalse(mess.IsPersonAuthorized(bob));
        }

        [TestMethod]
        public void IsPersonnelAuthorized_ReturnsThroughSquadrons()
        {
            KeyBunch hq = KeyBunch.All.Single(bunch => bunch.Name == "HQ");
            Person bob = Person.All.Single(person => person.Name == "Bob Lee");
            Assert.IsTrue(hq.IsPersonAuthorized(bob));
        }
    }
}
