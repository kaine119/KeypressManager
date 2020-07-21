using Database.DatabaseModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DatabaseTest
{
    [TestClass]
    public class KeyBunchTest
    {
        private KeyBunch TestKeyBunch;
        private List<Person> TestAuthorizedPersonnel = new List<Person> {
          new Person { NRIC = "001A", Name = "Alice Tan", Rank = Rank.PTE, ContactNumber = "90123456" },
          new Person { NRIC = "002B", Name = "Bob Lee", Rank = Rank.PTE, ContactNumber = "90123456" },
          new Person { NRIC = "003C", Name = "Charlie Chan", Rank = Rank.PTE, ContactNumber = "90123456" },
        };

        [TestMethod]
        public void FindPersonnel_CorrectlyReturnsPersonnel()
        {
            TestKeyBunch = new KeyBunch
            {
                AuthorizedPersonnel = TestAuthorizedPersonnel
            };
            
            List<Person> queryForEveryone = TestKeyBunch.FindPersonnel("00"); // everyone on the list has an NRIC starting with 00
            Assert.AreEqual(queryForEveryone.Count, 3);

            List<Person> queryForAlice = TestKeyBunch.FindPersonnel("Alice");
            Assert.AreEqual(1, queryForAlice.Count);
            Assert.AreEqual("Alice Tan", queryForAlice[0].Name);
        }
    }
}
