DECLARE @CountryId UNIQUEIDENTIFIER
SELECT @CountryId = Id FROM [Lookup].Country WHERE Name = 'UK - England'

DECLARE @AddressId UNIQUEIDENTIFIER
SET @AddressId = NEWID()

INSERT INTO [Organisation].Address (Id, Address1, TownOrCity, CountryId, Telephone, Email)
VALUES (@AddressId, 'N/A', 'N/A', @CountryId, '00000000', 'N/A')

INSERT INTO [Organisation].Organisation (Id, [Name], OrganisationType, OrganisationStatus, CompanyRegistrationNumber, BusinessAddressId)
VALUES ('911609B1-8057-4ADA-8CDD-67BB7836BF53', 'Producer compliance scheme balancing system (PBS)', 1, 2, '00000000', @AddressId)

INSERT INTO [Organisation].ProducerBalancingScheme (Lock, OrganisationId)
VALUES ('X', '911609B1-8057-4ADA-8CDD-67BB7836BF53')
GO