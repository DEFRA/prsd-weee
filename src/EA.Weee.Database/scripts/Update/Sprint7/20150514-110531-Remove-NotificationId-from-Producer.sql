

GO
PRINT N'Dropping FK_Producer_Notification...';


GO
ALTER TABLE [Business].[Producer] DROP CONSTRAINT [FK_Producer_Notification];


GO
PRINT N'Altering [Business].[Producer]...';


GO
ALTER TABLE [Business].[Producer] DROP COLUMN [NotificationId];


GO
PRINT N'Update complete.';


GO
