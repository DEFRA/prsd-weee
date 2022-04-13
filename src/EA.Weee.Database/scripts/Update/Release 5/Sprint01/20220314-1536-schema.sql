CREATE SCHEMA [Evidence]
    AUTHORIZATION [dbo];
GO

CREATE TABLE [Evidence].[Note](
	[Id] [uniqueidentifier] NOT NULL,
	[Reference] [int] IDENTITY(1,1) NOT NULL,
	[AatfId] [uniqueidentifier] NOT NULL,
	[OrganisationId] [uniqueidentifier] NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NOT NULL,
	[RecipientId] [uniqueidentifier] NOT NULL,
	[WasteType] [int] NULL,
	[Protocol] [int] NULL,
	[CreatedById] [nvarchar](128) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[SubmittedById] [nvarchar](128) NULL,
	[SubmittedDate] [datetime] NULL,
	[Status] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Evidence.Note] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [Evidence].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_OrganisationId] FOREIGN KEY([OrganisationId])
REFERENCES [Organisation].[Organisation] ([Id])
GO

ALTER TABLE [Evidence].[Note]  CHECK CONSTRAINT [FK_Note_OrganisationId]
GO

CREATE NONCLUSTERED INDEX [IDX_Note_OrganisationId] ON  [Evidence].[Note] 
(
	[OrganisationId] ASC
)
GO

ALTER TABLE [Evidence].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_AatfId] FOREIGN KEY([AatfId])
REFERENCES [AATF].[AATF] ([Id])
GO

ALTER TABLE [Evidence].[Note]  CHECK CONSTRAINT [FK_Note_AatfId]
GO

CREATE NONCLUSTERED INDEX [IDX_Note_AatfId] ON  [Evidence].[Note] 
(
	[AatfId] ASC
)
GO

ALTER TABLE [Evidence].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_RecipientId] FOREIGN KEY([RecipientId])
REFERENCES [PCS].[Scheme] ([Id])
GO

ALTER TABLE [Evidence].[Note]  CHECK CONSTRAINT [FK_Note_RecipientId]
GO

CREATE NONCLUSTERED INDEX [IDX_Note_RecipientId] ON  [Evidence].[Note] 
(
	[RecipientId] ASC
)
GO


ALTER TABLE [Evidence].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_CreatedBy_UserId] FOREIGN KEY([CreatedById])
REFERENCES [Identity].[AspNetUsers] ([Id])
GO

ALTER TABLE [Evidence].[Note]  CHECK CONSTRAINT [FK_Note_CreatedBy_UserId]
GO

CREATE NONCLUSTERED INDEX [IDX_Note_CreatedById] ON  [Evidence].[Note] 
(
	[CreatedById] ASC
)
GO


ALTER TABLE [Evidence].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_SubmittedBy_UserId] FOREIGN KEY([SubmittedById])
REFERENCES [Identity].[AspNetUsers] ([Id])
GO

ALTER TABLE [Evidence].[Note]  CHECK CONSTRAINT [FK_Note_SubmittedBy_UserId]
GO

CREATE NONCLUSTERED INDEX [IDX_Note_SubmittedById] ON  [Evidence].[Note] 
(
	[SubmittedById] ASC
)
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Evidence].[NoteTonnage](
	[Id] [uniqueidentifier] NOT NULL,
	[NoteId] [uniqueidentifier] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[Received] [decimal](28, 3) NULL,
	[Reused] [decimal](28, 3) NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_NoteTonnage_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [Evidence].[NoteTonnage]  WITH NOCHECK ADD  CONSTRAINT [FK_NoteTonnage_NoteId] FOREIGN KEY([NoteId])
REFERENCES [Evidence].[NoteTonnage] ([Id])
GO

ALTER TABLE [Evidence].[NoteTonnage] CHECK CONSTRAINT [FK_NoteTonnage_NoteId]
GO

CREATE NONCLUSTERED INDEX [IDX_NoteTonnage_NoteId] ON [Evidence].[NoteTonnage]
(
	[NoteId] ASC
)
GO

