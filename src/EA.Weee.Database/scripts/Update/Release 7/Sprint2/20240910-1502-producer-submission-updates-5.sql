
ALTER TABLE  [Producer].[DirectProducerSubmissionHistory] ADD AuthorisedRepresentativeId [uniqueidentifier] NULL;

ALTER TABLE [Producer].[DirectProducerSubmissionHistory] WITH CHECK ADD CONSTRAINT [FK_DirectProducerSubmissionHistory_AuthorisedRepresentative] 
    FOREIGN KEY([AuthorisedRepresentativeId]) REFERENCES [Producer].[AuthorisedRepresentative] ([Id]);
CREATE NONCLUSTERED INDEX [IX_DirectProducerSubmissionHistory_AuthorisedRepresentativeId] ON [Producer].[DirectProducerSubmissionHistory] ([AuthorisedRepresentativeId]);

