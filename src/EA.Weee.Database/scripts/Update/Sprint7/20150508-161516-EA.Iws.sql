GO
PRINT N'Creating [Business].[Producer]...';


GO
CREATE TABLE [Business].[Producer] (
    [Id]                   UNIQUEIDENTIFIER NOT NULL,
    [Name]                 NVARCHAR (100)   NOT NULL,
    [IsSiteOfExport]       BIT              NOT NULL,
    [Type]                 NVARCHAR (64)    NOT NULL,
    [CompaniesHouseNumber] NVARCHAR (64)    NULL,
    [RegNumber1]           NVARCHAR (64)    NULL,
    [RegNumber2]           NVARCHAR (64)    NULL,
    [NotificationId]       UNIQUEIDENTIFIER NOT NULL,
    [AddressId]            UNIQUEIDENTIFIER NOT NULL,
    [ContactId]            UNIQUEIDENTIFIER NOT NULL,
    [RowVersion]           ROWVERSION       NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating FK_Producer_Notification...';


GO
ALTER TABLE [Business].[Producer] WITH NOCHECK
    ADD CONSTRAINT [FK_Producer_Notification] FOREIGN KEY ([NotificationId]) REFERENCES [Notification].[Notification] ([Id]);


GO
PRINT N'Creating FK_Producer_Address...';


GO
ALTER TABLE [Business].[Producer] WITH NOCHECK
    ADD CONSTRAINT [FK_Producer_Address] FOREIGN KEY ([AddressId]) REFERENCES [Business].[Address] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [Business].[Producer] WITH CHECK CHECK CONSTRAINT [FK_Producer_Notification];

ALTER TABLE [Business].[Producer] WITH CHECK CHECK CONSTRAINT [FK_Producer_Address];


GO
PRINT N'Update complete.';


GO
