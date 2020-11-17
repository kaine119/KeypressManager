using Database.DatabaseModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseTest
{
    [TestClass, TestCategory("Models")]
    public class PersonTest
    {
        [TestMethod]
        public void Person_InstantiatesCorrectly()
        {
            Person alice = Person.All.First();
            Assert.AreEqual(1, alice.ID);
            Assert.AreEqual("Alice Tan", alice.Name);
            Assert.AreEqual("101A", alice.NRIC);
            Assert.AreEqual(Rank.REC, alice.Rank);
            Assert.AreEqual("90123456", alice.ContactNumber);

            Person bob = Person.All.ElementAt(1);
            Assert.AreEqual("111 SQN", bob.Squadrons.First().Name);
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
            Assert.IsTrue(Person.All.Any(arg => arg.Name == "PersonWrite"));
        }

        [TestMethod]
        public void Write_EditsExistingPersonCorrectly()
        {
            Person test = Person.All.Where(person => person.Name == "PersonWrite").Single();
            test.NRIC = "106A";
            test.Write();
            Person testAfterWrite = Person.All.Where(person => person.Name == "PersonWrite").Single();
            Assert.AreEqual("106A", testAfterWrite.NRIC);
        }

        [TestMethod]
        public void GetStaff_GetsCorrectly()
        {
            IEnumerable<Person> staff = Person.AllStaff;
            Person charlie = staff.First();
            Assert.AreEqual("Charlie Chan", charlie.Name);
            Assert.AreEqual("103C", charlie.NRIC);
        }

        [TestMethod]
        public void WriteStaff_ReplacesCorrectly()
        {
            Person dennis = Person.AllStaff.ElementAt(1);
            List<Person> staffToWrite = new List<Person> { dennis };
            Person.WriteStaff(staffToWrite);
            Assert.AreEqual(1, Person.AllStaff.Count());
            Assert.AreEqual(dennis, Person.AllStaff.Single());
        }
    }
}
