ALTER TABLE [Organisation].[Organisation] DROP CONSTRAINT [FK_Organisation_Contact];


GO
PRINT N'Starting rebuilding table [Organisation].[Contact]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [Organisation].[tmp_ms_xx_Contact] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [FirstName] NVARCHAR (35)    NOT NULL,
    [LastName]  NVARCHAR (35)    NOT NULL,
    [Position]  NVARCHAR (35)   NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_Contact_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [Organisation].[Contact])
    BEGIN
        INSERT INTO [Organisation].[tmp_ms_xx_Contact] ([Id], [FirstName], [LastName],[Position])
        SELECT   [Id],
                 [FirstName],
                 [LastName],
				 [Position]
        FROM     [Organisation].[Contact]
        ORDER BY [Id] ASC;
    END

DROP TABLE [Organisation].[Contact];

EXECUTE sp_rename N'[Organisation].[tmp_ms_xx_Contact]', N'Contact';

EXECUTE sp_rename N'[Organisation].[tmp_ms_xx_constraint_PK_Contact_Id]', N'PK_Contact_Id', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

PRINT N'Creating FK_Organisation_OrganisationAddress...';


GO
ALTER TABLE [Organisation].[Organisation] WITH NOCHECK
    ADD CONSTRAINT [FK_Organisation_Contact] FOREIGN KEY ([ContactId]) REFERENCES [Organisation].[Contact] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO
ALTER TABLE [Organisation].[Organisation] WITH CHECK CHECK CONSTRAINT [FK_Organisation_Contact];
GO
PRINT N'Update complete.';


GO