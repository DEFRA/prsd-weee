SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Test].[EntityWithForeignId](
	[Id] [uniqueidentifier] NOT NULL,
	[SimpleEntityId] [uniqueidentifier] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_EntityWithForeignId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Test].[EntityWithForeignId]  WITH CHECK ADD  CONSTRAINT [FK_EntityWithForeignId_SimpleEntity] FOREIGN KEY([SimpleEntityId])
REFERENCES [Test].[SimpleEntity] ([Id])
GO

ALTER TABLE [Test].[EntityWithForeignId] CHECK CONSTRAINT [FK_EntityWithForeignId_SimpleEntity]
GO