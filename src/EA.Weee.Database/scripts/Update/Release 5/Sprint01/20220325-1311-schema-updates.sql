

ALTER TABLE [Evidence].[Note] ADD NoteType [INT] NOT NULL;
	
ALTER TABLE [Evidence].[NoteTonnage]  WITH CHECK ADD  CONSTRAINT [FK_Note_CategoryId] FOREIGN KEY([CategoryId])
REFERENCES [Lookup].[WeeeCategory] ([Id])
GO

ALTER TABLE [Evidence].[NoteTonnage]  CHECK CONSTRAINT [FK_Note_CategoryId]
GO

CREATE NONCLUSTERED INDEX [IDX_Note_CategoryId] ON  [Evidence].[NoteTonnage] 
(
	[CategoryId] ASC
)
GO

ALTER TABLE [Evidence].[NoteTonnage] DROP CONSTRAINT [FK_NoteTonnage_NoteId]
GO

ALTER TABLE [Evidence].[NoteTonnage]  WITH NOCHECK ADD  CONSTRAINT [FK_NoteTonnage_NoteId] FOREIGN KEY([NoteId])
REFERENCES [Evidence].[Note] ([Id])
GO

ALTER TABLE [Evidence].[NoteTonnage] CHECK CONSTRAINT [FK_NoteTonnage_NoteId]
GO
