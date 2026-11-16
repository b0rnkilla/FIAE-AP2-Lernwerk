namespace FiaeTrainer.Core.Sql.Workshop;

/// <summary> Die drei Werkstattfälle teilen ihre Ausgangsdaten innerhalb eines Kapitels. </summary>
internal sealed record WorkshopScenario(string[] Objects, string Seed, string Extra)
{
    public static WorkshopScenario Get(string name) => name switch
    {
        "Orders" => new(["Artikel", "Bestellungen", "Positionen", "Rueckgaben"], """
            CREATE TABLE Artikel(Id int PRIMARY KEY, Name nvarchar(40) NOT NULL);
            CREATE TABLE Bestellungen(Id int PRIMARY KEY, Kunde nvarchar(40) NOT NULL);
            CREATE TABLE Rueckgaben(Id int PRIMARY KEY, BestellungId int NOT NULL, ArtikelId int NOT NULL);
            INSERT Artikel VALUES (1,N'Sensor'),(2,N'Kabel'),(3,N'Adapter');
            INSERT Bestellungen VALUES (10,N'Lea'),(20,N'Noah'),(30,N'Mira');
            INSERT Rueckgaben VALUES (100,10,1),(101,10,2),(102,20,1);
            """, "INSERT Artikel VALUES (4,N'Netzteil'); INSERT Bestellungen VALUES (40,N'Ali'); INSERT Rueckgaben VALUES (103,40,4);"),
        "Stock" => new(["Nord", "Sued", "Artikel", "Meldungen", "Archiv"], """
            CREATE TABLE Nord(ArtikelId int NOT NULL, Menge int NOT NULL);
            CREATE TABLE Sued(ArtikelId int NOT NULL, Menge int NOT NULL);
            CREATE TABLE Artikel(Id int PRIMARY KEY, Bestand int NOT NULL);
            CREATE TABLE Meldungen(ArtikelId int PRIMARY KEY, Gezaehlt int NULL);
            CREATE TABLE Archiv(ArtikelId int NOT NULL, Menge int NOT NULL);
            INSERT Nord VALUES (1,4),(2,3),(2,3),(3,0);
            INSERT Sued VALUES (1,4),(2,1),(4,5);
            INSERT Artikel VALUES (1,8),(2,5),(3,7),(4,9);
            INSERT Meldungen VALUES (1,6),(2,0),(3,NULL);
            """, "INSERT Nord VALUES (6,9),(7,2); INSERT Sued VALUES (6,9),(8,1); INSERT Artikel VALUES (6,2),(7,1),(8,9); INSERT Meldungen VALUES (6,NULL),(7,0);"),
        "Reception" => new(["Termine", "Tagesplan"], """
            CREATE TABLE Termine(Id int PRIMARY KEY, Titel nvarchar(60) NOT NULL, Intern nvarchar(60) NOT NULL, Bestaetigt bit NOT NULL);
            INSERT Termine VALUES (1,N'Lesung',N'Honorar',1),(2,N'Probe',N'Schlüssel',0),(3,N'Forum',N'Telefon',1);
            """, "INSERT Termine VALUES (4,N'Lesung',N'Vertrag',1),(5,N'Kurs',N'Material',0);"),
        _ => throw new InvalidDataException("Unbekannter Werkstattfall.")
    };

    /// <summary> Sicht für die Aufgaben zur Berechtigungsvergabe. </summary>
    public const string RECEPTION_VIEW = "CREATE VIEW Tagesplan AS SELECT Id,Titel FROM Termine WHERE Bestaetigt=1;";
}
