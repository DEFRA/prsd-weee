DROP INDEX [IX_DirectProducerSubmission_CreatedById] ON [Producer].[DirectProducerSubmission];
ALTER TABLE [Producer].[DirectProducerSubmission] DROP CONSTRAINT [FK_DirectProducerSubmission_CreatedBy];
ALTER TABLE [Producer].[DirectProducerSubmission] DROP COLUMN [CreatedById];

DROP INDEX [IX_DirectProducerSubmission_UpdatedById] ON [Producer].[DirectProducerSubmission];
ALTER TABLE [Producer].[DirectProducerSubmission] DROP CONSTRAINT [FK_DirectProducerSubmission_UpdatedBy];
ALTER TABLE [Producer].[DirectProducerSubmission] DROP COLUMN [UpdatedById];

ALTER TABLE [Producer].[DirectProducerSubmission] DROP COLUMN [CreatedDate];
ALTER TABLE [Producer].[DirectProducerSubmission] DROP COLUMN [UpdatedDate];
ALTER TABLE [Producer].[DirectProducerSubmission] DROP COLUMN [SubmittedDate];
ALTER TABLE [Producer].[DirectProducerSubmission] DROP COLUMN [Status];

DROP INDEX [IX_DirectProducerSubmission_ServiceOfNoticeAddressId] ON [Producer].[DirectProducerSubmission];
ALTER TABLE [Producer].[DirectProducerSubmission] DROP CONSTRAINT [FK_DirectProducerSubmission_ServiceOfNoticeAddress];
ALTER TABLE [Producer].[DirectProducerSubmission] DROP COLUMN [ServiceOfNoticeAddressId];

DROP INDEX [IX_DirectProducerSubmission_AppropriateSignatoryId] ON [Producer].[DirectProducerSubmission];
ALTER TABLE [Producer].[DirectProducerSubmission] DROP CONSTRAINT [FK_DirectProducerSubmission_AppropriateSignatory];
ALTER TABLE [Producer].[DirectProducerSubmission] DROP COLUMN [AppropriateSignatoryId];

ALTER TABLE [Producer].[DirectProducerSubmission] ADD [ComplianceYear] [int] NOT NULL;
ALTER TABLE [Producer].[DirectProducerSubmission] ADD [RegisteredProducerId] [uniqueidentifier] NOT NULL;

ALTER TABLE [Producer].[DirectProducerSubmission] WITH CHECK ADD CONSTRAINT [FK_DirectProducerSubmission_RegisteredProducer] 
    FOREIGN KEY([RegisteredProducerId]) REFERENCES [Producer].[RegisteredProducer] ([Id]);
CREATE NONCLUSTERED INDEX [IX_DirectProducerSubmission_RegisteredProducerId] ON [Producer].[DirectProducerSubmission] ([RegisteredProducerId]);

ALTER TABLE [Producer].[DirectProducerSubmission] DROP COLUMN [CompanyName];
ALTER TABLE [Producer].[DirectProducerSubmission] DROP COLUMN [TradingName];
ALTER TABLE [Producer].[DirectProducerSubmission] DROP COLUMN [CompanyRegistrationNumber];

