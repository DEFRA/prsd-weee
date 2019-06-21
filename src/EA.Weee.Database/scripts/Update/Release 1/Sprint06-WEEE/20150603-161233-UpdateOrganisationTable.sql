

GO
PRINT N'Dropping FK_AspNetUsers_Organisation...';


GO
ALTER TABLE [Identity].[AspNetUsers] DROP CONSTRAINT [FK_AspNetUsers_Organisation];


GO
/*
The column [Organisation].[Organisation].[Address1] is being dropped, data loss could occur.

The column [Organisation].[Organisation].[Address2] is being dropped, data loss could occur.

The column [Organisation].[Organisation].[AditionalRegistrationNumber] is being dropped, data loss could occur.

The column [Organisation].[Organisation].[Building] is being dropped, data loss could occur.

The column [Organisation].[Organisation].[Country] is being dropped, data loss could occur.

The column [Organisation].[Organisation].[Email] is being dropped, data loss could occur.

The column [Organisation].[Organisation].[Fax] is being dropped, data loss could occur.

The column [Organisation].[Organisation].[FirstName] is being dropped, data loss could occur.

The column [Organisation].[Organisation].[LastName] is being dropped, data loss could occur.

The column [Organisation].[Organisation].[PostalCode] is being dropped, data loss could occur.

The column [Organisation].[Organisation].[Region] is being dropped, data loss could occur.

The column [Organisation].[Organisation].[RegistrationNumber] is being dropped, data loss could occur.

The column [Organisation].[Organisation].[Telephone] is being dropped, data loss could occur.

The column [Organisation].[Organisation].[TownOrCity] is being dropped, data loss could occur.
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
    [Type]                      NVARCHAR (64)    NOT NULL,
    [Status]                    NCHAR (10)       CONSTRAINT [DF_Organisation_Status] DEFAULT (N'Incomplete') NOT NULL,
    [Name]                      NVARCHAR (2048)  NOT NULL,
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
        INSERT INTO [Organisation].[tmp_ms_xx_Organisation] ([Id], [Name], [Type])
        SELECT   [Id],
                 [Name],
                 [Type]
        FROM     [Organisation].[Organisation]
        ORDER BY [Id] ASC;
    END

DROP TABLE [Organisation].[Organisation];

EXECUTE sp_rename N'[Organisation].[tmp_ms_xx_Organisation]', N'Organisation';

EXECUTE sp_rename N'[Organisation].[tmp_ms_xx_constraint_PK_Organisation_Id]', N'PK_Organisation_Id', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating FK_AspNetUsers_Organisation...';


GO
ALTER TABLE [Identity].[AspNetUsers] WITH NOCHECK
    ADD CONSTRAINT [FK_AspNetUsers_Organisation] FOREIGN KEY ([OrganisationId]) REFERENCES [Organisation].[Organisation] ([Id]);


GO
PRINT N'Creating FK_Organisation_BusinessAddress...';


GO
ALTER TABLE [Organisation].[Organisation] WITH NOCHECK
    ADD CONSTRAINT [FK_Organisation_BusinessAddress] FOREIGN KEY ([BusinessAddressId]) REFERENCES [Organisation].[Address] ([Id]);


GO
PRINT N'Creating FK_Organisation_NotificationAddress...';


GO
ALTER TABLE [Organisation].[Organisation] WITH NOCHECK
    ADD CONSTRAINT [FK_Organisation_NotificationAddress] FOREIGN KEY ([NotificationAddressId]) REFERENCES [Organisation].[Address] ([Id]);


GO
PRINT N'Creating FK_Organisation_OrganisationAddress...';


GO
ALTER TABLE [Organisation].[Organisation] WITH NOCHECK
    ADD CONSTRAINT [FK_Organisation_OrganisationAddress] FOREIGN KEY ([OrganisationAddressId]) REFERENCES [Organisation].[Address] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [Identity].[AspNetUsers] WITH CHECK CHECK CONSTRAINT [FK_AspNetUsers_Organisation];

ALTER TABLE [Organisation].[Organisation] WITH CHECK CHECK CONSTRAINT [FK_Organisation_BusinessAddress];

ALTER TABLE [Organisation].[Organisation] WITH CHECK CHECK CONSTRAINT [FK_Organisation_NotificationAddress];

ALTER TABLE [Organisation].[Organisation] WITH CHECK CHECK CONSTRAINT [FK_Organisation_OrganisationAddress];


GO
PRINT N'Update complete.';


GO
