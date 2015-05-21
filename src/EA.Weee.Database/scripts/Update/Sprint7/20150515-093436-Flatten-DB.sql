

GO
PRINT N'Dropping FK_AspNetUsers_Organisation...';


GO
ALTER TABLE [Identity].[AspNetUsers] DROP CONSTRAINT [FK_AspNetUsers_Organisation];


GO
PRINT N'Dropping FK_Organisation_Country...';


GO
ALTER TABLE [Business].[Organisation] DROP CONSTRAINT [FK_Organisation_Country];


GO
PRINT N'Dropping FK_Producer_Contact...';


GO
ALTER TABLE [Business].[Producer] DROP CONSTRAINT [FK_Producer_Contact];


GO
PRINT N'Dropping FK_Producer_Address...';


GO
ALTER TABLE [Business].[Producer] DROP CONSTRAINT [FK_Producer_Address];


GO
PRINT N'Dropping FK_NotificationProducer_Producer...';


GO
ALTER TABLE [Business].[NotificationProducer] DROP CONSTRAINT [FK_NotificationProducer_Producer];


GO
PRINT N'Dropping FK_Notification_Exporter...';


GO
ALTER TABLE [Notification].[Notification] DROP CONSTRAINT [FK_Notification_Exporter];


GO
PRINT N'Dropping FK_Exporter_Address...';


GO
ALTER TABLE [Notification].[Exporter] DROP CONSTRAINT [FK_Exporter_Address];


GO
PRINT N'Dropping FK_Exporter_Contact...';


GO
ALTER TABLE [Notification].[Exporter] DROP CONSTRAINT [FK_Exporter_Contact];


GO
/*
The column [Business].[Organisation].[AddressId] is being dropped, data loss could occur.

The column [Business].[Organisation].[CountryId] is being dropped, data loss could occur.
*/
GO
PRINT N'Starting rebuilding table [Business].[Organisation]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [Business].[tmp_ms_xx_Organisation] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
    [Name]                        NVARCHAR (2048)  NOT NULL,
    [Type]                        NVARCHAR (64)    NOT NULL,
    [RowVersion]                  ROWVERSION       NOT NULL,
    [RegistrationNumber]          NVARCHAR (64)    NULL,
    [AditionalRegistrationNumber] NVARCHAR (64)    NULL,
    [Building]                    NVARCHAR (1024)  NULL,
    [Address1]                    NVARCHAR (1024)  NULL,
    [TownOrCity]                  NVARCHAR (1024)  NULL,
    [Address2]                    NVARCHAR (1024)  NULL,
    [PostalCode]                  NVARCHAR (64)    NULL,
    [Country]                     NVARCHAR (1024)  NULL,
    [FirstName]                   NVARCHAR (150)   NULL,
    [LastName]                    NVARCHAR (150)   NULL,
    [Telephone]                   NVARCHAR (150)   NULL,
    [Fax]                         NVARCHAR (150)   NULL,
    [Email]                       NVARCHAR (150)   NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_Organisation_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [Business].[Organisation])
    BEGIN
        INSERT INTO [Business].[tmp_ms_xx_Organisation] ([Id], [Name], [Type], [RegistrationNumber], [AditionalRegistrationNumber], [Building], [Address1], [TownOrCity], [Address2], [PostalCode], [FirstName], [LastName], [Telephone], [Fax], [Email])
        SELECT   [Id],
                 [Name],
                 [Type],
                 [RegistrationNumber],
                 [AditionalRegistrationNumber],
                 [Building],
                 [Address1],
                 [TownOrCity],
                 [Address2],
                 [PostalCode],
                 [FirstName],
                 [LastName],
                 [Telephone],
                 [Fax],
                 [Email]
        FROM     [Business].[Organisation]
        ORDER BY [Id] ASC;
    END

DROP TABLE [Business].[Organisation];

EXECUTE sp_rename N'[Business].[tmp_ms_xx_Organisation]', N'Organisation';

EXECUTE sp_rename N'[Business].[tmp_ms_xx_constraint_PK_Organisation_Id]', N'PK_Organisation_Id', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
/*
The column [Business].[Producer].[AddressId] is being dropped, data loss could occur.

The column [Business].[Producer].[ContactId] is being dropped, data loss could occur.

The column [Business].[Producer].[CountryId] is being dropped, data loss could occur.
*/
GO
PRINT N'Starting rebuilding table [Business].[Producer]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [Business].[tmp_ms_xx_Producer] (
    [Id]                           UNIQUEIDENTIFIER NOT NULL,
    [Name]                         NVARCHAR (100)   NOT NULL,
    [IsSiteOfExport]               BIT              NOT NULL,
    [Type]                         NVARCHAR (64)    NOT NULL,
    [RegistrationNumber]           NVARCHAR (64)    NULL,
    [AdditionalRegistrationNumber] NVARCHAR (64)    NULL,
    [Building]                     NVARCHAR (1024)  NULL,
    [Address1]                     NVARCHAR (1024)  NULL,
    [TownOrCity]                   NVARCHAR (1024)  NULL,
    [Address2]                     NVARCHAR (1024)  NULL,
    [PostalCode]                   NVARCHAR (64)    NULL,
    [Country]                      NVARCHAR (1024)  NULL,
    [FirstName]                    NVARCHAR (150)   NULL,
    [LastName]                     NVARCHAR (150)   NULL,
    [Telephone]                    NVARCHAR (150)   NULL,
    [Fax]                          NVARCHAR (150)   NULL,
    [Email]                        NVARCHAR (150)   NULL,
    [RowVersion]                   ROWVERSION       NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [Business].[Producer])
    BEGIN
        INSERT INTO [Business].[tmp_ms_xx_Producer] ([Id], [Name], [IsSiteOfExport], [Type], [RegistrationNumber], [AdditionalRegistrationNumber], [Building], [Address1], [TownOrCity], [Address2], [PostalCode], [FirstName], [LastName], [Telephone], [Fax], [Email])
        SELECT   [Id],
                 [Name],
                 [IsSiteOfExport],
                 [Type],
                 [RegistrationNumber],
                 [AdditionalRegistrationNumber],
                 [Building],
                 [Address1],
                 [TownOrCity],
                 [Address2],
                 [PostalCode],
                 [FirstName],
                 [LastName],
                 [Telephone],
                 [Fax],
                 [Email]
        FROM     [Business].[Producer]
        ORDER BY [Id] ASC;
    END

