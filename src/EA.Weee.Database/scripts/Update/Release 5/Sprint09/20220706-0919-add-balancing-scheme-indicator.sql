CREATE TABLE [Organisation].[ProducerBalancingScheme](
	[Lock] CHAR(1) NOT NULL,
	[OrganisationId] [uniqueidentifier] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	CONSTRAINT PK_ProducerBalancingScheme_Lock PRIMARY KEY (Lock),
    CONSTRAINT CK_ProducerBalancingScheme_Locked CHECK (Lock='X')
)
GO
ALTER TABLE [Organisation].[ProducerBalancingScheme]  WITH CHECK ADD  CONSTRAINT [FK_ProducerBalancingScheme_OrganisationId] FOREIGN KEY([OrganisationId])
REFERENCES [Organisation].[Organisation] ([Id])
GO

ALTER TABLE [Organisation].[ProducerBalancingScheme] CHECK CONSTRAINT [FK_ProducerBalancingScheme_OrganisationId]
GO

CREATE NONCLUSTERED INDEX [IDX_ProducerBalancingScheme_OrganisationId] ON [Organisation].[ProducerBalancingScheme]
(
	[OrganisationId] ASC
)
GO