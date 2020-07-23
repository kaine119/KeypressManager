CREATE TABLE KeyLists (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL
);

CREATE TABLE KeyBunches (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL,
  bunchNumber TEXT NOT NULL,
  keyListId INTEGER NOT NULL,
  numberOfKeys INTEGER NOT NULL,
  FOREIGN KEY (keyListId) REFERENCES KeyLists(id)
);

CREATE TABLE Personnel (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL,
  rank INTEGER NOT NULL,
  nric TEXT NOT NULL,
  contactNumber TEXT
);

CREATE TABLE Authorizations (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  keyBunchId INTEGER NOT NULL,
  personId INTEGER NOT NULL,
  UNIQUE (keyBunchId, personId),
  FOREIGN KEY (keyBunchId) REFERENCES KeyBunches(id),
  FOREIGN KEY (personId)   REFERENCES Personnel(id)
);

CREATE TABLE LogEntries (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  keyBunchDrawn INTEGER NOT NULL,
  timeIssued INTEGER NOT NULL,
  personDrawingKey INTEGER NOT NULL,
  personIssuingKey INTEGER NOT NULL, 
  timeReturned INTEGER NULL,
  personReturningKey INTEGER NULL,
  personReceivingKey INTEGER NULL,

  FOREIGN KEY (keyBunchDrawn) REFERENCES KeyBunches(id),
  FOREIGN KEY (personDrawingKey) REFERENCES Personnel(id),
  FOREIGN KEY (personIssuingKey) REFERENCES Personnel(id),
  FOREIGN KEY (personReturningKey) REFERENCES Personnel(id),
  FOREIGN KEY (personReceivingKey) REFERENCES Personnel(id)
);
