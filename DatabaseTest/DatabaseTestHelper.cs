using Microsoft.VisualStudio.TestTools.UnitTesting;
using Database;
using System.Reflection;

namespace DatabaseTest
{
    /// <summary>
    /// Test helper for Database tests.
    /// Contains static members needed for database tests.
    /// </summary>
    [TestClass]
    public class DatabaseTestHelper
    {
        [AssemblyInitialize]
        public static void InitializeDatabase(TestContext testContext)
        {
            var _ = new KeypressDatabase($"{System.IO.Path.GetDirectoryName(Assembly.GetCallingAssembly().Location)}/TestFixtures/test.sqlite3");
        }
    }
}
