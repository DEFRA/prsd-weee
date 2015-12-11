-- Create [PCS].[EeeOutputAmount]
CREATE TABLE [PCS].[EeeOutputAmount]
(
	[Id] [uniqueidentifier] NOT NULL,
	[RegisteredProducerId] [uniqueidentifier] NOT NULL,
	[CategoryId] [uniqueidentifier] NOT NULL,
	[ObligationType] [nvarchar](4) NOT NULL,
	[Tonnage] [decimal](38, 3) NOT NULL,
	
	CONSTRAINT [PK_EeeOutputAmount] PRIMARY KEY CLUSTERED ([Id] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- This check constraint ensures that the obligation type can only have values of 'B2B' or 'B2C'.
ALTER TABLE [PCS].[EeeOutputAmount]  WITH CHECK ADD
	CONSTRAINT [CK_EeeOutputAmount_ObligationType] CHECK ([ObligationType] IN ('B2B', 'B2C'))
GO

ALTER TABLE [PCS].[EeeOutputAmount]
	CHECK CONSTRAINT [CK_EeeOutputAmount_ObligationType]
GO

-- Add Foreign Key constraints.
ALTER TABLE [PCS].[EeeOutputAmount]  WITH CHECK ADD CONSTRAINT [FK_EeeOutputAmount_Category] FOREIGN KEY([CategoryId])
REFERENCES [Lookup].[Category] ([Id])
GO

ALTER TABLE [PCS].[EeeOutputAmount] CHECK CONSTRAINT [FK_EeeOutputAmount_Category]
GO

ALTER TABLE [PCS].[EeeOutputAmount]  WITH CHECK ADD CONSTRAINT [FK_EeeOutputAmount_RegisteredProducer] FOREIGN KEY([RegisteredProducerId])
REFERENCES [Producer].[RegisteredProducer] ([Id])
GO

ALTER TABLE [PCS].[EeeOutputAmount] CHECK CONSTRAINT [FK_EeeOutputAmount_RegisteredProducer]
GO

-- Create [PCS].[EeeOutputReturnVersion]
CREATE TABLE [PCS].[EeeOutputReturnVersion]
(
	[Id] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_EeeOutputReturnVersion] PRIMARY KEY CLUSTERED([Id] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Create [PCS].[EeeOutputReturnVersionAmount]
CREATE TABLE [PCS].[EeeOutputReturnVersionAmount]
(
	[Id] [uniqueidentifier] NOT NULL,
	[EeeOutputReturnVersionId] [uniqueidentifier] NOT NULL,
	[EeeOuputAmountId] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_EeeOutputReturnVersionAmount] PRIMARY KEY CLUSTERED ([Id] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Add Foreign Key constraints.
ALTER TABLE [PCS].[EeeOutputReturnVersionAmount]  WITH CHECK ADD CONSTRAINT [FK_EeeOutputReturnVersionAmount_EeeOutputAmount] FOREIGN KEY([EeeOuputAmountId])
REFERENCES [PCS].[EeeOutputAmount] ([Id])
GO

ALTER TABLE [PCS].[EeeOutputReturnVersionAmount] CHECK CONSTRAINT [FK_EeeOutputReturnVersionAmount_EeeOutputAmount]
GO

ALTER TABLE [PCS].[EeeOutputReturnVersionAmount]  WITH CHECK ADD CONSTRAINT [FK_EeeOutputReturnVersionAmount_EeeOutputReturnVersion] FOREIGN KEY([EeeOutputReturnVersionId])
REFERENCES [PCS].[EeeOutputReturnVersion] ([Id])
GO

ALTER TABLE [PCS].[EeeOutputReturnVersionAmount] CHECK CONSTRAINT [FK_EeeOutputReturnVersionAmount_EeeOutputReturnVersion]
GO

-- Add [EeeOutputReturnVersionId] column to [PCS].[DataReturnVersion]
ALTER TABLE [PCS].[DataReturnVersion]
ADD EeeOutputReturnVersionId [uniqueidentifier]
GO

ALTER TABLE [PCS].[DataReturnVersion]  WITH CHECK ADD CONSTRAINT [FK_DataReturnVersion_EeeOutputReturnVersion] FOREIGN KEY([EeeOutputReturnVersionId])
REFERENCES [PCS].[EeeOutputReturnVersion] ([Id])
GO

ALTER TABLE [PCS].[DataReturnVersion] CHECK CONSTRAINT [FK_DataReturnVersion_EeeOutputReturnVersion]
GO