CREATE TABLE [Evidence].[NoteTransferCategory](
	[Id] [uniqueidentifier] NOT NULL,
	[TransferNoteId] [uniqueidentifier] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_NoteTransferCategory_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UNQ_NoteTransferCategory_Category] UNIQUE NONCLUSTERED 
(
	[TransferNoteId] ASC,
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [Evidence].[NoteTransferCategory]  WITH NOCHECK ADD  CONSTRAINT [FK_NoteTransferCategory_TransferNoteId] FOREIGN KEY([TransferNoteId])
REFERENCES [Evidence].[Note] ([Id])
GO

ALTER TABLE [Evidence].[NoteTransferCategory] CHECK CONSTRAINT [FK_NoteTransferCategory_TransferNoteId]
GO