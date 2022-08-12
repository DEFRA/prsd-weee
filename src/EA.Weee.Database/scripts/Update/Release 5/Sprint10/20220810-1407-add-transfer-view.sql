GO
IF OBJECT_ID('Evidence.vwTransferEvidenceByCategory', 'V') IS NOT NULL
	DROP VIEW Evidence.vwTransferEvidenceByCategory;
GO
	
CREATE VIEW Evidence.vwTransferEvidenceByCategory AS
SELECT 
	nt.CategoryId AS CategoryId, 
	n.OrganisationId AS TransferOrganisation,
	n.RecipientId AS ReceiverOrganisation,
	n.ComplianceYear as ComplianceYear,
	SUM(ntt.Received) AS TransferredReceived,
	SUM(ntt.Reused) AS TransferredReused
FROM 
	[Evidence].NoteTonnage nt
	LEFT JOIN [Evidence].NoteTransferTonnage ntt ON ntt.NoteTonnageId = nt.Id
	LEFT JOIN [Evidence].Note n ON n.Id = ntt.TransferNoteId
WHERE
	n.Status = 3 AND 
	n.NoteType = 2 AND
	n.WasteType = 1
GROUP BY 
	nt.CategoryId,
	n.ComplianceYear,
	n.OrganisationId,
	n.RecipientId