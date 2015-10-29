PRINT N'Altering [Producer].[Producer]...';
GO
ALTER TABLE [Producer].[Producer]
  ALTER COLUMN AnnualTurnover [decimal](18, 8) NOT NULL 

GO    
PRINT N'Update complete.';

GO

