using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Database.DatabaseModels;
using Database;
using System.Linq;
using System.Collections.Generic;

namespace DatabaseTest
{
    [TestClass, TestCategory("Models")]
    public class PersonTest
    {
        KeypressDatabase db = DatabaseTestHelper.TestDatabase;

        [TestMethod]
        public void Person_InstantiatesCorrectly()
        {
            Person alice = db.AllPersonnel.First();
            Assert.AreEqual(1, alice.ID);
            Assert.AreEqual("Alice Tan", alice.Name);
            Assert.AreEqual("101A", alice.NRIC);
            Assert.AreEqual(Rank.REC, alice.Rank);
            Assert.AreEqual("90123456", alice.ContactNumber);
        }

        [TestMethod]
        public void Write_CreatesNewPersonCorrectly()
        {
            Person test = new Person
            {
                Name = "PersonWrite",
                NRIC = "105A",
                Rank = Rank.REC
            };
            test.Write();
            Assert.IsTrue(db.AllPersonnel.Any(arg => arg.Name == "PersonWrite"));
        }

        [TestMethod]
        public void Write_EditsExistingPersonCorrectly()
        {
            Person test = db.AllPersonnel.Where(person => person.Name == "PersonWrite").Single();
            test.NRIC = "106A";
            test.Write();
            Person testAfterWrite = db.AllPersonnel.Where(person => person.Name == "PersonWrite").Single();
            Assert.AreEqual("106A", testAfterWrite.NRIC);
        }
    }
}
