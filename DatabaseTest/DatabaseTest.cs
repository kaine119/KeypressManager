using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Database;
using Database.DatabaseModels;
using System.Reflection;
using System.Linq;

namespace DatabaseTest
{
    [TestClass]
    public class DatabaseTest
    {
        KeypressDatabase TestDatabase = new KeypressDatabase($"{System.IO.Path.GetDirectoryName(Assembly.GetCallingAssembly().Location)}/TestFixtures/test.sqlite3");

        [TestMethod]
        public void FirstTest()
        {
            Person firstPersonnel = TestDatabase.AllPersonnel.First();
            Assert.AreEqual("Mess", firstPersonnel.GetAuthorizedKeys().First().Name);
        }
    }
}
