ALTER TABLE [PCS].[EeeOutputAmount] DROP FK_EeeOutputAmount_WeeeCategory
ALTER TABLE [PCS].[EeeOutputAmount] DROP COLUMN WeeeCategoryId
ALTER TABLE [PCS].[EeeOutputAmount] ADD WeeeCategory INT NOT NULL
GO

ALTER TABLE [PCS].[WeeeCollectedAmount] DROP FK_WeeeCollectedAmount_WeeeCategory
ALTER TABLE [PCS].[WeeeCollectedAmount] DROP COLUMN WeeeCategoryId
ALTER TABLE [PCS].[WeeeCollectedAmount] ADD WeeeCategory INT NOT NULL
GO

ALTER TABLE [PCS].[WeeeDeliveredAmount] DROP FK_WeeeDeliveredAmount_WeeeCategory
ALTER TABLE [PCS].[WeeeDeliveredAmount] DROP COLUMN WeeeCategoryId
ALTER TABLE [PCS].[WeeeDeliveredAmount] ADD WeeeCategory INT NOT NULL
GO

DROP TABLE [Lookup].[WeeeCategory]
GO
