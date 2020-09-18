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

CREATE TABLE Squadrons (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL
);

CREATE TABLE Personnel (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL,
  rank INTEGER NOT NULL,
  nric TEXT NOT NULL,
  contactNumber TEXT
);

CREATE TABLE PersonnelSquadrons (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  personId INTEGER,
  squadronId INTEGER,
  unique (personId, squadronId),
  FOREIGN KEY (personId) REFERENCES Personnel(id),
  FOREIGN KEY (squadronId) REFERENCES Squadrons(id)
);

CREATE TABLE Staff (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  personId INTEGER NOT NULL,
  FOREIGN KEY (personId) REFERENCES Personnel(id)
);

CREATE TABLE Authorizations (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  keyBunchId INTEGER NOT NULL,
  personId INTEGER NOT NULL,
  UNIQUE (keyBunchId, personId),
  FOREIGN KEY (keyBunchId) REFERENCES KeyBunches(id),
  FOREIGN KEY (personId)   REFERENCES Personnel(id)
);

CREATE TABLE SquadronAuthorizations (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  keyBunchId INTEGER NOT NULL,
  squadronId INTEGER NOT NULL,
  UNIQUE (keyBunchId, squadronId),
  FOREIGN KEY (keyBunchId) REFERENCES KeyBunches(id),
  FOREIGN KEY (squadronId) REFERENCES Squadrons(id)
);

CREATE TABLE LogEntries (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  keyBunchDrawnId INTEGER NOT NULL,
  timeIssued INTEGER NOT NULL,
  personDrawingKeyId INTEGER NOT NULL,
  personIssuingKeyId INTEGER NOT NULL, 
  timeReturned INTEGER NULL,
  personReturningKeyId INTEGER NULL,
  personReceivingKeyId INTEGER NULL,

  FOREIGN KEY (keyBunchDrawnId) REFERENCES KeyBunches(id),
  FOREIGN KEY (personDrawingKeyId) REFERENCES Personnel(id),
  FOREIGN KEY (personIssuingKeyId) REFERENCES Personnel(id),
  FOREIGN KEY (personReturningKeyId) REFERENCES Personnel(id),
  FOREIGN KEY (personReceivingKeyId) REFERENCES Personnel(id)
);
