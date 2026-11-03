SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.TrainerVersion', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.TrainerVersion WHERE Version = 1)
        THROW 50000, N'Die Version der Trainingsdatenbank wird nicht unterstützt.', 1;
    COMMIT;
    RETURN;
END;

EXEC(N'CREATE SCHEMA fall');
EXEC(N'CREATE SCHEMA probe');
CREATE TABLE dbo.TrainerVersion (Version int NOT NULL PRIMARY KEY);
CREATE TABLE fall.Standorte (Id int NOT NULL PRIMARY KEY, Name nvarchar(80) NOT NULL, Ort nvarchar(80) NOT NULL);
CREATE TABLE fall.Mitarbeiter (Id int NOT NULL PRIMARY KEY, Vorname nvarchar(50) NOT NULL, Nachname nvarchar(50) NOT NULL, Abteilung nvarchar(50) NOT NULL, StandortId int NOT NULL REFERENCES fall.Standorte(Id));
CREATE TABLE fall.Fahrzeuge (Id int NOT NULL PRIMARY KEY, Kennzeichen nvarchar(20) NOT NULL UNIQUE, Typ nvarchar(30) NOT NULL);
CREATE TABLE fall.Sendungen (Id int NOT NULL PRIMARY KEY, Bezeichnung nvarchar(100) NOT NULL, Zielort nvarchar(80) NOT NULL, Wert decimal(12,2) NOT NULL CHECK (Wert >= 0), Status nvarchar(30) NOT NULL, Versanddatum date NOT NULL, StandortId int NOT NULL REFERENCES fall.Standorte(Id), FahrzeugId int NULL REFERENCES fall.Fahrzeuge(Id));
CREATE TABLE fall.Zutritte (Id int NOT NULL PRIMARY KEY, MitarbeiterId int NOT NULL REFERENCES fall.Mitarbeiter(Id), StandortId int NOT NULL REFERENCES fall.Standorte(Id), Zeitpunkt datetime2(0) NOT NULL);
CREATE TABLE fall.Fahrten (Id int NOT NULL PRIMARY KEY, FahrzeugId int NOT NULL REFERENCES fall.Fahrzeuge(Id), MitarbeiterId int NOT NULL REFERENCES fall.Mitarbeiter(Id), Abfahrt datetime2(0) NOT NULL, Rueckkehr datetime2(0) NULL, Zielort nvarchar(80) NOT NULL);

