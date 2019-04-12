ALTER TABLE [PCS].Scheme ADD AddressId UNIQUEIDENTIFIER NULL CONSTRAINT FK_Scheme_Address FOREIGN KEY REFERENCES [Organisation].[Address]
GO
UPDATE
	s
SET
	s.AddressId = o.OrganisationAddressId
FROM
	[PCS].Scheme s
	INNER JOIN [Organisation].Organisation o ON o.Id = s.OrganisationId
GO
ALTER TABLE [Organisation].[Organisation] DROP FK_Organisation_OrganisationAddress
GO
ALTER TABLE [Organisation].[Organisation] DROP COLUMN OrganisationAddressId
GO