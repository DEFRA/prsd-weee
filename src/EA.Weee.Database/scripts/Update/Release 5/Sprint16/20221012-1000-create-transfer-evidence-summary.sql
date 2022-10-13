GO
IF OBJECT_ID('Evidence.vwTransferSummary', 'V') IS NOT NULL
	DROP VIEW Evidence.vwTransferSummary;
GO

CREATE VIEW [Evidence].[vwTransferSummary] AS
SELECT
		n.Id,
		n.NoteType,
		'T' + CAST(n.Reference AS NVARCHAR) AS Reference,
		n.Reference AS ReferenceId,
		ens.[Name] AS [Status],
		n.Status AS StatusId,
		approvedHistory.ChangedDate AS TransferApprovedDateTime,
		CASE WHEN pbs.OrganisationId IS NULL THEN (CASE WHEN n.ApprovedTransfererSchemeName IS NOT NULL THEN n.ApprovedTransfererSchemeName ELSE originatingScheme.SchemeName END) ELSE originatingOrganisation.[Name] END AS TransferredSchemeName,
		originatingScheme.ApprovalNumber AS TransferredApprovalNumber,
		CASE WHEN n.ApprovedRecipientSchemeName IS NOT NULL THEN n.ApprovedRecipientSchemeName ELSE recipientScheme.SchemeName END AS RecipientSchemeName,
		recipientScheme.ApprovalNumber AS RecipientApprovalNumber,
		originatingScheme.Id AS OriginatingSchemeId,
		recipientScheme.Id AS RecipientSchemeId,
		n.OrganisationId AS OriginatingOrganisationId,
		n.RecipientId AS RecipientOrganisationId,
		n.ComplianceYear
FROM
	[Evidence].Note n
	INNER JOIN [Lookup].EvidenceNoteStatus ens ON n.[Status] = ens.Id
	INNER JOIN [Organisation].[Organisation] originatingOrganisation ON originatingOrganisation.Id = n.OrganisationId
	LEFT JOIN [Organisation].ProducerBalancingScheme pbs ON pbs.OrganisationId = originatingOrganisation.Id
	LEFT JOIN [PCS].Scheme originatingScheme ON originatingScheme.OrganisationId = originatingOrganisation.Id
	INNER JOIN [Organisation].[Organisation] recipientOrganisation ON recipientOrganisation.Id = n.RecipientId
	INNER JOIN [PCS].Scheme recipientScheme ON recipientScheme.OrganisationId = recipientOrganisation.Id
	OUTER APPLY (SELECT TOP 1 * 
				FROM 
					[Evidence].NoteStatusHistory nsh 
				WHERE
					n.Id = nsh.NoteId
					AND nsh.ToStatus = 3
				ORDER BY
					nsh.ChangedDate DESC) as approvedHistory
WHERE
	n.NoteType = 2