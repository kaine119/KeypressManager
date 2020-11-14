using Database.DatabaseModels;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace DatabaseTest
{
    [TestClass, TestCategory("Models")]
    public class KeyListTest
    {
        [TestMethod]
        public void KeyList_InstantiatesCorrectly()
        {
            KeyList keyList = KeyList.All.Single(list => list.Name == "Test Keypress");
            Assert.AreEqual(2, keyList.ID);
            Assert.AreEqual(2, keyList.Keys.Count());
        }

        [TestMethod]
        public void Write_CreatesNewList()
        {
            KeyList keyList = new KeyList
            {
                Name = "New Keylist",
                Colour = "ff0000"
            };
            keyList.Write();
            Assert.AreEqual(4, KeyList.All.Count());
            Assert.AreEqual("New Keylist", KeyList.All.Last().Name);
        }

        [TestMethod]
        public void Write_EditsExistingList()
        {
            int initialAllKeyListsCount = KeyList.All.Count();
            KeyList keyList = KeyList.All.Single(list => list.Name == "Test Keypress");
            keyList.Name = "Test Keypress (edited)";
            keyList.Write();
            Assert.AreEqual(initialAllKeyListsCount, KeyList.All.Count());
            Assert.IsTrue(KeyList.All.Any(list => list.Name == "Test Keypress (edited)"));
        }
    }
}
