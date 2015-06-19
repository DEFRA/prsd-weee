GO
PRINT N'Altering [Lookup].[CompetentAuthority]...';

GO
ALTER TABLE [Lookup].[CompetentAuthority]
    DROP COLUMN [Region],[RowVersion];
	
GO
ALTER TABLE [Lookup].[CompetentAuthority]
    ADD [CountryId] UNIQUEIDENTIFIER NOT NULL;

ALTER TABLE [Lookup].[CompetentAuthority] WITH NOCHECK
    ADD CONSTRAINT [FK_CompetentAuthority_Country] FOREIGN KEY ([CountryId]) REFERENCES [Lookup].[Country] ([Id]);
GO
PRINT N'Update complete.';
GO
