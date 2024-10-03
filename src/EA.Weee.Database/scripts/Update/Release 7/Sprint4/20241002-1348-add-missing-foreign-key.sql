ALTER TABLE [Producer].[DirectProducerSubmission] WITH CHECK ADD CONSTRAINT [FK_DirectProducerSubmission_DirectRegistrant] 
    FOREIGN KEY([DirectRegistrantId]) REFERENCES [Producer].[DirectRegistrant] ([Id]);


