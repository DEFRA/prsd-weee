

GO
PRINT N'Dropping FK_Notification_Exporter...';


GO
ALTER TABLE [Notification].[Notification] DROP CONSTRAINT [FK_Notification_Exporter];


GO
PRINT N'Dropping FK_Exporter_Contact...';


GO
ALTER TABLE [Notification].[Exporter] DROP CONSTRAINT [FK_Exporter_Contact];


GO
PRINT N'Dropping FK_Exporter_Address...';


GO
ALTER TABLE [Notification].[Exporter] DROP CONSTRAINT [FK_Exporter_Address];


GO
/*
The column [Notification].[Exporter].[CompanyHouseNumber] is being dropped, data loss could occur.
*/
GO
PRINT N'Starting rebuilding table [Notification].[Exporter]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [Notification].[tmp_ms_xx_Exporter] (
    [Id]                   UNIQUEIDENTIFIER NOT NULL,
    [Name]                 NVARCHAR (20)    NOT NULL,
    [Type]                 NVARCHAR (64)    NOT NULL,
    [RegistrationNumber1]  NVARCHAR (64)    NULL,
    [RegistrationNumber2]  NVARCHAR (64)    NULL,
    [CompaniesHouseNumber] NVARCHAR (64)    NULL,
    [AddressId]            UNIQUEIDENTIFIER NULL,
    [ContactId]            UNIQUEIDENTIFIER NULL,
    [RowVersion]           ROWVERSION       NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_Exporter] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [Notification].[Exporter])
    BEGIN
        INSERT INTO [Notification].[tmp_ms_xx_Exporter] ([Id], [Name], [Type], [RegistrationNumber1], [RegistrationNumber2], [AddressId], [ContactId])
        SELECT   [Id],
                 [Name],
                 [Type],
                 [RegistrationNumber1],
                 [RegistrationNumber2],
                 [AddressId],
                 [ContactId]
        FROM     [Notification].[Exporter]
        ORDER BY [Id] ASC;
    END

DROP TABLE [Notification].[Exporter];

EXECUTE sp_rename N'[Notification].[tmp_ms_xx_Exporter]', N'Exporter';

EXECUTE sp_rename N'[Notification].[tmp_ms_xx_constraint_PK_Exporter]', N'PK_Exporter', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating FK_Notification_Exporter...';


GO
ALTER TABLE [Notification].[Notification] WITH NOCHECK
    ADD CONSTRAINT [FK_Notification_Exporter] FOREIGN KEY ([ExporterId]) REFERENCES [Notification].[Exporter] ([Id]);


GO
PRINT N'Creating FK_Exporter_Contact...';


GO
ALTER TABLE [Notification].[Exporter] WITH NOCHECK
    ADD CONSTRAINT [FK_Exporter_Contact] FOREIGN KEY ([ContactId]) REFERENCES [Business].[Contact] ([Id]);


GO
PRINT N'Creating FK_Exporter_Address...';


GO
ALTER TABLE [Notification].[Exporter] WITH NOCHECK
    ADD CONSTRAINT [FK_Exporter_Address] FOREIGN KEY ([AddressId]) REFERENCES [Business].[Address] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [Notification].[Notification] WITH CHECK CHECK CONSTRAINT [FK_Notification_Exporter];

ALTER TABLE [Notification].[Exporter] WITH CHECK CHECK CONSTRAINT [FK_Exporter_Contact];

ALTER TABLE [Notification].[Exporter] WITH CHECK CHECK CONSTRAINT [FK_Exporter_Address];


GO
PRINT N'Update complete.';


GO
