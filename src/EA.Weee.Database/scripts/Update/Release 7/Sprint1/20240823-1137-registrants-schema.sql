
ALTER TABLE [Producer].[BrandName] ALTER COLUMN [ProducerSubmissionId] [uniqueidentifier] NULL;
ALTER TABLE [Producer].[SICCode] ALTER COLUMN [ProducerSubmissionId] [uniqueidentifier] NULL;
ALTER TABLE [Producer].[RegisteredProducer] ALTER COLUMN [SchemeId] [uniqueidentifier] NULL;
ALTER TABLE [Organisation].[Address] ALTER COLUMN [Telephone] [nvarchar](20) NULL;
ALTER TABLE [Organisation].[Address] ALTER COLUMN [Email] [nvarchar](256) NULL;
ALTER TABLE [Organisation].[Address] ADD [WebAddress] [nvarchar](256) NULL;
ALTER TABLE [Organisation].[OrganisationTransaction] ADD [OrganisationId] [uniqueidentifier] NULL;
ALTER TABLE [Organisation].[OrganisationTransaction] WITH CHECK ADD CONSTRAINT [FK_OrganisationTransaction_Organisation] 
    FOREIGN KEY([OrganisationId]) REFERENCES [Organisation].[Organisation] ([Id]);
CREATE NONCLUSTERED INDEX [IX_OrganisationTransaction_OrganisationId] ON [Organisation].[OrganisationTransaction] ([OrganisationId]);

