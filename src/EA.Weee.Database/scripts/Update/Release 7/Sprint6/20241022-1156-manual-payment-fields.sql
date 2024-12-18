
ALTER TABLE  [Producer].[DirectProducerSubmission] ADD ManualPaymentMethod [nvarchar](500) NULL;
ALTER TABLE  [Producer].[DirectProducerSubmission] ADD ManualPaymentReceivedDate DATETIME2 NULL;
ALTER TABLE  [Producer].[DirectProducerSubmission] ADD ManualPaymentDetails [nvarchar](2000) NULL;
ALTER TABLE  [Producer].[DirectProducerSubmission] ADD ManualPaymentMadeByUserId [nvarchar](128) NULL;

ALTER TABLE [Producer].[DirectProducerSubmission] WITH CHECK ADD CONSTRAINT [FK_DirectProducerSubmission_ManualPaymentMadeByUserId] 
    FOREIGN KEY(ManualPaymentMadeByUserId) REFERENCES [Identity].[AspNetUsers] ([Id]);

CREATE NONCLUSTERED INDEX [IX_DirectProducerSubmission_ManualPaymentMadeByUserId] ON [Producer].[DirectProducerSubmission] ([ManualPaymentMadeByUserId]);
