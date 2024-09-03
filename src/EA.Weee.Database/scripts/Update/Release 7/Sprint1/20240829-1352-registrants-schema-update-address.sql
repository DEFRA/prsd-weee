
ALTER TABLE [Producer].[DirectRegistrant] ADD [AddressId] [uniqueidentifier] NULL;
ALTER TABLE [Producer].[DirectRegistrant] WITH CHECK ADD CONSTRAINT [FK_DirectRegistrant_Address] 
    FOREIGN KEY([AddressId]) REFERENCES [Organisation].[Address] ([Id]);

CREATE NONCLUSTERED INDEX [IX_DirectRegistrant_AddressId] ON [Producer].[DirectRegistrant] ([AddressId]);