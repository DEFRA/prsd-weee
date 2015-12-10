GO
PRINT N'Updating [PCS].[DataReturnUpload]...';
ALTER TABLE [PCS].[DataReturnsUpload] DROP CONSTRAINT [FK_DataReturnsUpload_Scheme]

ALTER TABLE [PCS].[DataReturnsUploadError] DROP CONSTRAINT [FK_DataReturnsUploadError_DataReturnsUpload]
DROP TABLE [PCS].[DataReturnsUploadError]
DROP TABLE [PCS].[DataReturnsUpload]


CREATE TABLE [PCS].[DataReturnUpload](
	[Id] [uniqueidentifier] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[Data] [nvarchar](max) NOT NULL,	
	[Date] [datetime] NOT NULL,
	[FileName] [nvarchar](max) NULL,	
	[SchemeId] [uniqueidentifier] NOT NULL,	
	[ComplianceYear] [int] NULL,
	[Quarter] int NULL,
	[DataReturnVersionId] [uniqueidentifier] NULL,
	[ProcessTime] [time](7) NOT NULL
 CONSTRAINT [PK_DataReturnUpload] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [PCS].[DataReturnUpload]  WITH CHECK ADD  CONSTRAINT [FK_DataReturnUpload_Scheme] FOREIGN KEY([SchemeId])
REFERENCES [PCS].[Scheme] ([Id])
GO

ALTER TABLE [PCS].[DataReturnUpload] CHECK CONSTRAINT [FK_DataReturnUpload_Scheme]
GO

PRINT N'Creating [PCS].[DataReturnUploadError]...';

GO
CREATE TABLE[PCS].[DataReturnUploadError](
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
    [DataReturnUploadId] [uniqueidentifier]
NOT NULL,
    [LineNumber] [int]
NOT NULL CONSTRAINT[DF_DataReturnUploadError_LineNumber] DEFAULT((0)),
 CONSTRAINT[PK_DataReturnUploadError] PRIMARY KEY CLUSTERED
(
   [Id] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
) ON[PRIMARY] 

GO

ALTER TABLE[PCS].[DataReturnUploadError]
WITH CHECK ADD CONSTRAINT[FK_DataReturnUploadError_DataReturnUpload] FOREIGN KEY([DataReturnUploadId])
REFERENCES[PCS].[DataReturnUpload] ([Id])
GO

ALTER TABLE[PCS].[DataReturnUploadError]
CHECK CONSTRAINT[FK_DataReturnUploadError_DataReturnUpload]
GO



PRINT N'Create [PCS].[DataReturnVersion]...';
GO
CREATE TABLE [PCS].[DataReturnVersion](
	[Id] [uniqueidentifier] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[SubmittedDate] [datetime] NULL,
	[SubmittingUserId] [nvarchar](128) NULL,
	[DataReturnId] [uniqueidentifier] NOT NULL,	
 CONSTRAINT [PK_DataReturnVersion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

PRINT N'Create [PCS].[DataReturn]...';
CREATE TABLE [PCS].[DataReturn](
	[Id] [uniqueidentifier] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[ComplianceYear] [int] NOT NULL,
	[SchemeId] [uniqueidentifier] NOT NULL,
	[CurrentDataReturnVersionId] [uniqueidentifier] NULL,
	[Quarter] [int] NOT NULL,
 CONSTRAINT [PK_DataReturn] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [PCS].[DataReturnVersion]  WITH CHECK ADD  CONSTRAINT [FK_DataReturnVersion_DataReturn] FOREIGN KEY([DataReturnId])
REFERENCES [PCS].[DataReturn] ([Id])
GO

ALTER TABLE [PCS].[DataReturnVersion] CHECK CONSTRAINT [FK_DataReturnVersion_DataReturn]
GO

ALTER TABLE [PCS].[DataReturn]  WITH CHECK ADD  CONSTRAINT [FK_DataReturn_DataReturnVersion] FOREIGN KEY([CurrentDataReturnVersionId])
REFERENCES [PCS].[DataReturnVersion] ([Id])
GO

ALTER TABLE [PCS].[DataReturn] CHECK CONSTRAINT [FK_DataReturn_DataReturnVersion]
GO

ALTER TABLE [PCS].[DataReturn]  WITH CHECK ADD  CONSTRAINT [FK_DataReturn_Scheme] FOREIGN KEY([SchemeId])
REFERENCES [PCS].[Scheme] ([Id])
GO

ALTER TABLE [PCS].[DataReturn] CHECK CONSTRAINT [FK_DataReturn_Scheme]
GO


PRINT N'Update [PCS].[DataReturnUpload]...';
ALTER TABLE [PCS].[DataReturnUpload]  WITH CHECK ADD  CONSTRAINT [FK_DataReturnUpload_DataReturnVersion] FOREIGN KEY([DataReturnVersionId])
REFERENCES [PCS].[DataReturnVersion] ([Id])
GO

ALTER TABLE [PCS].[DataReturnUpload] CHECK CONSTRAINT [FK_DataReturnUpload_DataReturnVersion]
GO
PRINT N'Update complete...';