GO
IF OBJECT_ID('Evidence.vwEvidenceSumByCategory', 'V') IS NOT NULL
	DROP VIEW Evidence.vwEvidenceSumByCategory;
GO
GO
IF OBJECT_ID('Evidence.vwEvidenceSumByCategoryByRecipient', 'V') IS NOT NULL
	DROP VIEW Evidence.vwEvidenceSumByCategoryByRecipient;
GO

CREATE VIEW Evidence.vwEvidenceSumByCategoryByRecipient AS
SELECT 
	nt.CategoryId AS CategoryId, 
	n.RecipientId AS ReceiverOrganisation,
	n.ComplianceYear as ComplianceYear,
	SUM(nt.Received) AS Received,
	SUM(nt.Reused) AS Reused
FROM 
	[Evidence].Note n
	LEFT JOIN [Evidence].NoteTonnage nt ON nt.NoteId = n.Id
WHERE
	n.Status = 3 AND 
	n.NoteType = 1 AND
	n.WasteType = 1
GROUP BY 
	nt.CategoryId,
	n.ComplianceYear,
	n.RecipientId