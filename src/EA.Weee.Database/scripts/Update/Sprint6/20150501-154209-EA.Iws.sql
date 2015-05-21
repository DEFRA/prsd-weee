

GO
PRINT N'Creating [Notification].[Notification]...';


GO
CREATE TABLE [Notification].[Notification] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [UserId]             UNIQUEIDENTIFIER NOT NULL,
    [WasteAction]        INT              NOT NULL,
    [CompetentAuthority] INT              NOT NULL,
    [NotificationNumber] NVARCHAR (50)    NOT NULL,
    [RowVersion]         ROWVERSION       NOT NULL,
    CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Update complete.';


GO
