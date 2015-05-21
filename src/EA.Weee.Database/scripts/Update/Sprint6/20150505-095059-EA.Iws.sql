
GO
PRINT N'Altering [Business].[Organisation]...';


GO
ALTER TABLE [Business].[Organisation]
    ADD [CompaniesHouseNumber] NVARCHAR (64) NULL;


GO
PRINT N'Update complete.';


GO