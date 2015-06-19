

GO
PRINT N'Dropping FK_AspNetUsers_Organisation...';


GO
ALTER TABLE [Identity].[AspNetUsers] DROP CONSTRAINT [FK_AspNetUsers_Organisation];


GO
PRINT N'Altering [Identity].[AspNetUsers]...';


GO
ALTER TABLE [Identity].[AspNetUsers] DROP COLUMN [OrganisationId];


GO
PRINT N'Update complete.';


GO
