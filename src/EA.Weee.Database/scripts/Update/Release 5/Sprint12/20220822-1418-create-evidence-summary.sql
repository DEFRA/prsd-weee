GO
IF OBJECT_ID('Evidence.vwEvidenceSummary', 'V') IS NOT NULL
	DROP VIEW Evidence.vwEvidenceSummary;
GO

CREATE VIEW [Evidence].[vwEvidenceSummary] AS
SELECT
		n.Id,
		n.NoteType,
		CASE WHEN n.NoteType = 1 THEN 'E' ELSE 'T' END + CAST(n.Reference AS NVARCHAR) AS Reference,
		s.ApprovalNumber,
		ews.[Name] AS WasteType,
		ep.[Name] AS Protocol,
		ens.[Name] AS NoteStatus,
		n.StartDate,
		n.EndDate,
		submittedHistory.ChangedDate AS SubmittedDate,
		CASE WHEN pbs.OrganisationId IS NULL THEN s.SchemeName ELSE o.[Name] END AS Recipient,
		s.Id AS RecipientSchemeId,
		n.OrganisationId AS OriginatingOrganisationId,
		n.RecipientId AS RecipientOrganisationId,
		n.AatfId,
		aa.[Name] AS AatfName,
		n.ComplianceYear
FROM
	[Evidence].Note n
	INNER JOIN [Lookup].EvidenceNoteStatus ens ON n.[Status] = ens.Id
	INNER JOIN [Lookup].EvidenceNoteWasteType ews ON n.WasteType = ews.Id
	INNER JOIN [Lookup].EvidenceNoteProtocol ep ON n.Protocol = ep.Id
	INNER JOIN [Organisation].Organisation o ON o.Id = n.RecipientId
	LEFT JOIN [PCS].Scheme s ON s.OrganisationId = o.Id
	LEFT JOIN [Organisation].ProducerBalancingScheme pbs ON pbs.OrganisationId = o.Id
	LEFT JOIN [AATF].AATF aa ON aa.Id = n.AatfId
	OUTER APPLY (SELECT TOP 1 * 
				FROM 
					[Evidence].NoteStatusHistory nsh 
				WHERE
					n.Id = nsh.NoteId
					AND nsh.ToStatus = 2
				ORDER BY
					nsh.ChangedDate DESC) as submittedHistory