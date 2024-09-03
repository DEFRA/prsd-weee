DROP INDEX [IX_DirectRegistrant_RepresentingCompanyId] ON [Producer].[DirectRegistrant];
ALTER TABLE [Producer].[DirectRegistrant] DROP CONSTRAINT [FK_DirectRegistrant_RepresentingCompany];
ALTER TABLE [Producer].[DirectRegistrant] DROP COLUMN [RepresentingCompanyId];

ALTER TABLE [Producer].[DirectRegistrant] ADD [AuthorisedRepresentativeId] [uniqueidentifier] NULL;
ALTER TABLE [Producer].[DirectRegistrant] WITH CHECK ADD CONSTRAINT [FK_DirectRegistrant_AuthorisedRepresentative] 
    FOREIGN KEY([AuthorisedRepresentativeId]) REFERENCES [Producer].[AuthorisedRepresentative] ([Id]);

ALTER TABLE [Producer].[AuthorisedRepresentative] ADD [TradingName] [nvarchar](256) NULL;

CREATE NONCLUSTERED INDEX [IX_DirectRegistrant_AuthorisedRepresentativeId] ON [Producer].[DirectRegistrant] ([AuthorisedRepresentativeId]);