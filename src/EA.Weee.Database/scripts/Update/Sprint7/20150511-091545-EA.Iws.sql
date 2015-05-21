

GO
PRINT N'Dropping FK_Producer_Address...';


GO
ALTER TABLE [Business].[Producer] DROP CONSTRAINT [FK_Producer_Address];


GO
PRINT N'Dropping FK_Producer_Notification...';


GO
ALTER TABLE [Business].[Producer] DROP CONSTRAINT [FK_Producer_Notification];


GO
PRINT N'Dropping FK_Producer_Contact...';


GO
ALTER TABLE [Business].[Producer] DROP CONSTRAINT [FK_Producer_Contact];


GO
/*
The column [Business].[Producer].[RegNumber1] is being dropped, data loss could occur.

The column [Business].[Producer].[RegNumber2] is being dropped, data loss could occur.
*/
GO
PRINT N'Starting rebuilding table [Business].[Producer]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [Business].[tmp_ms_xx_Producer] (
    [Id]                   UNIQUEIDENTIFIER NOT NULL,
    [Name]                 NVARCHAR (100)   NOT NULL,
    [IsSiteOfExport]       BIT              NOT NULL,
    [Type]                 NVARCHAR (64)    NOT NULL,
    [CompaniesHouseNumber] NVARCHAR (64)    NULL,
    [RegistrationNumber1]  NVARCHAR (64)    NULL,
    [RegistrationNumber2]  NVARCHAR (64)    NULL,
    [NotificationId]       UNIQUEIDENTIFIER NOT NULL,
    [AddressId]            UNIQUEIDENTIFIER NOT NULL,
    [ContactId]            UNIQUEIDENTIFIER NOT NULL,
    [RowVersion]           ROWVERSION       NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [Business].[Producer])
    BEGIN
        INSERT INTO [Business].[tmp_ms_xx_Producer] ([Id], [Name], [IsSiteOfExport], [Type], [CompaniesHouseNumber], [NotificationId], [AddressId], [ContactId])
        SELECT   [Id],
                 [Name],
                 [IsSiteOfExport],
                 [Type],
                 [CompaniesHouseNumber],
                 [NotificationId],
                 [AddressId],
                 [ContactId]
        FROM     [Business].[Producer]
        ORDER BY [Id] ASC;
    END

DROP TABLE [Business].[Producer];

EXECUTE sp_rename N'[Business].[tmp_ms_xx_Producer]', N'Producer';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating FK_Producer_Address...';


GO
ALTER TABLE [Business].[Producer] WITH NOCHECK
    ADD CONSTRAINT [FK_Producer_Address] FOREIGN KEY ([AddressId]) REFERENCES [Business].[Address] ([Id]);


GO
PRINT N'Creating FK_Producer_Notification...';


GO
ALTER TABLE [Business].[Producer] WITH NOCHECK
    ADD CONSTRAINT [FK_Producer_Notification] FOREIGN KEY ([NotificationId]) REFERENCES [Notification].[Notification] ([Id]);


GO
PRINT N'Creating FK_Producer_Contact...';


GO
ALTER TABLE [Business].[Producer] WITH NOCHECK
    ADD CONSTRAINT [FK_Producer_Contact] FOREIGN KEY ([ContactId]) REFERENCES [Business].[Contact] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [Business].[Producer] WITH CHECK CHECK CONSTRAINT [FK_Producer_Address];

ALTER TABLE [Business].[Producer] WITH CHECK CHECK CONSTRAINT [FK_Producer_Notification];

ALTER TABLE [Business].[Producer] WITH CHECK CHECK CONSTRAINT [FK_Producer_Contact];


GO
PRINT N'Update complete.';


GO
