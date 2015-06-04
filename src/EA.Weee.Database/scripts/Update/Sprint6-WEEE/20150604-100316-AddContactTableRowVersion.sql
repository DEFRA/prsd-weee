

GO
PRINT N'Altering [Organisation].[Contact]...';


GO
ALTER TABLE [Organisation].[Contact]
    ADD [RowVersion] ROWVERSION NOT NULL;


GO
PRINT N'Update complete.';


GO
