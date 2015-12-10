-- Create [PCS].[EEEOutputAmount]
CREATE TABLE [PCS].[EEEOutputAmount]
(
	[Id] [uniqueidentifier] NOT NULL,
	[RegisteredProducerId] [uniqueidentifier] NOT NULL,
	[CategoryId] [uniqueidentifier] NOT NULL,
	[ObligationType] [nvarchar](4) NOT NULL,
	[Tonnage] [decimal](38, 3) NOT NULL,
	
	CONSTRAINT [PK_EEEOutputAmount] PRIMARY KEY CLUSTERED ([Id] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- This check constraint ensures that the obligation type can only have values of 'B2B' or 'B2C'.
ALTER TABLE [PCS].[EEEOutputAmount]  WITH CHECK ADD
	CONSTRAINT [CK_ProducerSubmission_ObligationType] CHECK ([ObligationType] IN ('B2B', 'B2C'))
GO

ALTER TABLE [PCS].[EEEOutputAmount]
	CHECK CONSTRAINT [CK_EEEOutputAmount_ObligationType]
GO

-- Add Foreign Key constraints.
ALTER TABLE [PCS].[EEEOutputAmount]  WITH CHECK ADD CONSTRAINT [FK_EEEOutputAmount_Category] FOREIGN KEY([CategoryId])
REFERENCES [Lookup].[Category] ([Id])
GO

ALTER TABLE [PCS].[EEEOutputAmount] CHECK CONSTRAINT [FK_EEEOutputAmount_Category]
GO

ALTER TABLE [PCS].[EEEOutputAmount]  WITH CHECK ADD CONSTRAINT [FK_EEEOutputAmount_RegisteredProducer] FOREIGN KEY([RegisteredProducerId])
REFERENCES [Producer].[RegisteredProducer] ([Id])
GO

ALTER TABLE [PCS].[EEEOutputAmount] CHECK CONSTRAINT [FK_EEEOutputAmount_RegisteredProducer]
GO

-- Create [PCS].[EEEOutputReturnVersion]
CREATE TABLE [PCS].[EEEOutputReturnVersion]
(
	[Id] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_EEEOutputReturnVersion] PRIMARY KEY CLUSTERED([Id] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Create [PCS].[EEEOutputReturnVersionAmount]
CREATE TABLE [PCS].[EEEOutputReturnVersionAmount]
(
	[Id] [uniqueidentifier] NOT NULL,
	[EEEOutputReturnVersionId] [uniqueidentifier] NOT NULL,
	[EEEOuputAmountId] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_EEEOutputReturnVersionAmount] PRIMARY KEY CLUSTERED ([Id] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Add Foreign Key constraints.
ALTER TABLE [PCS].[EEEOutputReturnVersionAmount]  WITH CHECK ADD CONSTRAINT [FK_EEEOutputReturnVersionAmount_EEEOutputAmount] FOREIGN KEY([EEEOuputAmountId])
REFERENCES [PCS].[EEEOutputAmount] ([Id])
GO

ALTER TABLE [PCS].[EEEOutputReturnVersionAmount] CHECK CONSTRAINT [FK_EEEOutputReturnVersionAmount_EEEOutputAmount]
GO

ALTER TABLE [PCS].[EEEOutputReturnVersionAmount]  WITH CHECK ADD CONSTRAINT [FK_EEEOutputReturnVersionAmount_EEEOutputReturnVersion] FOREIGN KEY([EEEOutputReturnVersionId])
REFERENCES [PCS].[EEEOutputReturnVersion] ([Id])
GO

ALTER TABLE [PCS].[EEEOutputReturnVersionAmount] CHECK CONSTRAINT [FK_EEEOutputReturnVersionAmount_EEEOutputReturnVersion]
GO