

GO
PRINT N'Altering [Notification].[Notification]...';


GO
ALTER TABLE [Notification].[Notification]
    ADD [ImporterId] UNIQUEIDENTIFIER NULL;


GO
PRINT N'Creating [Business].[Importer]...';


GO
CREATE TABLE [Business].[Importer] (
    [Id]                           UNIQUEIDENTIFIER NOT NULL,
    [Name]                         NVARCHAR (100)   NOT NULL,
    [Type]                         NVARCHAR (64)    NOT NULL,
    [RegistrationNumber]           NVARCHAR (64)    NOT NULL,
    [AdditionalRegistrationNumber] NVARCHAR (64)    NULL,
    [Building]                     NVARCHAR (1024)  NOT NULL,
    [Address1]                     NVARCHAR (1024)  NOT NULL,
    [Address2]                     NVARCHAR (1024)  NULL,
    [TownOrCity]                   NVARCHAR (1024)  NOT NULL,
    [PostalCode]                   NVARCHAR (64)    NOT NULL,
    [Country]                      NVARCHAR (1024)  NOT NULL,
    [FirstName]                    NVARCHAR (150)   NOT NULL,
    [LastName]                     NVARCHAR (150)   NOT NULL,
    [Telephone]                    NVARCHAR (150)   NOT NULL,
    [Fax]                          NVARCHAR (150)   NULL,
    [Email]                        NVARCHAR (150)   NOT NULL,
    [RowVersion]                   ROWVERSION       NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating FK_Notification_Importer...';


GO
ALTER TABLE [Notification].[Notification] WITH NOCHECK
    ADD CONSTRAINT [FK_Notification_Importer] FOREIGN KEY ([ImporterId]) REFERENCES [Business].[Importer] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [Notification].[Notification] WITH CHECK CHECK CONSTRAINT [FK_Notification_Importer];


GO
PRINT N'Update complete.';


GO
