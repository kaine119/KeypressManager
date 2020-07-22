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
    /// <summary>
    /// Test helper for Database tests.
    /// Contains static members needed for database tests.
    /// </summary>
    public class DatabaseTestHelper
    {
        /// <summary>
        /// The test database.
        /// This is copied fresh from TestFixtures/test.sqlite3 every time the project is built.
        /// </summary>
        public static KeypressDatabase TestDatabase
            = new KeypressDatabase($"{System.IO.Path.GetDirectoryName(Assembly.GetCallingAssembly().Location)}/TestFixtures/test.sqlite3");
    }
}
