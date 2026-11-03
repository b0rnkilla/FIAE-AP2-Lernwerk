SET NOCOUNT ON;
SET XACT_ABORT ON;
SET LOCK_TIMEOUT 1000;
CREATE TABLE #Artikel (
    Id int NOT NULL PRIMARY KEY,
    Name nvarchar(60) NOT NULL,
    Kategorie nvarchar(30) NOT NULL,
    Preis decimal(10,2) NOT NULL CHECK (Preis >= 0),
    Bestand int NOT NULL,
    Aktiv bit NOT NULL
);
CREATE TABLE #Konten (
    Id int NOT NULL PRIMARY KEY,
    Name nvarchar(60) NOT NULL,
    Guthaben decimal(10,2) NOT NULL CHECK (Guthaben >= 0)
);
INSERT INTO #Artikel (Id,Name,Kategorie,Preis,Bestand,Aktiv) VALUES
(1,N'Temperatursensor',N'Sensoren',25.00,3,1),
(2,N'Drucksensor',N'Sensoren',40.00,12,1),
(3,N'Transportbox',N'Zubehör',12.50,0,1),
(4,N'Altadapter',N'Zubehör',5.00,1,0),
(5,N'Prüfgerät',N'Geräte',180.00,6,1);
INSERT INTO #Konten (Id,Name,Guthaben) VALUES
(1,N'Werkstatt',600.00),(2,N'Einkauf',150.00),(3,N'Reserve',100.00);
