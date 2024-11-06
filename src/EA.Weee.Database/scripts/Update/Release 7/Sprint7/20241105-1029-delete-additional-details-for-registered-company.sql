;WITH deleteCte AS (SELECT 
acd.id
FROM [Organisation].[Organisation] o 
INNER JOIN [Producer].[DirectRegistrant] dr ON dr.OrganisationId = o.Id
INNER JOIN [Organisation].[AdditionalCompanyDetails] acd ON acd.DirectRegistrantId = dr.Id
WHERE o.[OrganisationType] = 1)
DELETE FROM [Organisation].[AdditionalCompanyDetails] WHERE id in (SELECT id FROM deleteCte)