
GO
PRINT N'Creating [PCS].[DataReturnsUpload]...';

GO
CREATE TABLE [PCS].[DataReturnsUpload](
	[Id] [uniqueidentifier] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[Data] [nvarchar](max) NOT NULL,
	[ComplianceYear] [int] NULL,
	[SchemeId] [uniqueidentifier] NOT NULL,
	[IsSubmitted] [bit] NOT NULL,
	[Date] [datetime] NOT NULL,
	[FileName] [nvarchar](max) NULL,
 CONSTRAINT [PK_DataReturnsUpload] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [PCS].[DataReturnsUpload]  WITH CHECK ADD  CONSTRAINT [FK_DataReturnsUpload_Scheme] FOREIGN KEY([SchemeId])
REFERENCES [PCS].[Scheme] ([Id])
GO

ALTER TABLE [PCS].[DataReturnsUpload] CHECK CONSTRAINT [FK_DataReturnsUpload_Scheme]
GO

GO
PRINT N'Creating [PCS].[DataReturnsUploadError]...';

GO
CREATE TABLE[PCS].[DataReturnsUploadError](
	[Id]
[uniqueidentifier]
NOT NULL,
    [RowVersion] [timestamp]
NOT NULL,
    [ErrorLevel] [int]
NOT NULL,
    [ErrorType] [int]
NOT NULL,
    [Description] [nvarchar](max) NOT NULL,
    [DataReturnsUploadId] [uniqueidentifier]
NOT NULL,
    [LineNumber] [int]
NOT NULL CONSTRAINT[DF_DataReturnsUploadError_LineNumber] DEFAULT((0)),
 CONSTRAINT[PK_DataReturnsUploadError] PRIMARY KEY CLUSTERED
(
   [Id] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]

GO

ALTER TABLE[PCS].[DataReturnsUploadError]
WITH CHECK ADD CONSTRAINT[FK_DataReturnsUploadError_DataReturnsUpload] FOREIGN KEY([DataReturnsUploadId])
REFERENCES[PCS].[DataReturnsUpload] ([Id])
GO

ALTER TABLE[PCS].[DataReturnsUploadError]
CHECK CONSTRAINT[FK_DataReturnsUploadError_DataReturnsUpload]
GO

PRINT N'Update complete.';
GO