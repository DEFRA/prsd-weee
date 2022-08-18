GO
IF OBJECT_ID('Evidence.vwTransferEvidenceByCategory', 'V') IS NOT NULL
	DROP VIEW Evidence.vwTransferEvidenceByCategory;
GO


CREATE VIEW Evidence.vwTransferEvidenceByCategory AS
SELECT 
	n.Id AS NoteId,
	nt.CategoryId AS CategoryId, 
	n.OrganisationId AS TransferOrganisation,
	n.RecipientId AS ReceiverOrganisation,
	n.ComplianceYear as ComplianceYear,
	ntt.Received AS TransferredReceived,
	ntt.Reused AS TransferredReused
FROM 
	[Evidence].Note n
	LEFT JOIN [Evidence].NoteTransferTonnage ntt ON n.Id = ntt.TransferNoteId
	LEFT JOIN [Evidence].NoteTonnage nt ON nt.Id = ntt.NoteTonnageId	
WHERE
	n.Status = 3 AND 
	n.NoteType = 2 AND
	n.WasteType = 1
