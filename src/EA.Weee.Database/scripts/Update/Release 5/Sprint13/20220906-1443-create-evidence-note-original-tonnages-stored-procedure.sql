
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Evidence].[getEvidenceNotesOriginalTonnage]
	@ComplianceYear SMALLINT,
	@OriginatingOrganisationId UNIQUEIDENTIFIER = NULL,
	@RecipientOrganisationId UNIQUEIDENTIFIER = NULL
AS
BEGIN
SET NOCOUNT ON;

SELECT
		CASE WHEN n.NoteType = 1 THEN 'E' ELSE 'T' END + CAST(n.Reference AS NVARCHAR) AS Reference,
		ens.[Name] AS NoteStatus,
		ca.Abbreviation AS AppropriateAuthority,
		submittedHistory.ChangedDate AS SubmittedDate,
		aa.Name AS AatfName,
		aa.ApprovalNumber AS AatfApprovalNumber,
		ews.[Name] AS ObligationType,
		n.StartDate AS ReceivedStartDate,
		n.EndDate AS ReceivedEndDate,
		CASE WHEN pbs.OrganisationId IS NULL THEN s.SchemeName ELSE recipientOrg.[Name] END AS Recipient,
		s.ApprovalNumber AS RecipientApprovalNumber,
		ep.[Name] AS Protocol,
		receivedCat.[1],
		receivedCat.[2],
		receivedCat.[3],
		receivedCat.[4],
		receivedCat.[5],
		receivedCat.[6],
		receivedCat.[7],
		receivedCat.[8],
		receivedCat.[9],
		receivedCat.[10],
		receivedCat.[11],
		receivedCat.[12],
		receivedCat.[13],
		receivedCat.[14],
		receivedCat.[1], receivedCat.[2] + receivedCat.[3]+ receivedCat.[4] + receivedCat.[5] + receivedCat.[6] + receivedCat.[7] + receivedCat.[8] + receivedCat.[8] + receivedCat.[10] + receivedCat.[11]
			+ receivedCat.[12] + receivedCat.[13] + receivedCat.[14] AS ReceivedTotal,
		reusedCat.[1], 
		reusedCat.[2],
		reusedCat.[3],
		reusedCat.[4],
		reusedCat.[5],
		reusedCat.[6],
		reusedCat.[7],
		reusedCat.[8],
		reusedCat.[9],
		reusedCat.[10],
		reusedCat.[11],
		reusedCat.[12],
		reusedCat.[13],
		reusedCat.[14],
		reusedCat.[1], reusedCat.[2] + reusedCat.[3]+ reusedCat.[4] + reusedCat.[5] + reusedCat.[6] + reusedCat.[7] + reusedCat.[8] + reusedCat.[8] + reusedCat.[10] + reusedCat.[11]
			+ reusedCat.[12] + reusedCat.[13] + reusedCat.[14] AS ReusedTotal
FROM
	[Evidence].Note n
	INNER JOIN [Lookup].EvidenceNoteStatus ens ON n.[Status] = ens.Id
	INNER JOIN [Lookup].EvidenceNoteWasteType ews ON n.WasteType = ews.Id
	INNER JOIN [Lookup].EvidenceNoteProtocol ep ON n.Protocol = ep.Id
	INNER JOIN [Organisation].Organisation recipientOrg ON recipientOrg.Id = n.RecipientId
	INNER JOIN [Organisation].Organisation originatingOrg ON originatingOrg.Id = n.OrganisationId
	INNER JOIN [AATF].AATF aa ON aa.Id = n.AatfId
	INNER JOIN [Lookup].CompetentAuthority ca ON ca.Id = aa.CompetentAuthorityId
	LEFT JOIN [PCS].Scheme s ON s.OrganisationId = recipientOrg.Id
	LEFT JOIN [Organisation].ProducerBalancingScheme pbs ON pbs.OrganisationId = recipientOrg.Id
	OUTER APPLY (SELECT TOP 1 * 
					FROM 
					[Evidence].NoteStatusHistory nsh 
					WHERE
					n.Id = nsh.NoteId
					AND nsh.ToStatus = 2
					ORDER BY
					nsh.ChangedDate DESC) as submittedHistory
	CROSS APPLY
		(
		SELECT
			pvt.NoteId,
			[1], [2], [3], [4], [5], [6], [7],	[8], [9], [10], [11], [12], [13], [14]
		FROM
				(SELECT
					COALESCE(nt.Received, 0) AS Received,
					n1.Id AS NoteId,
					nt.CategoryId as Category
				 FROM    
					[Evidence].NoteTonnage nt
					INNER JOIN [Evidence].Note n1 ON n1.Id = nt.NoteId 
				WHERE 
					nt.NoteId = n.Id
				GROUP BY
					n1.Id,
					nt.CategoryId,
					nt.Received
				) AS t
				PIVOT
				(   AVG(t.Received) FOR Category IN ([1], [2], [3], [4], [5], [6], [7],	[8], [9], [10], [11], [12], [13], [14])
				) pvt
			) receivedCat 
	CROSS APPLY
			(
			SELECT
				pvt.NoteId,
				[1], [2], [3], [4], [5], [6], [7],	[8], [9], [10], [11], [12], [13], [14]
			FROM
					(SELECT
						COALESCE(nt.Reused, 0) AS Reused,
						n1.Id AS NoteId,
						nt.CategoryId as Category
					 FROM 
						[Evidence].NoteTonnage nt
						INNER JOIN [Evidence].Note n1 ON n1.Id = nt.NoteId
					WHERE 
						nt.NoteId = n.Id
					GROUP BY
						n1.Id,
						nt.CategoryId,
						nt.Reused
					) AS t
					PIVOT
					(   AVG(t.Reused) FOR Category IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12], [13], [14])
					) pvt
				) reusedCat 
		WHERE
		(n.ComplianceYear = @ComplianceYear) AND
		(
			(@OriginatingOrganisationId IS NULL OR originatingOrg.Id = @OriginatingOrganisationId) AND
			(@RecipientOrganisationId IS NULL OR recipientOrg.Id = @RecipientOrganisationId)
		)
		ORDER BY
			n.Reference DESC

END
