using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Database.DatabaseModels;
using Database;
using System.Linq;
using System.Collections.Generic;

namespace DatabaseTest
{
    [TestClass]
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
        public void IsValid_ReturnsCorrectly()
        {
            // Requirements: Rank, Name, NRIC
            // Rank missing
            Assert.IsFalse(
                new Person
                {
                    Name = "Bennett",
                    NRIC = "100A"
                }.IsValid);

            // Name missing
            Assert.IsFalse(new Person
            {
                Rank = Rank.PTE,
                NRIC = "100A"
            }.IsValid);

            // NRIC missing
            Assert.IsFalse(new Person
            {
                Name = "Bennett",
                Rank = Rank.PTE
            }.IsValid);

            // nothing missing, should be valid
            Assert.IsTrue(new Person
            {
                Name = "Bennett",
                Rank = Rank.PTE,
                NRIC = "100A"
            }.IsValid);
        }

        [TestMethod]
        public void Write_WritesCorrectly()
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
    }
}
