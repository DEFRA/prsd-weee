CREATE NONCLUSTERED INDEX [IDX_NoteTonnage_NoteId_Received]
ON [Evidence].[NoteTonnage] ([CategoryId])
INCLUDE ([NoteId],[Received])

GO

CREATE NONCLUSTERED INDEX [IDX_NoteTonnage_NoteId_Reused]
ON [Evidence].[NoteTonnage] ([CategoryId])
INCLUDE ([NoteId],[Reused])