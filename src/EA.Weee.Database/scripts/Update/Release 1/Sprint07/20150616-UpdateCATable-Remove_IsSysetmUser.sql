GO
PRINT N'Altering [Lookup].[CompetentAuthority]...';

GO
ALTER TABLE [Lookup].[CompetentAuthority] DROP CONSTRAINT [DF_CompetentAuthority_IsSystemUser]

GO
ALTER TABLE [Lookup].[CompetentAuthority]
    DROP COLUMN [IsSystemUser];
	
GO
PRINT N'Update complete.';
GO

