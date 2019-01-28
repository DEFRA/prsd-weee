

GO
PRINT N'Altering [PCS].[MemberUpload]...';


GO
ALTER TABLE [PCS].[MemberUpload] ALTER COLUMN [ComplianceYear] INT NULL;

GO
PRINT N'Update complete.';


GO