IF OBJECT_ID('[Producer].[DirectProducerSubmissionHistory]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [Producer].[DirectProducerSubmissionHistory]
END
CREATE TABLE [Producer].[DirectProducerSubmissionHistory] (
    [Id] [uniqueidentifier] NOT NULL,
    [DirectProducerSubmissionId] [uniqueidentifier] NOT NULL,
    [ServiceOfNoticeAddressId] [uniqueidentifier] NULL,
    [AppropriateSignatoryId] [uniqueidentifier] NULL,
    [EeeOutputReturnVersionId] [uniqueidentifier] NULL,
    [CompanyName] [nvarchar](256) NULL,
    [TradingName] [nvarchar](256) NULL,
    [CompanyRegistrationNumber] [nvarchar](15) NULL,
    [Status] [int] NOT NULL,
    [SubmittedDate] [datetime] NULL,
    [CreatedDate] [datetime] NOT NULL,
    [UpdatedDate] [datetime] NULL,
    [CreatedById] [nvarchar](128) NOT NULL,
    [UpdatedById] [nvarchar](128) NULL,
    [RowVersion] [timestamp] NOT NULL
    CONSTRAINT [PK_DirectProducerSubmissionHistory] PRIMARY KEY CLUSTERED ([Id] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];

ALTER TABLE [Producer].[DirectProducerSubmissionHistory] WITH CHECK ADD CONSTRAINT [FK_DirectProducerSubmissionHistory_DirectProducerSubmission] 
    FOREIGN KEY([DirectProducerSubmissionId]) REFERENCES [Producer].[DirectProducerSubmission] ([Id]);
CREATE NONCLUSTERED INDEX [IX_DirectProducerSubmissionHistory_DirectProducerSubmissionId] ON [Producer].[DirectProducerSubmissionHistory] ([DirectProducerSubmissionId]);

ALTER TABLE [Producer].[DirectProducerSubmissionHistory] WITH CHECK ADD CONSTRAINT [FK_DirectProducerSubmissionHistory_ServiceOfNoticeAddress] 
    FOREIGN KEY([ServiceOfNoticeAddressId]) REFERENCES [Organisation].[Address] ([Id]);
CREATE NONCLUSTERED INDEX [IX_DirectProducerSubmissionHistory_ServiceOfNoticeAddressId] ON [Producer].[DirectProducerSubmissionHistory] ([ServiceOfNoticeAddressId]);

ALTER TABLE [Producer].[DirectProducerSubmissionHistory] WITH CHECK ADD CONSTRAINT [FK_DirectProducerSubmissionHistory_AppropriateSignatoryId] 
    FOREIGN KEY([AppropriateSignatoryId]) REFERENCES [Organisation].[Contact] ([Id]);
CREATE NONCLUSTERED INDEX [IX_DirectProducerSubmissionHistory_AppropriateSignatoryId] ON [Producer].[DirectProducerSubmissionHistory] ([AppropriateSignatoryId]);

ALTER TABLE [Producer].[DirectProducerSubmissionHistory] WITH CHECK ADD CONSTRAINT [FK_DirectProducerSubmissionHistory_EeeOutputReturnVersionId] 
    FOREIGN KEY([EeeOutputReturnVersionId]) REFERENCES [PCS].[EeeOutputReturnVersion] ([Id]);
CREATE NONCLUSTERED INDEX [IX_DirectProducerSubmissionHistory_EeeOutputReturnVersionId] ON [Producer].[DirectProducerSubmissionHistory] ([EeeOutputReturnVersionId]);

ALTER TABLE [Producer].[DirectProducerSubmissionHistory] WITH CHECK ADD CONSTRAINT [FK_DirectProducerSubmissionHistory_UpdatedBy] 
    FOREIGN KEY([UpdatedById]) REFERENCES [Identity].[AspNetUsers] ([Id]);
CREATE NONCLUSTERED INDEX [IX_DirectProducerSubmissionHistory_UpdatedById] ON [Producer].[DirectProducerSubmissionHistory] ([UpdatedById]);

ALTER TABLE [Producer].[DirectProducerSubmissionHistory] WITH CHECK ADD CONSTRAINT [FK_DirectProducerSubmissionHistory_CreatedBy] 
    FOREIGN KEY([CreatedById]) REFERENCES [Identity].[AspNetUsers] ([Id]);
CREATE NONCLUSTERED INDEX [IX_DirectProducerSubmissionHistory_CreatedById] ON [Producer].[DirectProducerSubmissionHistory] ([CreatedById]);


ALTER TABLE [Producer].[DirectProducerSubmission] ADD [CurrentSubmissionId] [uniqueidentifier] NULL;
ALTER TABLE [Producer].[DirectProducerSubmission] WITH CHECK ADD CONSTRAINT [FK_DirectProducerSubmission_CurrentSubmission] 
    FOREIGN KEY([CurrentSubmissionId]) REFERENCES [Producer].[DirectProducerSubmissionHistory] ([Id]);

CREATE NONCLUSTERED INDEX [IX_DirectProducerSubmission_CurrentSubmissionId] ON [Producer].[DirectProducerSubmission] ([CurrentSubmissionId]);


