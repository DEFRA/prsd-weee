ALTER TABLE [Producer].[DirectProducerSubmission] ADD PaymentState [int] NULL;
GO
ALTER TABLE [Producer].[DirectProducerSubmission] DROP COLUMN PaymentId;
ALTER TABLE [Producer].[DirectProducerSubmission] DROP COLUMN PaymentReference;
ALTER TABLE [Producer].[DirectProducerSubmission] DROP COLUMN PaymentStatus;

UPDATE [Producer].[DirectProducerSubmission] SET PaymentState = 1;