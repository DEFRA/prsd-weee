﻿
CREATE TABLE [Pcs].[ObligationScheme](
	[Id] [uniqueidentifier] NOT NULL,
	[ObligationUploadId] [uniqueidentifier] NOT NULL,
	[ComplianceYear] INT NOT NULL,
	[UpdatedDate] DATETIME NOT NULL,
	[SchemeId] [uniqueidentifier] NOT NULL,
	[CategoryId] INT NOT NULL,
	[Obligation] DECIMAL(28,3) NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_ObligationScheme_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY])
GO

ALTER TABLE [Pcs].[ObligationScheme]  WITH CHECK ADD  CONSTRAINT [FK_ObligationScheme_SchemeId] FOREIGN KEY([SchemeId])
REFERENCES [PCS].[Scheme] ([Id])
GO

ALTER TABLE [Pcs].[ObligationScheme]  CHECK CONSTRAINT [FK_ObligationScheme_SchemeId]
GO

CREATE NONCLUSTERED INDEX [IDX_ObligationScheme_SchemeId] ON [Pcs].[ObligationScheme]
(
	[SchemeId] ASC
)
GO

ALTER TABLE [Pcs].[ObligationScheme]  WITH CHECK ADD  CONSTRAINT [FK_ObligationScheme_ObligationUploadId] FOREIGN KEY([ObligationUploadId])
REFERENCES [PCS].[ObligationUpload] ([Id])
GO

ALTER TABLE [Pcs].[ObligationScheme]  CHECK CONSTRAINT [FK_ObligationScheme_ObligationUploadId]
GO

CREATE NONCLUSTERED INDEX [IDX_ObligationScheme_ObligationUploadId] ON [Pcs].[ObligationScheme]
(
	[ObligationUploadId] ASC
)
GO

ALTER TABLE [Pcs].[ObligationScheme]
ADD CONSTRAINT UNQ_Category_Scheme UNIQUE (CategoryId, SchemeId, ComplianceYear);
GO