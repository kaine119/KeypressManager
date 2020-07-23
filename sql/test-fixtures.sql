-- Dummy fixtures for testing --
INSERT INTO KeyLists (id, name) 
  VALUES (1, "Main Keypress"),
         (2, "Test Keypress");

INSERT INTO KeyBunches (id, name, bunchNumber, keyListId, numberOfKeys)
  VALUES (1, "Mess", "01", 1, 4),
         (2, "Office", "02", 1, 16),
         (3, "HQ", "B01", 1, 20),
         (4, "Edit target", "B11", 1, 25),
         (5, "Pristine 1", "01", 2, 10),
         (6, "Pristine 2", "01", 2, 10);

INSERT INTO Personnel (id, nric, name, rank, contactNumber)
  VALUES (1, "101A", "Alice Tan", 4, "90123456"),
         (2, "102B", "Bob Lee", 5, "90123456"),
         (3, "103C", "Charlie Chan", 6, "90123456");

INSERT INTO Authorizations (keyBunchId, personId)
  VALUES (1, 1), (2, 2), (3, 3);

INSERT INTO LogEntries (timeIssued, keyBunchDrawn, personDrawingKey, personIssuingKey, timeReturned, personReturningKey, personReceivingKey)
  VALUES (1585742400, 1, 1, 3, 1585756800, 1, 3),
         (1585742410, 1, 2, 3, NULL, NULL, NULL);
