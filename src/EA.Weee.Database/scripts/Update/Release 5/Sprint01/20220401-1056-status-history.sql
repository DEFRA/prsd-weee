CREATE TABLE [Evidence].[NoteStatusHistory](
	[Id] [uniqueidentifier] NOT NULL,
	[NoteId] [uniqueidentifier] NOT NULL,
	[FromStatus] [int] NOT NULL,
	[ToStatus] [int] NOT NULL,
	[ChangedById] [nvarchar](128) NOT NULL,
	[ChangedDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NOT NULL
 CONSTRAINT [PK_Evidence.NoteStatusHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [Evidence].[NoteStatusHistory]  WITH CHECK ADD  CONSTRAINT [FK_NoteStatusHistory_NoteId] FOREIGN KEY([NoteId])
REFERENCES [Evidence].[Note] ([Id])
GO

ALTER TABLE [Evidence].[NoteStatusHistory]  CHECK CONSTRAINT [FK_NoteStatusHistory_NoteId]
GO

CREATE NONCLUSTERED INDEX [IDX_NoteStatusHistory_NoteId] ON  [Evidence].[NoteStatusHistory] 
(
	[NoteId] ASC
)
GO

ALTER TABLE [Evidence].[NoteStatusHistory]  WITH CHECK ADD  CONSTRAINT [FK_NoteStatusHistory_ChangedBy_UserId] FOREIGN KEY([ChangedById])
REFERENCES [Identity].[AspNetUsers] ([Id])
GO

ALTER TABLE [Evidence].[NoteStatusHistory]  CHECK CONSTRAINT [FK_NoteStatusHistory_ChangedBy_UserId]
GO

CREATE NONCLUSTERED INDEX [IDX_NoteStatusHistory_ChangedById] ON  [Evidence].[NoteStatusHistory] 
(
	[ChangedById] ASC
)
GO