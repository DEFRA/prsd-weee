
GO
PRINT N'Altering [Organisation].[Organisation]...';


GO
ALTER TABLE [Organisation].[Organisation] ALTER COLUMN [CompanyRegistrationNumber] NVARCHAR (15) NULL;


GO
PRINT N'Update complete.';


GO
