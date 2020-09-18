using Database;
using Database.DatabaseModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace DatabaseTest
{
    [TestClass, TestCategory("Models")]
    public class SquadronTest
    {
        KeypressDatabase db = DatabaseTestHelper.TestDatabase;

        [TestMethod]
        public void Squadron_InstantiatesCorrectly()
        {
            Squadron sqn = db.AllSquadrons.First();
            Person bob = db.AllPersonnel.ElementAt(1);
            Assert.AreEqual(bob, sqn.Personnel.First());
        }

        [TestMethod]
        public void Write_WritesCorrectly()
        {
            Person newPerson = new Person { Name = "NewSquadronTest", Rank = Rank.PTE, NRIC = "101A", ContactNumber = "90123456" };
            Person bob = db.AllPersonnel.Single(person => person.Name == "Bob Lee");
            Squadron sqn = new Squadron { Name = "TEST SQN" };
            sqn.Personnel.Add(newPerson);
            sqn.Personnel.Add(bob);
            sqn.Write();

            Squadron written = db.AllSquadrons.Single(sqn => sqn.Name == "TEST SQN");
            Assert.IsTrue(written.Personnel.Contains(bob));
            Assert.IsTrue(written.Personnel.Contains(newPerson));
            Assert.IsNotNull(written.Personnel.Single(person => person.Equals(newPerson)).ID);
        }
    }
}
