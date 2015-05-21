

GO
PRINT N'Dropping FK_Organisation_Country...';


GO
ALTER TABLE [Business].[Organisation] DROP CONSTRAINT [FK_Organisation_Country];


GO
PRINT N'Dropping FK_Address_Country...';


GO
ALTER TABLE [Business].[Address] DROP CONSTRAINT [FK_Address_Country];


GO
/*
The column [Business].[Address].[Region] is being dropped, data loss could occur.

The column [Business].[Address].[StreetOrSuburb] is being dropped, data loss could occur.
*/
GO
PRINT N'Starting rebuilding table [Business].[Address]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [Business].[tmp_ms_xx_Address] (
    [Id]         UNIQUEIDENTIFIER NOT NULL,
    [Building]   NVARCHAR (1024)  NOT NULL,
    [Address1]   NVARCHAR (1024)  NULL,
    [TownOrCity] NVARCHAR (1024)  NOT NULL,
    [Address2]   NVARCHAR (1024)  NULL,
    [PostalCode] NVARCHAR (64)    NOT NULL,
    [CountryId]  UNIQUEIDENTIFIER NOT NULL,
    [RowVersion] ROWVERSION       NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_Address_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [Business].[Address])
    BEGIN
        INSERT INTO [Business].[tmp_ms_xx_Address] ([Id], [Building], [TownOrCity], [PostalCode], [CountryId])
        SELECT   [Id],
                 [Building],
                 [TownOrCity],
                 [PostalCode],
                 [CountryId]
        FROM     [Business].[Address]
        ORDER BY [Id] ASC;
    END

DROP TABLE [Business].[Address];

EXECUTE sp_rename N'[Business].[tmp_ms_xx_Address]', N'Address';

EXECUTE sp_rename N'[Business].[tmp_ms_xx_constraint_PK_Address_Id]', N'PK_Address_Id', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating FK_Organisation_Country...';


GO
ALTER TABLE [Business].[Organisation] WITH NOCHECK
    ADD CONSTRAINT [FK_Organisation_Country] FOREIGN KEY ([AddressId]) REFERENCES [Business].[Address] ([Id]);


GO
PRINT N'Creating FK_Address_Country...';


GO
ALTER TABLE [Business].[Address] WITH NOCHECK
    ADD CONSTRAINT [FK_Address_Country] FOREIGN KEY ([CountryId]) REFERENCES [Lookup].[Country] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [Business].[Organisation] WITH CHECK CHECK CONSTRAINT [FK_Organisation_Country];

ALTER TABLE [Business].[Address] WITH CHECK CHECK CONSTRAINT [FK_Address_Country];


GO
PRINT N'Update complete.';


GO
