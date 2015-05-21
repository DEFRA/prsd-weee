

GO
PRINT N'Altering [Identity].[AspNetUsers]...';


GO
ALTER TABLE [Identity].[AspNetUsers]
    ADD [FirstName] NVARCHAR (256) NULL,
        [Surname]   NVARCHAR (256) NULL;


GO
PRINT N'Update complete.';


GO
