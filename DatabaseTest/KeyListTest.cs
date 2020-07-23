using Database;
using Database.DatabaseModels;
using DatabaseTest;
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace DatabaseTest
{
    [TestClass]
    public class KeyListTest
    {
        KeypressDatabase db = DatabaseTestHelper.TestDatabase;

        [TestMethod]
        public void KeyList_InstantiatesCorrectly()
        {
            KeyList keyList = db.AllKeyLists.Single(list => list.Name == "Test Keypress");
            Assert.AreEqual(2, keyList.ID);
            Assert.AreEqual(2, keyList.Keys.Count());
        }

        [TestMethod]
        public void Write_CreatesNewList()
        {
            KeyList keyList = new KeyList
            {
                Name = "New Keylist"
            };
            keyList.Write();
            Assert.AreEqual(3, db.AllKeyLists.Count());
            Assert.AreEqual("New Keylist", db.AllKeyLists.Last().Name);
        }

        [TestMethod]
        public void Write_EditsExistingList()
        {
            int initialAllKeyListsCount = db.AllKeyLists.Count();
            KeyList keyList = db.AllKeyLists.Single(list => list.Name == "Test Keypress");
            keyList.Name = "Test Keypress (edited)";
            keyList.Write();
            Assert.AreEqual(initialAllKeyListsCount, db.AllKeyLists.Count());
            Assert.IsTrue(db.AllKeyLists.Any(list => list.Name == "Test Keypress (edited)"));
        }
    }
}
