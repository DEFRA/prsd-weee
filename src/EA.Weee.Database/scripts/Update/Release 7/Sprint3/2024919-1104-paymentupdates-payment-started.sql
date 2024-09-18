CREATE TABLE [Producer].[PaymentSession] (
    [Id] [uniqueidentifier] NOT NULL,
    [UserId] NVARCHAR(128) NOT NULL,
    [DirectRegistrantId] [uniqueidentifier] NOT NULL,
    [DirectProducerSubmissionId] [uniqueidentifier] NOT NULL,
    [PaymentId] NVARCHAR(35) NOT NULL,
    [PaymentReference] NVARCHAR(20) NOT NULL,
    [PaymentReturnToken] NVARCHAR(150) NOT NULL,
    [Amount] DECIMAL(18,2) NOT NULL,
    [Status] INT NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedById] NVARCHAR(128) NULL,
    [RowVersion]		ROWVERSION       NOT NULL,
    CONSTRAINT [PK_PaymentSession] PRIMARY KEY CLUSTERED ([Id] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];

ALTER TABLE [Producer].[PaymentSession] WITH CHECK ADD CONSTRAINT [FK_PaymentSession_UserId] 
    FOREIGN KEY([UserId]) REFERENCES [Identity].[AspNetUsers] ([Id]);

CREATE NONCLUSTERED INDEX [IX_PaymentSession_UserId] ON [Producer].[PaymentSession] ([UserId]);

ALTER TABLE [Producer].[PaymentSession] WITH CHECK ADD CONSTRAINT [FK_PaymentSession_DirectRegistrantId] 
    FOREIGN KEY([DirectRegistrantId]) REFERENCES [Producer].[DirectRegistrant] ([Id]);

CREATE NONCLUSTERED INDEX [IX_PaymentSession_DirectRegistrantId] ON [Producer].[PaymentSession] ([DirectRegistrantId]);

ALTER TABLE [Producer].[PaymentSession] WITH CHECK ADD CONSTRAINT [FK_PaymentSession_DirectProducerSubmissionId] 
    FOREIGN KEY([DirectProducerSubmissionId]) REFERENCES [Producer].[DirectProducerSubmission] ([Id]);

CREATE NONCLUSTERED INDEX [IX_PaymentSession_DirectProducerSubmissionId] ON [Producer].[PaymentSession] ([DirectProducerSubmissionId]);

ALTER TABLE [Producer].[PaymentSession] WITH CHECK ADD CONSTRAINT [FK_PaymentSession_UpdatedById] 
    FOREIGN KEY([UpdatedById]) REFERENCES [Identity].[AspNetUsers] ([Id]);

CREATE NONCLUSTERED INDEX [IX_PaymentSession_UpdatedById] ON [Producer].[PaymentSession] ([UpdatedById]);