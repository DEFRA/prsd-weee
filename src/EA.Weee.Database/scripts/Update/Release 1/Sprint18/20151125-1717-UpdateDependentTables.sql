/*
 * The brand name and SIC code tables depend on the producer table.
 * This script updates these two tables to reference the new producer submission table.
 */

-- Create and populate producer submission ID column.
ALTER TABLE [Producer].[BrandName]
ADD [ProducerSubmissionId] UNIQUEIDENTIFIER NULL
GO

UPDATE [Producer].[BrandName]
SET [ProducerSubmissionId] = [ProducerId]
GO

ALTER TABLE [Producer].[BrandName]
ALTER COLUMN [ProducerSubmissionId] UNIQUEIDENTIFIER NOT NULL
GO

-- Create new foreign key and drop existing foreign key.
ALTER TABLE [Producer].[BrandName] WITH CHECK
ADD CONSTRAINT [FK_BrandName_ProducerSubmission] FOREIGN KEY([ProducerSubmissionId])
REFERENCES [Producer].[ProducerSubmission] ([Id])
GO

ALTER TABLE [Producer].[BrandName]
CHECK CONSTRAINT [FK_BrandName_ProducerSubmission]
GO

ALTER TABLE [Producer].[BrandName]
DROP CONSTRAINT [FK_BrandNameList_Producer]
GO

-- Drop existing column.
ALTER TABLE [Producer].[BrandName]
DROP COLUMN [ProducerId]
GO


-- Create and populate producer submission ID column.
ALTER TABLE [Producer].[SICCode]
ADD [ProducerSubmissionId] UNIQUEIDENTIFIER NULL
GO

UPDATE [Producer].[SICCode]
SET [ProducerSubmissionId] = [ProducerId]
GO

ALTER TABLE [Producer].[SICCode]
ALTER COLUMN [ProducerSubmissionId] UNIQUEIDENTIFIER NOT NULL
GO

-- Create new foreign key and drop existing foreign key.
ALTER TABLE [Producer].[SICCode] WITH CHECK
ADD CONSTRAINT [FK_SICCode_ProducerSubmission] FOREIGN KEY([ProducerSubmissionId])
REFERENCES [Producer].[ProducerSubmission] ([Id])
GO

ALTER TABLE [Producer].[SICCode]
CHECK CONSTRAINT [FK_SICCode_ProducerSubmission]
GO

ALTER TABLE [Producer].[SICCode]
DROP CONSTRAINT [FK_SICCodeList_Producer]
GO

-- Create new index and drop existing index.
CREATE NONCLUSTERED INDEX [IX_SICCode_ProducerSubmissionId] ON [Producer].[SICCode] 
(
	[ProducerSubmissionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

DROP INDEX [IX_SICCode_ProducerId] ON [Producer].[SICCode] WITH ( ONLINE = OFF )

-- Drop existing column.
ALTER TABLE [Producer].[SICCode]
DROP COLUMN [ProducerId]
GO