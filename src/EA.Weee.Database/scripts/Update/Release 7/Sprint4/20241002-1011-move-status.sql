ALTER TABLE [Producer].[DirectProducerSubmissionHistory] DROP COLUMN [Status];
ALTER TABLE [Producer].[DirectProducerSubmission] ADD [Status] [int] NULL;
GO
UPDATE [Producer].[DirectProducerSubmission] SET [Status] = 1;
ALTER TABLE [Producer].[DirectProducerSubmission] ALTER COLUMN [Status] [int] NOT NULL;