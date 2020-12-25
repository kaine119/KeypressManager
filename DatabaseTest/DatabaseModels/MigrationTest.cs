using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DatabaseTest.DatabaseModels
{
    [TestClass]
    public class MigrationTest
    {
        [TestMethod]
        public void DoesItWork()
        {
            Database.Migration.ApplyMigrations(null);
        }
    }
}
