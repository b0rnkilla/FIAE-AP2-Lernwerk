-- Nur die vom Lernwerk markierte Trainingsdatenbank erhält dieses leere Arbeitsschema.
IF NOT EXISTS (SELECT 1 FROM sys.extended_properties WHERE class=0 AND name=N'TrainerIdentity' AND CONVERT(nvarchar(100),value)=N'FIAE-AP2-Trainer-v1')
    THROW 51001,N'Trainingsdatenbank fehlt',1;
IF EXISTS (SELECT 1 FROM sys.extended_properties WHERE class=0 AND name=N'WerkstattSchemaVersion' AND CONVERT(int,value)=1)
    RETURN;
IF EXISTS (SELECT 1 FROM sys.extended_properties WHERE class=0 AND name=N'WerkstattSchemaVersion')
    THROW 51003,N'Unbekannte Werkstattversion',1;
SET XACT_ABORT ON;
BEGIN TRANSACTION;
DECLARE @lock int;
EXEC @lock=sys.sp_getapplock @Resource=N'AP2-Werkstattinstallation', @LockMode='Exclusive', @LockTimeout=6000;
IF @lock<0 THROW 51000,N'Werkstatt belegt',1;
IF NOT EXISTS (SELECT 1 FROM sys.extended_properties WHERE class=0 AND name=N'WerkstattSchemaVersion')
BEGIN
    IF SCHEMA_ID(N'LernwerkProbe') IS NOT NULL OR DATABASE_PRINCIPAL_ID(N'LernwerkWerkautor') IS NOT NULL OR DATABASE_PRINCIPAL_ID(N'LernwerkRezeption') IS NOT NULL OR DATABASE_PRINCIPAL_ID(N'LernwerkEmpfangRolle') IS NOT NULL
        THROW 51002,N'Namenskonflikt',1;
    CREATE USER LernwerkWerkautor WITHOUT LOGIN WITH DEFAULT_SCHEMA=LernwerkProbe;
    CREATE USER LernwerkRezeption WITHOUT LOGIN WITH DEFAULT_SCHEMA=LernwerkProbe;
    CREATE ROLE LernwerkEmpfangRolle;
    ALTER ROLE LernwerkEmpfangRolle ADD MEMBER LernwerkRezeption;
    GRANT CREATE TABLE, CREATE VIEW TO LernwerkWerkautor;
    GRANT IMPERSONATE ON USER::LernwerkRezeption TO LernwerkWerkautor;
    EXEC(N'CREATE SCHEMA LernwerkProbe AUTHORIZATION LernwerkWerkautor;');
    EXEC sys.sp_addextendedproperty @name=N'WerkstattSchemaVersion', @value=1;
END;
COMMIT;