CREATE TABLE [Producer].[DirectRegistrant] (
    [Id] [uniqueidentifier] NOT NULL,
    [OrganisationId] [uniqueidentifier] NOT NULL,
    [ContactId] [uniqueidentifier] NOT NULL,
    [SICCodeId] [uniqueidentifier] NULL,
    [BrandNameId] [uniqueidentifier] NULL,
    [RepresentingCompanyId] [uniqueidentifier] NULL,
    [RowVersion] [timestamp] NOT NULL
    CONSTRAINT [PK_DirectRegistrant] PRIMARY KEY CLUSTERED ([Id] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];

ALTER TABLE [Producer].[DirectRegistrant] WITH CHECK ADD CONSTRAINT [FK_DirectRegistrant_Organisation] 
    FOREIGN KEY([OrganisationId]) REFERENCES [Organisation].[Organisation] ([Id]);

ALTER TABLE [Producer].[DirectRegistrant] WITH CHECK ADD CONSTRAINT [FK_DirectRegistrant_Contact] 
    FOREIGN KEY([ContactId]) REFERENCES [Organisation].[Contact] ([Id]);

ALTER TABLE [Producer].[DirectRegistrant] WITH CHECK ADD CONSTRAINT [FK_DirectRegistrant_BrandName] 
    FOREIGN KEY([BrandNameId]) REFERENCES [Producer].[BrandName] ([Id]);

ALTER TABLE [Producer].[DirectRegistrant] WITH CHECK ADD CONSTRAINT [FK_DirectRegistrant_SICCode] 
    FOREIGN KEY([SICCodeId]) REFERENCES [Producer].[SICCode] ([Id]);

ALTER TABLE [Producer].[DirectRegistrant] WITH CHECK ADD CONSTRAINT [FK_DirectRegistrant_RepresentingCompany] 
    FOREIGN KEY([RepresentingCompanyId]) REFERENCES [Producer].[Business] ([Id]);

CREATE NONCLUSTERED INDEX [IX_DirectRegistrant_OrganisationId] ON [Producer].[DirectRegistrant] ([OrganisationId]);
CREATE NONCLUSTERED INDEX [IX_DirectRegistrant_ContactId] ON [Producer].[DirectRegistrant] ([ContactId]);
CREATE NONCLUSTERED INDEX [IX_DirectRegistrant_SICCodeId] ON [Producer].[DirectRegistrant] ([SICCodeId]);
CREATE NONCLUSTERED INDEX [IX_DirectRegistrant_BrandNameId] ON [Producer].[DirectRegistrant] ([BrandNameId]);
CREATE NONCLUSTERED INDEX [IX_DirectRegistrant_RepresentingCompanyId] ON [Producer].[DirectRegistrant] ([RepresentingCompanyId]);

CREATE TABLE [Organisation].[AdditionalCompanyDetails] (
    [Id] [uniqueidentifier] NOT NULL,
    [DirectRegistrantId] [uniqueidentifier] NOT NULL,
    [FirstName] [nvarchar](35) NOT NULL,
    [LastName] [nvarchar](35) NOT NULL,
    [Type] int NOT NULL,
    [RowVersion] [timestamp] NOT NULL
    CONSTRAINT [PK_AdditionalCompanyDetails] PRIMARY KEY CLUSTERED ([Id] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];

ALTER TABLE [Organisation].[AdditionalCompanyDetails] WITH CHECK ADD CONSTRAINT [FK_Organisation_AdditionalCompanyDetails] 
    FOREIGN KEY([DirectRegistrantId]) REFERENCES [Producer].[DirectRegistrant] ([Id]);

CREATE NONCLUSTERED INDEX [IX_Organisation_DirectRegistrantId] ON [Organisation].[AdditionalCompanyDetails] ([DirectRegistrantId]);

CREATE TABLE [Producer].[DirectProducerSubmission] (
    [Id] [uniqueidentifier] NOT NULL,
    [DirectRegistrantId] [uniqueidentifier] NOT NULL,
    [ServiceOfNoticeAddressId] [uniqueidentifier] NULL,
    [AppropriateSignatoryId] [uniqueidentifier] NULL,
    [CreatedDate] [datetime] NOT NULL,
    [UpdatedDate] [datetime] NULL,
    [CreatedById] [nvarchar](128) NOT NULL,
    [UpdatedById] [nvarchar](128) NULL,
    [PaymentStatus] [int] NULL,
    [SubmittedDate] [datetime] NULL,
    [PaymentReference] [nvarchar](20) NULL,
    [PaymentId] [nvarchar](20) NULL,
    [RowVersion] [timestamp] NOT NULL
    CONSTRAINT [PK_DirectProducerSubmission] PRIMARY KEY CLUSTERED ([Id] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];

ALTER TABLE [Producer].[DirectProducerSubmission] WITH CHECK ADD CONSTRAINT [FK_DirectProducerSubmission_ServiceOfNoticeAddress] 
    FOREIGN KEY([ServiceOfNoticeAddressId]) REFERENCES [Organisation].[Address] ([Id]);

    ALTER TABLE [Producer].[DirectProducerSubmission] WITH CHECK ADD CONSTRAINT [FK_DirectProducerSubmission_AppropriateSignatory] 
    FOREIGN KEY([AppropriateSignatoryId]) REFERENCES [Organisation].[Contact] ([Id]);

ALTER TABLE [Producer].[DirectProducerSubmission] WITH CHECK ADD CONSTRAINT [FK_DirectProducerSubmission_CreatedBy] 
    FOREIGN KEY([CreatedById]) REFERENCES [Identity].[AspNetUsers] ([Id]);

ALTER TABLE [Producer].[DirectProducerSubmission] WITH CHECK ADD CONSTRAINT [FK_DirectProducerSubmission_UpdatedBy] 
    FOREIGN KEY([UpdatedById]) REFERENCES [Identity].[AspNetUsers] ([Id]);

CREATE NONCLUSTERED INDEX [IX_DirectProducerSubmission_DirectRegistrantId] ON [Producer].[DirectProducerSubmission] ([DirectRegistrantId]);
CREATE NONCLUSTERED INDEX [IX_DirectProducerSubmission_ServiceOfNoticeAddressId] ON [Producer].[DirectProducerSubmission] ([ServiceOfNoticeAddressId]);
CREATE NONCLUSTERED INDEX [IX_DirectProducerSubmission_AppropriateSignatoryId] ON [Producer].[DirectProducerSubmission] ([AppropriateSignatoryId]);
CREATE NONCLUSTERED INDEX [IX_DirectProducerSubmission_CreatedById] ON [Producer].[DirectProducerSubmission] ([CreatedById]);
CREATE NONCLUSTERED INDEX [IX_DirectProducerSubmission_UpdatedById] ON [Producer].[DirectProducerSubmission] ([UpdatedById]);
