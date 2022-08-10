GO
IF OBJECT_ID('Evidence.vwEvidenceByCategory', 'V') IS NOT NULL
	DROP VIEW Evidence.vwEvidenceByCategory;
GO
	
CREATE VIEW Evidence.vwEvidenceByCategory AS
SELECT 
	nt.CategoryId AS CategoryId, 
	n.OrganisationId AS TransferOrganisation,
	n.RecipientId AS ReceiverOrganisation,
	n.ComplianceYear as ComplianceYear,
	SUM(nt.Received) AS Received,
	SUM(nt.Reused) AS Reused
FROM 
	[Evidence].NoteTonnage nt
	LEFT JOIN [Evidence].Note n ON n.Id = nt.NoteId
WHERE
	n.Status = 3 AND 
	n.NoteType = 2 AND
	n.WasteType = 1
GROUP BY 
	nt.CategoryId,
	n.ComplianceYear,
	n.OrganisationId,
	n.RecipientId