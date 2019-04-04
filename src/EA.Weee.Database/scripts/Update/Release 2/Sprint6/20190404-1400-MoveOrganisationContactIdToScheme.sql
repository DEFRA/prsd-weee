ALTER TABLE [PCS].Scheme ADD ContactId UNIQUEIDENTIFIER NULL CONSTRAINT FK_Scheme_Contact FOREIGN KEY REFERENCES [Organisation].[Contact]
GO
UPDATE
	s
SET
	s.ContactId = o.ContactId
FROM
	[PCS].Scheme s
	INNER JOIN [Organisation].Organisation o ON o.Id = s.OrganisationId
GO
ALTER TABLE [Organisation].[Organisation] DROP FK_Organisation_Contact
GO
ALTER TABLE [Organisation].[Organisation] DROP COLUMN ContactId
GO