
ALTER TABLE [Producer].[DirectRegistrant] ADD [ContactId] [uniqueidentifier] NULL;
ALTER TABLE [Producer].[DirectRegistrant] WITH CHECK ADD CONSTRAINT [FK_DirectRegistrant_Contact] 
    FOREIGN KEY([ContactId]) REFERENCES [Organisation].[Contact] ([Id]);

CREATE NONCLUSTERED INDEX [IX_DirectRegistrant_ContactId] ON [Producer].[DirectRegistrant] ([ContactId]);

ALTER TABLE [Producer].[DirectProducerSubmission] ADD [CompanyName] [nvarchar](256) NULL;
ALTER TABLE [Producer].[DirectProducerSubmission] ADD [TradingName] [nvarchar](256) NULL;
ALTER TABLE [Producer].[DirectProducerSubmission] ADD [CompanyRegistrationNumber] [nvarchar](15) NULL;
ALTER TABLE [Producer].[DirectProducerSubmission] ADD [Status] [int] NOT NULL;