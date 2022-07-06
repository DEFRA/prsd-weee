CREATE TABLE [Organisation].[ProducerBalancingScheme](
	[Lock] CHAR(1) NOT NULL,
	[OrganisationId] [uniqueidentifier] NOT NULL,
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

DECLARE @AddressId UNIQUEIDENTIFIER
SET @AddressId = NEWID()

INSERT INTO [Organisation].Address (Id, Address1, TownOrCity, CountryId, Telephone, Email)
VALUES (@AddressId, 'N/A', 'N/A', '184E1785-26B4-4AE4-80D3-AE319B103ACB', '00000000', 'N/A')

DECLARE @OrganisationId UNIQUEIDENTIFIER
SET @OrganisationId = NEWID()
INSERT INTO [Organisation].Organisation (Id, [Name], OrganisationType, OrganisationStatus, CompanyRegistrationNumber, BusinessAddressId)
VALUES (@OrganisationId, 'Producer compliance scheme balancing system (PBS)', 1, 2, '00000000', @AddressId)

INSERT INTO [Organisation].ProducerBalancingScheme (Lock, OrganisationId)
VALUES ('X', @OrganisationId)
GO
