

GO
PRINT N'Creating [Business].[Contact]...';


GO
CREATE TABLE [Business].[Contact] (
    [Id]         UNIQUEIDENTIFIER NOT NULL,
    [FirstName]  NVARCHAR (150)   NOT NULL,
    [LastName]   NVARCHAR (150)   NOT NULL,
    [Telephone]  NVARCHAR (150)   NOT NULL,
    [Fax]        NVARCHAR (150)   NULL,
    [Email]      NVARCHAR (150)   NOT NULL,
    [RowVersion] ROWVERSION       NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating FK_Producer_Contact...';


GO
ALTER TABLE [Business].[Producer] WITH NOCHECK
    ADD CONSTRAINT [FK_Producer_Contact] FOREIGN KEY ([ContactId]) REFERENCES [Business].[Contact] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [Business].[Producer] WITH CHECK CHECK CONSTRAINT [FK_Producer_Contact];


GO
PRINT N'Update complete.';


GO
