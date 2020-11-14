using Database.DatabaseModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DatabaseTest
{
    [TestClass, TestCategory("Models")]
    public class SquadronTest
    {
        [TestMethod]
        public void Squadron_InstantiatesCorrectly()
        {
            Squadron sqn = Squadron.All.First();
            Person bob = Person.All.ElementAt(1);
            Assert.AreEqual(bob, sqn.Personnel.First());
        }

        [TestMethod]
        public void Write_WritesCorrectly()
        {
            Person newPerson = new Person { Name = "NewSquadronTest", Rank = Rank.PTE, NRIC = "101A", ContactNumber = "90123456" };
            Person bob = Person.All.Single(person => person.Name == "Bob Lee");
            Squadron sqn = new Squadron { Name = "TEST SQN" };
            sqn.Personnel.Add(newPerson);
            sqn.Personnel.Add(bob);
            sqn.Write();

            Squadron written = Squadron.All.Single(sqn => sqn.Name == "TEST SQN");
            Assert.IsTrue(written.Personnel.Contains(bob));
            Assert.IsTrue(written.Personnel.Contains(newPerson));
            Assert.IsNotNull(written.Personnel.Single(person => person.Equals(newPerson)).ID);
        }
    }
}
