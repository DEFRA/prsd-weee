

GO
PRINT N'Creating [Organisation].[Contact]...';


GO
CREATE TABLE [Organisation].[Contact] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [FirstName] NVARCHAR (64)    NOT NULL,
    [LastName]  NVARCHAR (64)    NOT NULL,
    [Position]  NVARCHAR (128)   NOT NULL,
    CONSTRAINT [PK_Contact_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating FK_Organisation_Contact...';


GO
ALTER TABLE [Organisation].[Organisation] WITH NOCHECK
    ADD CONSTRAINT [FK_Organisation_Contact] FOREIGN KEY ([ContactId]) REFERENCES [Organisation].[Contact] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [Organisation].[Organisation] WITH CHECK CHECK CONSTRAINT [FK_Organisation_Contact];


GO
PRINT N'Update complete.';


GO
