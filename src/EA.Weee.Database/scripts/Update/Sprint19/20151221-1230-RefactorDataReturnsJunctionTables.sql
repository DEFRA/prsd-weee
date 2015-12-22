ALTER TABLE [PCS].[EeeOutputReturnVersionAmount] DROP CONSTRAINT [PK_EeeOutputReturnVersionAmount]
ALTER TABLE [PCS].[EeeOutputReturnVersionAmount] DROP COLUMN [Id]
ALTER TABLE [PCS].[EeeOutputReturnVersionAmount] ADD CONSTRAINT [PK_EeeOutputReturnVersionAmount] PRIMARY KEY CLUSTERED 
(
	[EeeOutputReturnVersionId] ASC,
	[EeeOuputAmountId] ASC
)
GO

ALTER TABLE [PCS].[WeeeCollectedReturnVersionAmount] DROP CONSTRAINT [PK_WeeeCollectedReturnVersionAmount]
ALTER TABLE [PCS].[WeeeCollectedReturnVersionAmount] DROP COLUMN [Id]
ALTER TABLE [PCS].[WeeeCollectedReturnVersionAmount] ADD CONSTRAINT [PK_WeeeCollectedReturnVersionAmount] PRIMARY KEY CLUSTERED
(
	[WeeeCollectedReturnVersionId] ASC,
	[WeeeCollectedAmountId] ASC
)
GO

ALTER TABLE [PCS].[WeeeDeliveredReturnVersionAmount] DROP CONSTRAINT [PK_WeeeDeliveredReturnVersionAmount]
ALTER TABLE [PCS].[WeeeDeliveredReturnVersionAmount] DROP COLUMN [Id]
ALTER TABLE [PCS].[WeeeDeliveredReturnVersionAmount] ADD CONSTRAINT [PK_WeeeDeliveredReturnVersionAmount] PRIMARY KEY CLUSTERED 
(
	[WeeeDeliveredReturnVersionId] ASC,
	[WeeeDeliveredAmountId] ASC
)
GO