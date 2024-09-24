ALTER TABLE [Producer].[DirectProducerSubmission] DROP COLUMN PaymentState;



ALTER TABLE  [Producer].[DirectProducerSubmission] ADD FinalPaymentSessionId [uniqueidentifier] NULL;
ALTER TABLE [Producer].[DirectProducerSubmission] WITH CHECK ADD CONSTRAINT [FK_DirectProducerSubmission_FinalPaymentSessionId] 
    FOREIGN KEY([FinalPaymentSessionId]) REFERENCES [Producer].[PaymentSession] ([Id]);

CREATE NONCLUSTERED INDEX [IX_DirectProducerSubmission_FinalPaymentSessionId] ON [Producer].[DirectProducerSubmission] ([FinalPaymentSessionId]);