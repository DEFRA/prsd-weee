ALTER TABLE [Organisation].[AdditionalCompanyDetails] ADD [ORDER] INT NULL;
GO
UPDATE [Organisation].[AdditionalCompanyDetails] SET [ORDER] = 0;
ALTER TABLE [Organisation].[AdditionalCompanyDetails] ALTER COLUMN [ORDER] INT NOT NULL;
