

GO
PRINT N'Altering [Organisation].[Address]...';


GO
ALTER TABLE [Organisation].[Address] ALTER COLUMN [Postcode] NVARCHAR (10) NULL;


GO
PRINT N'Update complete.';


GO
