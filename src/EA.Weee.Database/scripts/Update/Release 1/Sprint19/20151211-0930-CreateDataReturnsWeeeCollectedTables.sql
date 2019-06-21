-- Create [PCS].[WeeeCollectedReturnVersion]
CREATE TABLE [PCS].[WeeeCollectedReturnVersion]
(
	[Id] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_WeeeCollectedReturnVersion] PRIMARY KEY CLUSTERED([Id] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Create [PCS].[WeeeCollectedAmount]
CREATE TABLE [PCS].[WeeeCollectedAmount](
	[Id] [uniqueidentifier] NOT NULL,
	[SourceType] [int] NOT NULL,
	[CategoryId] [uniqueidentifier] NOT NULL,
	[ObligationType] [nvarchar](4) NOT NULL,
	[Tonnage] [decimal](38, 3) NOT NULL,
	CONSTRAINT [PK_WeeeCollectedAmount] PRIMARY KEY CLUSTERED ([Id] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Add Foreign Key constraints.
ALTER TABLE [PCS].[WeeeCollectedAmount]  WITH CHECK ADD CONSTRAINT [FK_WeeeCollectedAmount_Category] FOREIGN KEY([CategoryId])
REFERENCES [Lookup].[Category] ([Id])
GO

ALTER TABLE [PCS].[WeeeCollectedAmount] CHECK CONSTRAINT [FK_WeeeCollectedAmount_Category]
GO

ALTER TABLE [PCS].[WeeeCollectedAmount]  WITH CHECK ADD CONSTRAINT [CK_WeeeCollectedAmount] CHECK  (([ObligationType]='B2B' OR [ObligationType]='B2C'))
GO

ALTER TABLE [PCS].[WeeeCollectedAmount] CHECK CONSTRAINT [CK_WeeeCollectedAmount]
GO

-- Create [PCS].[WeeeCollectedReturnVersionAmount]
CREATE TABLE [PCS].[WeeeCollectedReturnVersionAmount]
(
	[Id] [uniqueidentifier] NOT NULL,
	[WeeeCollectedReturnVersionId] [uniqueidentifier] NOT NULL,
	[WeeeCollectedAmountId] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_WeeeCollectedReturnVersionAmount] PRIMARY KEY CLUSTERED ([Id] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Add Foreign Key constraints.
ALTER TABLE [PCS].[WeeeCollectedReturnVersionAmount]  WITH CHECK ADD CONSTRAINT [FK_WeeeCollectedReturnVersionAmount_WeeeCollectedAmount] FOREIGN KEY([WeeeCollectedAmountId])
REFERENCES [PCS].[WeeeCollectedAmount] ([Id])
GO

ALTER TABLE [PCS].[WeeeCollectedReturnVersionAmount] CHECK CONSTRAINT [FK_WeeeCollectedReturnVersionAmount_WeeeCollectedAmount]
GO

ALTER TABLE [PCS].[WeeeCollectedReturnVersionAmount]  WITH CHECK ADD CONSTRAINT [FK_WeeeCollectedReturnVersionAmount_WeeeCollectedReturnVersion] FOREIGN KEY([WeeeCollectedReturnVersionId])
REFERENCES [PCS].[WeeeCollectedReturnVersion] ([Id])
GO

ALTER TABLE [PCS].[WeeeCollectedReturnVersionAmount] CHECK CONSTRAINT [FK_WeeeCollectedReturnVersionAmount_WeeeCollectedReturnVersion]
GO

-- Add [WeeeDeliveredReturnVersionId] column to [PCS].[DataReturnVersion]
ALTER TABLE [PCS].[DataReturnVersion]
ADD WeeeCollectedReturnVersionId [uniqueidentifier]
GO

ALTER TABLE [PCS].[DataReturnVersion]  WITH CHECK ADD CONSTRAINT [FK_DataReturnVersion_WeeeCollectedReturnVersion] FOREIGN KEY([WeeeCollectedReturnVersionId])
REFERENCES [PCS].[WeeeCollectedReturnVersion] ([Id])
GO

ALTER TABLE [PCS].[DataReturnVersion] CHECK CONSTRAINT [FK_DataReturnVersion_WeeeCollectedReturnVersion]
GO