	GO
IF OBJECT_ID('Evidence.vwTransferSumByCategoryByOriginator', 'V') IS NOT NULL
	DROP VIEW Evidence.vwTransferSumByCategoryByOriginator;
GO

CREATE VIEW Evidence.vwTransferSumByCategoryByOriginator AS
SELECT 
	nt.CategoryId AS CategoryId, 
	n.OrganisationId AS TransferOrganisation,
	n.ComplianceYear as ComplianceYear,
	SUM(ntt.Received) AS TransferredReceived,
	SUM(ntt.Reused) AS TransferredReused
FROM 
	[Evidence].Note n
	LEFT JOIN [Evidence].NoteTransferTonnage ntt ON n.Id = ntt.TransferNoteId
	LEFT JOIN [Evidence].NoteTonnage nt ON nt.Id = ntt.NoteTonnageId	
WHERE
	n.Status = 3 AND 
	n.NoteType = 2 AND
	n.WasteType = 1
GROUP BY 
	nt.CategoryId,
	n.ComplianceYear,
	n.OrganisationId