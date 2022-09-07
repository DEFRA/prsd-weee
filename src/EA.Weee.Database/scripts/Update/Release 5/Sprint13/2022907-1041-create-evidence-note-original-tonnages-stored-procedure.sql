IF OBJECT_ID('[Evidence].[getEvidenceNotesOriginalTonnage]', 'P') IS NOT NULL
	DROP PROC [Evidence].[getEvidenceNotesOriginalTonnage]
GO
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
		ens.[Name] AS [Status],
		ca.Abbreviation AS AppropriateAuthority,
		submittedHistory.ChangedDate AS SubmittedDateTime,
		aa.Name AS SubmittedByAatf,
		aa.ApprovalNumber AS AatfApprovalNumber,
		ews.[Name] AS ObligationType,
		n.StartDate AS ReceivedStartDate,
		n.EndDate AS ReceivedEndDate,
		CASE WHEN pbs.OrganisationId IS NULL THEN s.SchemeName ELSE recipientOrg.[Name] END AS Recipient,
		s.ApprovalNumber AS RecipientApprovalNumber,
		ep.[Name] AS Protocol,
		receivedCat.[1] AS Cat1Received,
		receivedCat.[2] AS Cat2Received,
		receivedCat.[3] AS Cat3Received,
		receivedCat.[4] AS Cat4Received,
		receivedCat.[5] AS Cat5Received,
		receivedCat.[6] AS Cat6Received,
		receivedCat.[7] AS Cat7Received,
		receivedCat.[8] AS Cat8Received,
		receivedCat.[9] AS Cat9Received,
		receivedCat.[10] AS Cat10Received,
		receivedCat.[11] AS Cat11Received,
		receivedCat.[12] AS Cat12Received,
		receivedCat.[13] AS Cat13Received,
		receivedCat.[14] AS Cat14Received,
		receivedCat.[1], receivedCat.[2] + receivedCat.[3]+ receivedCat.[4] + receivedCat.[5] + receivedCat.[6] + receivedCat.[7] + receivedCat.[8] + receivedCat.[8] + receivedCat.[10] + receivedCat.[11]
			+ receivedCat.[12] + receivedCat.[13] + receivedCat.[14] AS TotalReceived,
		reusedCat.[1] AS Cat1Reused, 
		reusedCat.[2] AS Cat2Reused,
		reusedCat.[3] AS Cat3Reused,
		reusedCat.[4] AS Cat4Reused,
		reusedCat.[5] AS Cat5Reused,
		reusedCat.[6] AS Cat6Reused,
		reusedCat.[7] AS Cat7Reused,
		reusedCat.[8] AS Cat8Reused,
		reusedCat.[9] AS Cat9Reused,
		reusedCat.[10] AS Cat10Reused,
		reusedCat.[11] AS Cat11Reused,
		reusedCat.[12] AS Cat12Reused,
		reusedCat.[13] AS Cat13Reused,
		reusedCat.[14] AS Cat14Reused,
		reusedCat.[1], reusedCat.[2] + reusedCat.[3]+ reusedCat.[4] + reusedCat.[5] + reusedCat.[6] + reusedCat.[7] + reusedCat.[8] + reusedCat.[8] + reusedCat.[10] + reusedCat.[11]
			+ reusedCat.[12] + reusedCat.[13] + reusedCat.[14] AS TotalReused
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
			CAST([1] AS DECIMAL(28, 3)) AS [1], CAST([2] AS DECIMAL(28, 3)) AS [2], CAST([3] AS DECIMAL(28, 3)) AS [3], CAST([4] AS DECIMAL(28, 3)) AS [4], 
				CAST([5] AS DECIMAL(28, 3)) AS [5], CAST([6] AS DECIMAL(28, 3)) AS [6], CAST([7] AS DECIMAL(28, 3)) AS [7],	CAST([8] AS DECIMAL(28, 3)) AS [8], 
				CAST([9] AS DECIMAL(28, 3)) AS [9], CAST([10] AS DECIMAL(28, 3)) AS [10], CAST([11] AS DECIMAL(28, 3)) AS [11], CAST([12] AS DECIMAL(28, 3)) AS [12], 
				CAST([13] AS DECIMAL(28, 3)) AS [13], CAST([14] AS DECIMAL(28, 3)) AS [14]
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
				CAST([1] AS DECIMAL(28, 3)) AS [1], CAST([2] AS DECIMAL(28, 3)) AS [2], CAST([3] AS DECIMAL(28, 3)) AS [3], CAST([4] AS DECIMAL(28, 3)) AS [4], 
				CAST([5] AS DECIMAL(28, 3)) AS [5], CAST([6] AS DECIMAL(28, 3)) AS [6], CAST([7] AS DECIMAL(28, 3)) AS [7],	CAST([8] AS DECIMAL(28, 3)) AS [8], 
				CAST([9] AS DECIMAL(28, 3)) AS [9], CAST([10] AS DECIMAL(28, 3)) AS [10], CAST([11] AS DECIMAL(28, 3)) AS [11], CAST([12] AS DECIMAL(28, 3)) AS [12], 
				CAST([13] AS DECIMAL(28, 3)) AS [13], CAST([14] AS DECIMAL(28, 3)) AS [14]
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
			n.Reference ASC

END
