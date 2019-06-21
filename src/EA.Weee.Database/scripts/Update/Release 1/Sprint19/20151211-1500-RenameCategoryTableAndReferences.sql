EXEC sp_rename 'Lookup.Category', 'WeeeCategory';
GO

-- EeeOutputAmount
ALTER TABLE [PCS].[EeeOutputAmount] DROP CONSTRAINT FK_EeeOutputAmount_Category;
GO

EXEC sp_rename 'PCS.EeeOutputAmount.CategoryId', 'WeeeCategoryId', 'COLUMN';
GO

ALTER TABLE [PCS].[EeeOutputAmount] WITH CHECK ADD CONSTRAINT [FK_EeeOutputAmount_WeeeCategory] FOREIGN KEY([WeeeCategoryId])
REFERENCES [Lookup].[WeeeCategory] ([Id])
GO

ALTER TABLE [PCS].[EeeOutputAmount] CHECK CONSTRAINT [FK_EeeOutputAmount_WeeeCategory]
GO

-- WeeeCollectedAmount
ALTER TABLE [PCS].[WeeeCollectedAmount] DROP CONSTRAINT FK_WeeeCollectedAmount_Category;
GO

EXEC sp_rename 'PCS.WeeeCollectedAmount.CategoryId', 'WeeeCategoryId', 'COLUMN';
GO

ALTER TABLE [PCS].[WeeeCollectedAmount] WITH CHECK ADD CONSTRAINT [FK_WeeeCollectedAmount_WeeeCategory] FOREIGN KEY([WeeeCategoryId])
REFERENCES [Lookup].[WeeeCategory] ([Id])
GO

ALTER TABLE [PCS].[WeeeCollectedAmount] CHECK CONSTRAINT [FK_WeeeCollectedAmount_WeeeCategory]
GO

-- WeeeDeliveredAmount
ALTER TABLE [PCS].[WeeeDeliveredAmount] DROP CONSTRAINT FK_WeeeDeliveredAmount_Category;
GO

EXEC sp_rename 'PCS.WeeeDeliveredAmount.CategoryId', 'WeeeCategoryId', 'COLUMN';
GO

ALTER TABLE [PCS].[WeeeDeliveredAmount] WITH CHECK ADD CONSTRAINT [FK_WeeeDeliveredAmount_WeeeCategory] FOREIGN KEY([WeeeCategoryId])
REFERENCES [Lookup].[WeeeCategory] ([Id])
GO

ALTER TABLE [PCS].[WeeeDeliveredAmount] CHECK CONSTRAINT [FK_WeeeDeliveredAmount_WeeeCategory]
GO
