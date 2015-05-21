

GO
PRINT N'Altering [Identity].[AspNetUsers]...';


GO
ALTER TABLE [Identity].[AspNetUsers]
    ADD [OrganisationId] UNIQUEIDENTIFIER NULL;


GO
PRINT N'Creating FK_AspNetUsers_Organisation...';


GO
ALTER TABLE [Identity].[AspNetUsers] WITH NOCHECK
    ADD CONSTRAINT [FK_AspNetUsers_Organisation] FOREIGN KEY ([OrganisationId]) REFERENCES [Business].[Organisation] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [Identity].[AspNetUsers] WITH CHECK CHECK CONSTRAINT [FK_AspNetUsers_Organisation];


GO
PRINT N'Update complete.';


GO
