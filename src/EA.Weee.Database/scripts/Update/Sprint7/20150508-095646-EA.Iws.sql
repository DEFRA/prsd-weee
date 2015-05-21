

GO
PRINT N'Altering [Notification].[Notification]...';


GO
ALTER TABLE [Notification].[Notification]
    ADD [CreatedDate] DATETIME2 (0) CONSTRAINT [DF_Notification_CreatedDate] DEFAULT (getdate()) NOT NULL;


GO
PRINT N'Update complete.';


GO
