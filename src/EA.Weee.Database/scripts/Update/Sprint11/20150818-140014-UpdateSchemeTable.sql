

GO
PRINT N'Altering [PCS].[Scheme]...';


GO
ALTER TABLE [PCS].[Scheme] ALTER COLUMN [ApprovalNumber] NVARCHAR (16) NULL;

ALTER TABLE [PCS].[Scheme] ALTER COLUMN [SchemeName] NVARCHAR (70) NULL;


GO
PRINT N'Update complete.';


GO
