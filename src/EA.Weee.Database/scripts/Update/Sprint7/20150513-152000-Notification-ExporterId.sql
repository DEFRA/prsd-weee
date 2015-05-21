ALTER TABLE [Notification].[Notification]
ADD [ExporterId] UNIQUEIDENTIFIER NULL CONSTRAINT [FK_Notification_Exporter] 
FOREIGN KEY REFERENCES [Notification].[Exporter] ([Id])
GO