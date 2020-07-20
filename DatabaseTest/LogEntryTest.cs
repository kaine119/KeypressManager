using Database.DatabaseTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DatabaseTest
{
    [TestClass]
    public class LogEntryTest
    {
        [TestMethod]
        public void IsKeyReturned_CorrectlyReturns()
        {
            LogEntry unreturnedKey = new LogEntry
            {
                DateTimeIssued = DateTime.Now,
                PersonDrawingKey = new Person { Name = "Drawer", ContactNumber = "90123456", NRIC = "100A", Rank = Rank.PTE },
                PersonIssuingKey = new Person { Name = "Issuer", ContactNumber = "90123456", NRIC = "200B", Rank = Rank.CFC }
            };
            Assert.IsFalse(unreturnedKey.IsKeyReturned);

            LogEntry returnedKey = new LogEntry
            {
                DateTimeIssued = DateTime.Now,
                PersonDrawingKey = new Person { Name = "Drawer", ContactNumber = "90123456", NRIC = "100A", Rank = Rank.PTE },
                PersonIssuingKey = new Person { Name = "Issuer", ContactNumber = "90123456", NRIC = "200B", Rank = Rank.CFC },
                DateTimeReturned = DateTime.Now.AddMinutes(10),
                PersonReturningKey = new Person { Name = "Drawer", ContactNumber = "90123456", NRIC = "100A", Rank = Rank.PTE },
                PersonReceivingKey = new Person { Name = "Issuer", ContactNumber = "90123456", NRIC = "200B", Rank = Rank.CFC }
            };
            Assert.IsTrue(returnedKey.IsKeyReturned);
        }



    }
}
