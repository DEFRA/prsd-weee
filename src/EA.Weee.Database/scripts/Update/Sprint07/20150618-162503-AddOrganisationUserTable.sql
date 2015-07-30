

GO
PRINT N'Creating [Organisation].[OrganisationUser]...';


GO
CREATE TABLE [Organisation].[OrganisationUser] (
    [Id]                     UNIQUEIDENTIFIER NOT NULL,
    [UserId]                 NVARCHAR (128)   NOT NULL,
    [OrganisationId]         UNIQUEIDENTIFIER NOT NULL,
    [OrganisationUserStatus] INT              NOT NULL,
    [RowVersion]             ROWVERSION       NOT NULL,
    CONSTRAINT [PK_OrganisationUser] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating FK_OrganisationUser_Organisation...';


GO
ALTER TABLE [Organisation].[OrganisationUser] WITH NOCHECK
    ADD CONSTRAINT [FK_OrganisationUser_Organisation] FOREIGN KEY ([OrganisationId]) REFERENCES [Organisation].[Organisation] ([Id]);


GO
PRINT N'Creating FK_OrganisationUser_AspNetUsers...';


GO
ALTER TABLE [Organisation].[OrganisationUser] WITH NOCHECK
    ADD CONSTRAINT [FK_OrganisationUser_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [Identity].[AspNetUsers] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [Organisation].[OrganisationUser] WITH CHECK CHECK CONSTRAINT [FK_OrganisationUser_Organisation];

ALTER TABLE [Organisation].[OrganisationUser] WITH CHECK CHECK CONSTRAINT [FK_OrganisationUser_AspNetUsers];


GO
PRINT N'Update complete.';


GO
