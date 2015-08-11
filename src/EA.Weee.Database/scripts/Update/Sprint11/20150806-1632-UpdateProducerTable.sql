PRINT N'Altering [Producer].[Producer]...';
GO

EXEC sp_rename '[Producer].[Producer].LastSubmitted', 'UpdatedDate', 'COLUMN';
GO

GO    
PRINT N'Update complete.';

GO