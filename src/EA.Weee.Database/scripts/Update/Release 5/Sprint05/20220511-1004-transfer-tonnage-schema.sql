
CREATE TABLE [Evidence].[NoteTransferTonnage](
	[Id] [uniqueidentifier] NOT NULL,
	[TransferNoteId] [uniqueidentifier] NOT NULL,
	[NoteTonnageId] [uniqueidentifier] NOT NULL,
	[Received] [decimal](28, 3) NOT NULL,
	[Reused] [decimal](28, 3) NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_NoteTransferTonnage_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UNQ_TransferNote_NoteTonnage] UNIQUE NONCLUSTERED 
(
	[NoteTonnageId] ASC,
	[TransferNoteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [Evidence].[NoteTransferTonnage]  WITH NOCHECK ADD  CONSTRAINT [FK_TransferTonnage_TransferNoteId] FOREIGN KEY([TransferNoteId])
REFERENCES [Evidence].[Note] ([Id])
GO

ALTER TABLE [Evidence].[NoteTransferTonnage] CHECK CONSTRAINT [FK_TransferTonnage_TransferNoteId]
GO


ALTER TABLE [Evidence].[NoteTransferTonnage]  WITH NOCHECK ADD  CONSTRAINT [FK_TransferTonnage_NoteTonnageId] FOREIGN KEY([NoteTonnageId])
REFERENCES [Evidence].[NoteTonnage] ([Id])
GO

ALTER TABLE [Evidence].[NoteTransferTonnage] CHECK CONSTRAINT [FK_TransferTonnage_TransferNoteId]
GO


CREATE NONCLUSTERED INDEX [IDX_NoteTransferTonnage_TransferNoteId] ON  [Evidence].[NoteTransferTonnage] 
(
	[TransferNoteId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IDX_NoteTransferTonnage_NoteTonnageId] ON  [Evidence].[NoteTransferTonnage] 
(
	[NoteTonnageId] ASC
)
GO

ALTER TABLE [Evidence].[Note] ALTER COLUMN AatfId UNIQUEIDENTIFIER NULL;
GO

ALTER TABLE [Evidence].[Note] ADD CONSTRAINT CHK_AAtf_OrganisationId CHECK (AatfId IS NOT NULL OR OrganisationId IS NOT NULL)
GO