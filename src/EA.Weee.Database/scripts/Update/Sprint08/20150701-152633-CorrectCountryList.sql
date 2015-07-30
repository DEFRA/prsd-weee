GO
PRINT N'Updating [Lookup].[Country]...';

UPDATE [Lookup].[Country] SET [Name] = 'Venezuela (Bolivarian Republic of)' WHERE [Name] = 'Venezuela (Bolivarian Repulic of';

GO
PRINT N'Update complete.';


GO