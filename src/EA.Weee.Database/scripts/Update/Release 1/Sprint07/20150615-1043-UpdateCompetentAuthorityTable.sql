GO
PRINT N'Altering [Lookup].[CompetentAuthority]...';


GO
ALTER TABLE [Lookup].[CompetentAuthority]
    ADD [Region] NVARCHAR (2048) NOT NULL;


GO
PRINT N'Update complete.';


GO