INSERT fall.Standorte VALUES (1,N'Nordlager',N'Dortmund'),(2,N'Südlager',N'Essen'),(3,N'Zentrale',N'Dortmund');
INSERT fall.Mitarbeiter VALUES
(1,N'Lea',N'Winter',N'Logistik',1),(2,N'Tim',N'Berger',N'Fahrdienst',1),(3,N'Mira',N'Koch',N'IT',3),
(4,N'Nora',N'Seidel',N'Logistik',1),(5,N'Jonas',N'Wolf',N'Fahrdienst',2),(6,N'Emil',N'Braun',N'Sicherheit',1),
(7,N'Aylin',N'Demir',N'IT',3),(8,N'Paul',N'Neumann',N'Logistik',2),(9,N'Eva',N'Sommer',N'Sicherheit',2);
INSERT fall.Fahrzeuge VALUES (1,N'DO-LN 101',N'Transporter'),(2,N'DO-LN 202',N'Transporter'),(3,N'E-LN 303',N'Lkw'),(4,N'DO-LN 404',N'Pkw');
INSERT fall.Sendungen VALUES
(101,N'Laborkoffer',N'Köln',12000,N'Offen','20261012',1,1),
(102,N'Kühlmodule',N'Essen',8500,N'Geliefert','20261012',1,2),
(103,N'Verbandmaterial',N'Bonn',2000,N'Geliefert','20261011',2,3),
(104,N'Medizinische Sensoren',N'Köln',48000,N'Verschwunden','20261012',1,2),
(105,N'Prüfgeräte',N'Dortmund',18000,N'Offen','20261012',1,1),
(106,N'Werkzeugkoffer',N'Essen',4000,N'Verschwunden','20261011',2,3),
(107,N'Leere Transportboxen',N'Dortmund',500,N'Storniert','20261012',2,NULL),
(108,N'Monitore',N'Dortmund',8500,N'Geliefert','20261012',1,2),
(109,N'Analysegeräte',N'Essen',30000,N'Offen','20261013',2,3),
(110,N'Ersatzsensoren',N'Bonn',15000,N'Geliefert','20261013',1,1),
(111,N'Probenbehälter',N'Köln',7500,N'Offen','20261013',2,NULL),
(112,N'Schutzkleidung',N'Essen',8000,N'Geliefert','20261013',2,3);
INSERT fall.Zutritte VALUES
(1,1,1,'20261012 08:00'),(2,1,1,'20261012 12:00'),(3,1,1,'20261012 17:00'),
(4,2,1,'20261012 18:00'),(5,3,3,'20261012 09:00'),
(6,4,1,'20261012 08:30'),(7,4,1,'20261012 12:30'),(8,4,1,'20261012 18:30'),
(9,4,1,'20261012 20:10'),(10,4,1,'20261012 20:40'),(11,4,1,'20261012 21:00'),(12,4,1,'20261012 21:10'),(13,4,1,'20261012 21:15'),
(14,5,2,'20261012 10:00'),(15,6,1,'20261012 19:00'),(16,6,1,'20261012 20:00'),(17,6,1,'20261012 22:00'),(18,9,2,'20261012 20:20');
INSERT fall.Fahrten VALUES
(1,1,2,'20261012 18:10','20261012 20:30',N'Köln'),
(2,2,4,'20261012 21:20',NULL,N'Privatdepot'),
(3,3,5,'20261012 10:10','20261012 14:00',N'Essen'),
(4,1,1,'20261012 17:10','20261012 17:50',N'Dortmund'),
(5,2,2,'20261011 08:00','20261011 11:00',N'Essen'),
(6,3,5,'20261013 08:00',NULL,N'Bonn');

-- Zweiter Datenbestand: zusätzliche Treffer, fehlende Beziehungen und gleiche Werte verhindern Zufallserfolge.
SELECT * INTO probe.Standorte FROM fall.Standorte;
SELECT * INTO probe.Mitarbeiter FROM fall.Mitarbeiter;
SELECT * INTO probe.Fahrzeuge FROM fall.Fahrzeuge;
SELECT * INTO probe.Sendungen FROM fall.Sendungen;
SELECT * INTO probe.Zutritte FROM fall.Zutritte;
SELECT * INTO probe.Fahrten FROM fall.Fahrten;
INSERT probe.Mitarbeiter VALUES (10,N'Sibel',N'Stark',N'Logistik',1),(11,N'Ben',N'Adler',N'IT',3);
INSERT probe.Fahrzeuge VALUES (5,N'DO-LN 505',N'Transporter'),(6,N'DO-LN 606',N'Pkw');
INSERT probe.Sendungen VALUES
(113,N'Zusatzsensoren',N'Köln',53000,N'Verschwunden','20261012',1,5),
(114,N'Prüfboxen',N'Essen',8500,N'Offen','20261012',2,NULL),
(115,N'Frachtkisten',N'Bonn',1000,N'Storniert','20261013',2,5),
(116,N'Testmonitore',N'Dortmund',20000,N'Geliefert','20261012',1,5);
INSERT probe.Zutritte VALUES (19,10,1,'20261012 20:30'),(20,10,1,'20261012 20:45'),(21,10,1,'20261012 21:00'),(22,10,1,'20261012 21:10');
INSERT probe.Fahrten VALUES (7,5,10,'20261012 21:30',NULL,N'Privatdepot');

CREATE USER TrainerReader WITHOUT LOGIN WITH DEFAULT_SCHEMA = fall;
GRANT SELECT ON SCHEMA::fall TO TrainerReader;
GRANT SELECT ON SCHEMA::probe TO TrainerReader;
INSERT dbo.TrainerVersion VALUES (1);
COMMIT;
