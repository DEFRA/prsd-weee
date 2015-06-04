

GO
PRINT N'Dropping DF_Organisation_Status...';


GO
ALTER TABLE [Organisation].[Organisation] DROP CONSTRAINT [DF_Organisation_Status];


GO
PRINT N'Dropping FK_Organisation_Contact...';


GO
ALTER TABLE [Organisation].[Organisation] DROP CONSTRAINT [FK_Organisation_Contact];


GO
PRINT N'Dropping FK_AspNetUsers_Organisation...';


GO
ALTER TABLE [Identity].[AspNetUsers] DROP CONSTRAINT [FK_AspNetUsers_Organisation];


GO
PRINT N'Dropping FK_Organisation_OrganisationAddress...';


GO
ALTER TABLE [Organisation].[Organisation] DROP CONSTRAINT [FK_Organisation_OrganisationAddress];


GO
PRINT N'Dropping FK_Organisation_NotificationAddress...';


GO
ALTER TABLE [Organisation].[Organisation] DROP CONSTRAINT [FK_Organisation_NotificationAddress];


GO
PRINT N'Dropping FK_Organisation_BusinessAddress...';


GO
ALTER TABLE [Organisation].[Organisation] DROP CONSTRAINT [FK_Organisation_BusinessAddress];


GO
/*
The column [Organisation].[Organisation].[Status] is being dropped, data loss could occur.

The column [Organisation].[Organisation].[Type] is being dropped, data loss could occur.

The column [Organisation].[Organisation].[OrganisationStatus] on table [Organisation].[Organisation] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.

The column [Organisation].[Organisation].[OrganisationType] on table [Organisation].[Organisation] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.
*/
GO
PRINT N'Starting rebuilding table [Organisation].[Organisation]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [Organisation].[tmp_ms_xx_Organisation] (
    [Id]                        UNIQUEIDENTIFIER NOT NULL,
    [RowVersion]                ROWVERSION       NOT NULL,
    [Name]                      NVARCHAR (2048)  NOT NULL,
    [OrganisationType]          INT              NOT NULL,
    [OrganisationStatus]        INT              NOT NULL,
    [TradingName]               NVARCHAR (2048)  NULL,
    [CompanyRegistrationNumber] NVARCHAR (64)    NULL,
    [ContactId]                 UNIQUEIDENTIFIER NULL,
    [OrganisationAddressId]     UNIQUEIDENTIFIER NULL,
    [BusinessAddressId]         UNIQUEIDENTIFIER NULL,
    [NotificationAddressId]     UNIQUEIDENTIFIER NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_Organisation_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [Organisation].[Organisation])
    BEGIN
        INSERT INTO [Organisation].[tmp_ms_xx_Organisation] ([Id], [Name], [TradingName], [CompanyRegistrationNumber], [ContactId], [OrganisationAddressId], [BusinessAddressId], [NotificationAddressId])
        SELECT   [Id],
                 [Name],
                 [TradingName],
                 [CompanyRegistrationNumber],
                 [ContactId],
                 [OrganisationAddressId],
                 [BusinessAddressId],
                 [NotificationAddressId]
        FROM     [Organisation].[Organisation]
        ORDER BY [Id] ASC;
    END

DROP TABLE [Organisation].[Organisation];

EXECUTE sp_rename N'[Organisation].[tmp_ms_xx_Organisation]', N'Organisation';

EXECUTE sp_rename N'[Organisation].[tmp_ms_xx_constraint_PK_Organisation_Id]', N'PK_Organisation_Id', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating FK_Organisation_Contact...';


GO
ALTER TABLE [Organisation].[Organisation] WITH NOCHECK
    ADD CONSTRAINT [FK_Organisation_Contact] FOREIGN KEY ([ContactId]) REFERENCES [Organisation].[Contact] ([Id]);


GO
PRINT N'Creating FK_AspNetUsers_Organisation...';


GO
ALTER TABLE [Identity].[AspNetUsers] WITH NOCHECK
    ADD CONSTRAINT [FK_AspNetUsers_Organisation] FOREIGN KEY ([OrganisationId]) REFERENCES [Organisation].[Organisation] ([Id]);


GO
PRINT N'Creating FK_Organisation_OrganisationAddress...';


GO
ALTER TABLE [Organisation].[Organisation] WITH NOCHECK
    ADD CONSTRAINT [FK_Organisation_OrganisationAddress] FOREIGN KEY ([OrganisationAddressId]) REFERENCES [Organisation].[Address] ([Id]);


GO
PRINT N'Creating FK_Organisation_NotificationAddress...';


GO
ALTER TABLE [Organisation].[Organisation] WITH NOCHECK
    ADD CONSTRAINT [FK_Organisation_NotificationAddress] FOREIGN KEY ([NotificationAddressId]) REFERENCES [Organisation].[Address] ([Id]);


GO
PRINT N'Creating FK_Organisation_BusinessAddress...';


GO
ALTER TABLE [Organisation].[Organisation] WITH NOCHECK
    ADD CONSTRAINT [FK_Organisation_BusinessAddress] FOREIGN KEY ([BusinessAddressId]) REFERENCES [Organisation].[Address] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [Organisation].[Organisation] WITH CHECK CHECK CONSTRAINT [FK_Organisation_Contact];

ALTER TABLE [Identity].[AspNetUsers] WITH CHECK CHECK CONSTRAINT [FK_AspNetUsers_Organisation];

ALTER TABLE [Organisation].[Organisation] WITH CHECK CHECK CONSTRAINT [FK_Organisation_OrganisationAddress];

ALTER TABLE [Organisation].[Organisation] WITH CHECK CHECK CONSTRAINT [FK_Organisation_NotificationAddress];

ALTER TABLE [Organisation].[Organisation] WITH CHECK CHECK CONSTRAINT [FK_Organisation_BusinessAddress];


GO
PRINT N'Update complete.';


GO
