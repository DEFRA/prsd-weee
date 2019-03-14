
SELECT
	 m.Id
INTO 
	#MemberUploadsId
FROM
	[PCS].[MemberUpload] m
	INNER JOIN [Organisation].Organisation o ON o.Id = m.OrganisationId
	INNER JOIN [PCS].[Scheme] s ON s.OrganisationId = o.Id
	INNER JOIN [Lookup].CompetentAuthority c ON c.Id = s.CompetentAuthorityId 
WHERE
	c.Abbreviation = 'EA'
	AND m.ComplianceYear = 2019
	AND m.IsSubmitted = 1

UPDATE [Producer].ProducerSubmission SET Invoiced = 0 WHERE MemberUploadId IN (SELECT * FROM #MemberUploadsId)
UPDATE [PCS].[MemberUpload] SET InvoiceRunId = NULL WHERE Id IN (SELECT * FROM #MemberUploadsId)

UPDATE [Charging].IbisFileData
SET 
	CustomerFileData = '',
	TransactionFileData = ''
WHERE
	FileId = ''

DROP TABLE #MemberUploadsId