DROP TABLE [Business].[Producer];

EXECUTE sp_rename N'[Business].[tmp_ms_xx_Producer]', N'Producer';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
/*
The column [Notification].[Exporter].[AddressId] is being dropped, data loss could occur.

The column [Notification].[Exporter].[ContactId] is being dropped, data loss could occur.

The column [Notification].[Exporter].[CountryId] is being dropped, data loss could occur.
*/
GO
PRINT N'Starting rebuilding table [Notification].[Exporter]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [Notification].[tmp_ms_xx_Exporter] (
    [Id]                           UNIQUEIDENTIFIER NOT NULL,
    [Name]                         NVARCHAR (20)    NOT NULL,
    [Type]                         NVARCHAR (64)    NOT NULL,
    [RegistrationNumber]           NVARCHAR (64)    NULL,
    [AdditionalRegistrationNumber] NVARCHAR (64)    NULL,
    [Building]                     NVARCHAR (1024)  NULL,
    [Address1]                     NVARCHAR (1024)  NULL,
    [TownOrCity]                   NVARCHAR (1024)  NULL,
    [Address2]                     NVARCHAR (1024)  NULL,
    [PostalCode]                   NVARCHAR (64)    NULL,
    [Country]                      NVARCHAR (1024)  NULL,
    [FirstName]                    NVARCHAR (150)   NULL,
    [LastName]                     NVARCHAR (150)   NULL,
    [Telephone]                    NVARCHAR (150)   NULL,
    [Fax]                          NVARCHAR (150)   NULL,
    [Email]                        NVARCHAR (150)   NULL,
    [RowVersion]                   ROWVERSION       NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_Exporter] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [Notification].[Exporter])
    BEGIN
        INSERT INTO [Notification].[tmp_ms_xx_Exporter] ([Id], [Name], [Type], [RegistrationNumber], [AdditionalRegistrationNumber], [Building], [Address1], [TownOrCity], [Address2], [PostalCode], [FirstName], [LastName], [Telephone], [Fax], [Email])
        SELECT   [Id],
                 [Name],
                 [Type],
                 [RegistrationNumber],
                 [AdditionalRegistrationNumber],
                 [Building],
                 [Address1],
                 [TownOrCity],
                 [Address2],
                 [PostalCode],
                 [FirstName],
                 [LastName],
                 [Telephone],
                 [Fax],
                 [Email]
        FROM     [Notification].[Exporter]
        ORDER BY [Id] ASC;
    END

DROP TABLE [Notification].[Exporter];

EXECUTE sp_rename N'[Notification].[tmp_ms_xx_Exporter]', N'Exporter';

EXECUTE sp_rename N'[Notification].[tmp_ms_xx_constraint_PK_Exporter]', N'PK_Exporter', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating FK_AspNetUsers_Organisation...';


GO
ALTER TABLE [Identity].[AspNetUsers] WITH NOCHECK
    ADD CONSTRAINT [FK_AspNetUsers_Organisation] FOREIGN KEY ([OrganisationId]) REFERENCES [Business].[Organisation] ([Id]);


GO
PRINT N'Creating FK_NotificationProducer_Producer...';


GO
ALTER TABLE [Business].[NotificationProducer] WITH NOCHECK
    ADD CONSTRAINT [FK_NotificationProducer_Producer] FOREIGN KEY ([ProducerId]) REFERENCES [Business].[Producer] ([Id]);


GO
PRINT N'Creating FK_Notification_Exporter...';


GO
ALTER TABLE [Notification].[Notification] WITH NOCHECK
    ADD CONSTRAINT [FK_Notification_Exporter] FOREIGN KEY ([ExporterId]) REFERENCES [Notification].[Exporter] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [Identity].[AspNetUsers] WITH CHECK CHECK CONSTRAINT [FK_AspNetUsers_Organisation];

ALTER TABLE [Business].[NotificationProducer] WITH CHECK CHECK CONSTRAINT [FK_NotificationProducer_Producer];

ALTER TABLE [Notification].[Notification] WITH CHECK CHECK CONSTRAINT [FK_Notification_Exporter];


GO
PRINT N'Update complete.';


GO
