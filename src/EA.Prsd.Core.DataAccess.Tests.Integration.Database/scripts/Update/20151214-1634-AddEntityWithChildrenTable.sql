SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Test].[EntityWithChildren](
	[Id] [uniqueidentifier] NOT NULL,
	[SimpleEntityAId] [uniqueidentifier] NULL,
	[SimpleEntityBId] [uniqueidentifier] NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_EntityWithChildren] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Test].[EntityWithChildren]  WITH CHECK ADD  CONSTRAINT [FK_EntityWithChildren_SimpleEntityA] FOREIGN KEY([SimpleEntityAId])
REFERENCES [Test].[SimpleEntity] ([Id])
GO

ALTER TABLE [Test].[EntityWithChildren] CHECK CONSTRAINT [FK_EntityWithChildren_SimpleEntityA]
GO

ALTER TABLE [Test].[EntityWithChildren]  WITH CHECK ADD  CONSTRAINT [FK_EntityWithChildren_SimpleEntityB] FOREIGN KEY([SimpleEntityBId])
REFERENCES [Test].[SimpleEntity] ([Id])
GO

ALTER TABLE [Test].[EntityWithChildren] CHECK CONSTRAINT [FK_EntityWithChildren_SimpleEntityB]
GO
