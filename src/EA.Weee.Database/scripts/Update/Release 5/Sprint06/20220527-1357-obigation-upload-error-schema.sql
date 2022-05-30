
CREATE TABLE [Pcs].[ObligationUpload](
	[Id] [uniqueidentifier] NOT NULL,
	[UploadedById] [nvarchar](128) NOT NULL,
	[CompetentAuthorityId] [uniqueidentifier] NOT NULL,
	[UploadedDate] [datetime] NOT NULL,
	[Data] [nvarchar](Max) NOT NULL,
	[FileName] [nvarchar](Max) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_ObligationUpload_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY])
GO

ALTER TABLE [Pcs].[ObligationUpload]  WITH NOCHECK ADD  CONSTRAINT [FK_ObligationUpload_CompetentAuthorityId] FOREIGN KEY([CompetentAuthorityId])
REFERENCES [Lookup].[CompetentAuthority] ([Id])
GO

ALTER TABLE [Pcs].[ObligationUpload] CHECK CONSTRAINT [FK_ObligationUpload_CompetentAuthorityId]
GO
CREATE NONCLUSTERED INDEX [IDX_ObligationUpload_CompetentAuthorityId] ON [Pcs].[ObligationUpload]
(
	[CompetentAuthorityId] ASC
)
GO

CREATE TABLE [Pcs].[ObligationUploadError](
	[Id] [uniqueidentifier] NOT NULL,
	[ObligationUploadId] [uniqueidentifier] NOT NULL,
	[ErrorType] [int] NOT NULL,
	[Description] [nvarchar](1000) NOT NULL,
	[SchemeIdentifier] [nvarchar](16) NULL,
	[SchemeName] [nvarchar](70) NULL,
	[Category] [int] NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_ObligationUploadError_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [Pcs].[ObligationUploadError]  WITH NOCHECK ADD  CONSTRAINT [FK_ObligationUploadError_ObligationUploadId] FOREIGN KEY([ObligationUploadId])
REFERENCES [Pcs].[ObligationUpload] ([Id])
GO

ALTER TABLE [Pcs].[ObligationUploadError] CHECK CONSTRAINT [FK_ObligationUploadError_ObligationUploadId]
GO

CREATE NONCLUSTERED INDEX [IDX_ObligationUploadError_ObligationUploadId] ON [Pcs].[ObligationUploadError]
(
	[ObligationUploadId] ASC
)
GO
