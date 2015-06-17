GO
PRINT N'Altering [Lookup].[CompetentAuthority]...';


GO
ALTER TABLE [Lookup].[CompetentAuthority]
    DROP COLUMN [IsSystemUser];


GO
PRINT N'Update complete.';


GO
