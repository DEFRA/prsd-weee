-- Create [PCS].[WeeeDeliveredReturnVersion]
CREATE TABLE [PCS].[WeeeDeliveredReturnVersion]
(
	[Id] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_WeeeDeliveredReturnVersion] PRIMARY KEY CLUSTERED([Id] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Create [PCS].[AatfDeliveryLocation]
CREATE TABLE [PCS].[AatfDeliveryLocation]
(
	[Id] [uniqueidentifier] NOT NULL,
	[ApprovalNumber] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	CONSTRAINT [PK_AatfDeliveryLocation] PRIMARY KEY CLUSTERED([Id] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Create [PCS].[AeDeliveryLocation]
CREATE TABLE [PCS].[AeDeliveryLocation]
(
	[Id] [uniqueidentifier] NOT NULL,
	[ApprovalNumber] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	CONSTRAINT [PK_AeDeliveryLocation] PRIMARY KEY CLUSTERED([Id] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Create [PCS].[WeeeDeliveredAmount]
CREATE TABLE [PCS].[WeeeDeliveredAmount](
	[Id] [uniqueidentifier] NOT NULL,
	[AatfDeliveryLocationId] [uniqueidentifier] NULL,
	[AeDeliveryLocationId] [uniqueidentifier] NULL,
	[CategoryId] [uniqueidentifier] NOT NULL,
	[ObligationType] [nvarchar](4) NOT NULL,
	[Tonnage] [decimal](38, 3) NOT NULL,
	CONSTRAINT [PK_WeeeDeliveredAmount] PRIMARY KEY CLUSTERED([Id] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Add Foreign Key constraints.
ALTER TABLE [PCS].[WeeeDeliveredAmount]  WITH CHECK ADD CONSTRAINT [FK_WeeeDeliveredAmount_AatfDeliveryLocation] FOREIGN KEY([AatfDeliveryLocationId])
REFERENCES [PCS].[AatfDeliveryLocation] ([Id])
GO

ALTER TABLE [PCS].[WeeeDeliveredAmount] CHECK CONSTRAINT [FK_WeeeDeliveredAmount_AatfDeliveryLocation]
GO

ALTER TABLE [PCS].[WeeeDeliveredAmount]  WITH CHECK ADD CONSTRAINT [FK_WeeeDeliveredAmount_AeDeliveryLocation] FOREIGN KEY([AeDeliveryLocationId])
REFERENCES [PCS].[AeDeliveryLocation] ([Id])
GO

ALTER TABLE [PCS].[WeeeDeliveredAmount] CHECK CONSTRAINT [FK_WeeeDeliveredAmount_AeDeliveryLocation]
GO

ALTER TABLE [PCS].[WeeeDeliveredAmount]  WITH CHECK ADD CONSTRAINT [FK_WeeeDeliveredAmount_Category] FOREIGN KEY([CategoryId])
REFERENCES [Lookup].[Category] ([Id])
GO

ALTER TABLE [PCS].[WeeeDeliveredAmount] CHECK CONSTRAINT [FK_WeeeDeliveredAmount_Category]
GO

ALTER TABLE [PCS].[WeeeDeliveredAmount]  WITH CHECK ADD CONSTRAINT [CK_WeeeDeliveredAmount_ObligationType] CHECK  (([ObligationType]='B2B' OR [ObligationType]='B2C'))
GO

ALTER TABLE [PCS].[WeeeDeliveredAmount] CHECK CONSTRAINT [CK_WeeeDeliveredAmount_ObligationType]
GO

-- Create [PCS].[WeeeDeliveredReturnVersionAmount]
CREATE TABLE [PCS].[WeeeDeliveredReturnVersionAmount]
(
	[Id] [uniqueidentifier] NOT NULL,
	[WeeeDeliveredReturnVersionId] [uniqueidentifier] NOT NULL,
	[WeeeDeliveredAmountId] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_WeeeDeliveredReturnVersionAmount] PRIMARY KEY CLUSTERED([Id] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Add Foreign Key constraints.
ALTER TABLE [PCS].[WeeeDeliveredReturnVersionAmount]  WITH CHECK ADD CONSTRAINT [FK_WeeeDeliveredReturnVersionAmount_WeeeDeliveredAmount] FOREIGN KEY([WeeeDeliveredAmountId])
REFERENCES [PCS].[WeeeDeliveredAmount] ([Id])
GO

ALTER TABLE [PCS].[WeeeDeliveredReturnVersionAmount] CHECK CONSTRAINT [FK_WeeeDeliveredReturnVersionAmount_WeeeDeliveredAmount]
GO

ALTER TABLE [PCS].[WeeeDeliveredReturnVersionAmount]  WITH CHECK ADD CONSTRAINT [FK_WeeeDeliveredReturnVersionAmount_WeeeDeliveredReturnVersion] FOREIGN KEY([WeeeDeliveredReturnVersionId])
REFERENCES [PCS].[WeeeDeliveredReturnVersion] ([Id])
GO

ALTER TABLE [PCS].[WeeeDeliveredReturnVersionAmount] CHECK CONSTRAINT [FK_WeeeDeliveredReturnVersionAmount_WeeeDeliveredReturnVersion]
GO

-- Add [WeeeDeliveredReturnVersionId] column to [PCS].[DataReturnVersion]
ALTER TABLE [PCS].[DataReturnVersion]
ADD WeeeDeliveredReturnVersionId [uniqueidentifier]
GO

ALTER TABLE [PCS].[DataReturnVersion]  WITH CHECK ADD CONSTRAINT [FK_DataReturnVersion_WeeeDeliveredReturnVersion] FOREIGN KEY([WeeeDeliveredReturnVersionId])
REFERENCES [PCS].[WeeeDeliveredReturnVersion] ([Id])
GO

ALTER TABLE [PCS].[DataReturnVersion] CHECK CONSTRAINT [FK_DataReturnVersion_WeeeDeliveredReturnVersion]
GO

