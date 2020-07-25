using Database;
using Database.DatabaseModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseTest
{
    [TestClass]
    public class LogEntryTest
    {
        KeypressDatabase db = DatabaseTestHelper.TestDatabase;

        [TestMethod]
        public void IsKeyReturned_CorrectlyReturns()
        {
            LogEntry unreturnedKey = new LogEntry
            {
                TimeIssued = DateTime.Now,
                PersonDrawingKey = new Person { Name = "Drawer", ContactNumber = "90123456", NRIC = "100A", Rank = Rank.PTE },
                PersonIssuingKey = new Person { Name = "Issuer", ContactNumber = "90123456", NRIC = "200B", Rank = Rank.CFC }
            };
            Assert.IsFalse(unreturnedKey.IsKeyReturned);

            LogEntry returnedKey = new LogEntry
            {
                TimeIssued = DateTime.Now,
                PersonDrawingKey = new Person { Name = "Drawer", ContactNumber = "90123456", NRIC = "100A", Rank = Rank.PTE },
                PersonIssuingKey = new Person { Name = "Issuer", ContactNumber = "90123456", NRIC = "200B", Rank = Rank.CFC },
                TimeReturned = DateTime.Now.AddMinutes(10),
                PersonReturningKey = new Person { Name = "Drawer", ContactNumber = "90123456", NRIC = "100A", Rank = Rank.PTE },
                PersonReceivingKey = new Person { Name = "Issuer", ContactNumber = "90123456", NRIC = "200B", Rank = Rank.CFC }
            };
            Assert.IsTrue(returnedKey.IsKeyReturned);
        }

        [TestMethod]
        public void LogEntry_InstantiatesCorrectly()
        {
            LogEntry firstLog = db.AllLogEntries.First();
            Assert.AreEqual(new DateTime(2020, 4, 1, 12, 0, 0), firstLog.TimeIssued);
            Assert.AreEqual(new DateTime(2020, 4, 1, 13, 0, 0), firstLog.TimeReturned);
            Assert.AreEqual("Alice Tan", firstLog.PersonDrawingKey.Name);
            Assert.AreEqual("Charlie Chan", firstLog.PersonIssuingKey.Name);
            Assert.AreEqual("Alice Tan", firstLog.PersonReturningKey.Name);
            Assert.AreEqual("Charlie Chan", firstLog.PersonReceivingKey.Name);

            LogEntry unreturnedLog = db.AllLogEntries.ElementAt(1);
            Assert.IsFalse(unreturnedLog.IsKeyReturned);
        }

        [TestMethod]
        public void Write_WritesCorrectlyForExistingPersonnel()
        {
            Person customer = db.AllPersonnel.First();
            Person staff = db.AllPersonnel.Last();
            LogEntry log = new LogEntry
            {
                KeyBunchDrawn = db.AllKeyBunches.First(),
                TimeIssued = new DateTimeOffset(2020, 4, 1, 14, 0, 0, TimeSpan.FromHours(8)),
                PersonDrawingKey = customer,
                PersonIssuingKey = staff,
                TimeReturned = new DateTimeOffset(2020, 4, 1, 16, 0, 0, TimeSpan.FromHours(8)),
                PersonReturningKey = customer,
                PersonReceivingKey = staff
            };
            log.Write();

            Console.WriteLine($"Object timestamp: {log.TimeIssued}");
            Console.WriteLine($"Written timestamp: {log.TimeIssued?.ToUnixTimeSeconds()}");

            LogEntry writtenLog = db.AllLogEntries.Last();
            Console.WriteLine($"Read timestamp: {writtenLog.TimeIssued}");
            Assert.AreEqual(log.TimeIssued, writtenLog.TimeIssued.Value);
        }
    }
}
