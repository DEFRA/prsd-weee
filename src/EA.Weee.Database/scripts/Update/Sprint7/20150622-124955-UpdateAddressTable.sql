GO
PRINT N'Dropping FK_Organisation_BusinessAddress...';


GO
ALTER TABLE [Organisation].[Organisation] DROP CONSTRAINT [FK_Organisation_BusinessAddress];


GO

GO
PRINT N'Dropping FK_Organisation_NotificationAddress...';


GO
ALTER TABLE [Organisation].[Organisation] DROP CONSTRAINT [FK_Organisation_NotificationAddress];


GO

GO
PRINT N'Dropping FK_Organisation_OrganisationAddress...';


GO
ALTER TABLE [Organisation].[Organisation] DROP CONSTRAINT [FK_Organisation_OrganisationAddress];


GO

GO
PRINT N'Starting rebuilding table [Organisation].[Address]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [Organisation].[tmp_ms_xx_Address] (
    [Id]				UNIQUEIDENTIFIER NOT NULL,
    [Address1]			NVARCHAR (35)  NOT NULL,
    [Address2]			NVARCHAR (35)  NULL,
    [TownOrCity]		NVARCHAR (35)  NOT NULL,
    [CountyOrRegion]	NVARCHAR (35)  NULL,
    [Postcode]		    NVARCHAR (10)  NULL,
	[CountryId]         UNIQUEIDENTIFIER NOT NULL,
    [Telephone]			NVARCHAR (20)	  NOT NULL,
	[Email]				NVARCHAR (256)	  NOT NULL,
	[RowVersion]		ROWVERSION       NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_Address_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [Organisation].[Address])
    BEGIN
        INSERT INTO [Organisation].[tmp_ms_xx_Address] ([Id], [Address1], [Address2], [TownOrCity], [CountyOrRegion], [Postcode], [CountryId], [Telephone], [Email])
        SELECT   [Id],
				[Address1],
				[Address2],
				[TownOrCity],
				[CountyOrRegion],
				[Postcode],
				(SELECT Id From [Lookup].[Country] WHERE Name LIKE '%'+[Country]+'%'),
				[Telephone],
				[Email]
        FROM     [Organisation].[Address]
        ORDER BY [Id] ASC;
    END
DROP TABLE [Organisation].[Address];

EXECUTE sp_rename N'[Organisation].[tmp_ms_xx_Address]', N'Address';

EXECUTE sp_rename N'[Organisation].[tmp_ms_xx_constraint_PK_Address_Id]', N'PK_Address_Id', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
GO

ALTER TABLE [Organisation].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_Country] FOREIGN KEY([CountryId])
REFERENCES [Lookup].[Country] ([Id])
GO

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

ALTER TABLE [Organisation].[Organisation] WITH CHECK CHECK CONSTRAINT [FK_Organisation_BusinessAddress];

ALTER TABLE [Organisation].[Organisation] WITH CHECK CHECK CONSTRAINT [FK_Organisation_NotificationAddress];

ALTER TABLE [Organisation].[Organisation] WITH CHECK CHECK CONSTRAINT [FK_Organisation_OrganisationAddress];


GO
PRINT N'Update complete.';


GO
