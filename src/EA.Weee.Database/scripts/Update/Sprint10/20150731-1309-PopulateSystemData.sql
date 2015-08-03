GO
PRINT N'Populating [dbo].[SystemData]...';

GO
INSERT INTO [dbo].[SystemData] (Id, LatestPRNSeed) VALUES (NEWID(), 0);

GO
PRINT N'Update complete.';

GO