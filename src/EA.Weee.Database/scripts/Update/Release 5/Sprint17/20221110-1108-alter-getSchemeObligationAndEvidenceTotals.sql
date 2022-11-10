
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [Evidence].[getSchemeObligationAndEvidenceTotals]
	@ComplianceYear SMALLINT,
	@SchemeId UNIQUEIDENTIFIER = NULL,
	@AppropriateAuthorityId UNIQUEIDENTIFIER = NULL,
	@OrganisationId UNIQUEIDENTIFIER = NULL
WITH RECOMPILE
AS

BEGIN
SET NOCOUNT ON;

IF OBJECT_ID('tempdb..#EvidenceSummaryWithTotals') IS NOT NULL 
BEGIN
	DROP TABLE #EvidenceSummaryWithTotals
END

CREATE TABLE #EvidenceSummaryWithTotals(
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	CategoryId INT NULL,
	CategoryName NVARCHAR(60),
	SchemeId UNIQUEIDENTIFIER NOT NULL,
	SchemeName NVARCHAR(MAX) NOT NULL,
	ApprovalNumber NVARCHAR(MAX) NOT NULL,
	OrganisationId UNIQUEIDENTIFIER NOT NULL,
	Obligation DECIMAL(28, 3),
	EvidenceReceivedInTotal DECIMAL(28, 3),
	EvidenceReuseInTotal DECIMAL(28, 3),
	NonHouseholdEvidenceReceivedInTotal DECIMAL(28, 3),
	NonHouseHoldEvidenceReuseInTotal DECIMAL(28, 3),
	TransferEvidenceReceivedIn DECIMAL(28, 3),
	TransferEvidenceReuseIn DECIMAL(28, 3),
	TransferEvidenceReceivedOut DECIMAL(28, 3),
	TransferEvidenceReuseOut DECIMAL(28, 3)
)

INSERT INTO #EvidenceSummaryWithTotals (CategoryId, CategoryName, SchemeId, SchemeName, ApprovalNumber, OrganisationId, Obligation, EvidenceReceivedInTotal, EvidenceReuseInTotal, 
TransferEvidenceReceivedIn, TransferEvidenceReuseIn, TransferEvidenceReceivedOut, TransferEvidenceReuseOut, NonHouseholdEvidenceReceivedInTotal, NonHouseHoldEvidenceReuseInTotal)

SELECT c.Id,
	CASE WHEN c.Id < 1000 THEN CAST(c.Id AS NVARCHAR) + '. ' + c.[Name] ELSE c.[Name] END,
	s.Id,
	CASE WHEN s.SchemeName IS NULL THEN ' ' + CAST(s.Id AS NVARCHAR(MAX)) ELSE s.SchemeName END,
	CASE WHEN s.ApprovalNumber IS NULL THEN ' ' + CAST(s.Id AS NVARCHAR(MAX)) ELSE s.ApprovalNumber END,
	s.OrganisationId,
	0, 
	0, 
	0,
	0, 
	0, 
	0,
	0, 
	0,
	0
FROM
(
	SELECT 
		*
	FROM
		Lookup.WeeeCategory c
	UNION ALL
	SELECT 
		1000 As Id,
		'Category 2-10 summary' AS NAME
	UNION ALL
	SELECT 
		1001 As Id,
		'Total (tonnes)' AS NAME
) c
CROSS JOIN [PCS].Scheme s
WHERE
	s.Id = @SchemeId OR (@SchemeId IS NULL AND 
							(s.Id IN (SELECT SchemeId FROM [PCS].ObligationScheme WHERE ComplianceYear = @ComplianceYear) 
							OR s.Id IN (SELECT 
											p.Id
										FROM [Evidence].Note n
											INNER JOIN [Organisation].Organisation o WITH (NOLOCK) ON o.Id = n.RecipientId
											INNER JOIN [PCS].Scheme p WITH (NOLOCK) ON p.OrganisationId = o.Id
										WHERE 
											n.ComplianceYear = @ComplianceYear
											AND n.NoteType = 1
										)
							OR s.Id IN (SELECT 
											p.Id
										FROM [Evidence].Note n
											INNER JOIN [Organisation].Organisation o WITH (NOLOCK) ON o.Id = n.OrganisationId
											INNER JOIN [PCS].Scheme p WITH (NOLOCK) ON p.OrganisationId = o.Id
										WHERE 
											n.ComplianceYear = @ComplianceYear
											AND n.NoteType = 2
											)
							OR s.Id IN (SELECT 
											p.Id
										FROM [Evidence].Note n
											INNER JOIN [Organisation].Organisation o WITH (NOLOCK) ON o.Id = n.RecipientId
											INNER JOIN [PCS].Scheme p WITH (NOLOCK) ON p.OrganisationId = o.Id
										WHERE 
											n.ComplianceYear = @ComplianceYear
											AND n.NoteType = 2
											)
						))
	AND (s.CompetentAuthorityId = @AppropriateAuthorityId OR @AppropriateAuthorityId IS NULL)
	AND (s.OrganisationId = @OrganisationId OR @OrganisationId IS NULL)
			
ORDER BY
	s.Id

UPDATE s
SET 
	s.Obligation = osa.Obligation
FROM
	#EvidenceSummaryWithTotals s
	LEFT JOIN [PCS].ObligationSchemeAmount osa WITH (NOLOCK) ON s.CategoryId = osa.CategoryId
	LEFT JOIN [PCS].ObligationScheme os WITH (NOLOCK) ON os.Id = osa.ObligationSchemeId AND os.SchemeId = s.SchemeId
	LEFT JOIN [PCS].Scheme sch WITH (NOLOCK) ON sch.Id = os.SchemeId 
WHERE
	os.ComplianceYear = @ComplianceYear
	AND (os.SchemeId = @SchemeId OR @SchemeId IS NULL)
	AND (sch.CompetentAuthorityId = @AppropriateAuthorityId OR @AppropriateAuthorityId IS NULL)
	AND (sch.OrganisationId = @OrganisationId OR @OrganisationId IS NULL)

UPDATE 
	s
SET
	s.EvidenceReceivedInTotal = COALESCE(evc.Received, 0),
	s.EvidenceReuseInTotal = COALESCE(evc.Reused, 0)
FROM
	#EvidenceSummaryWithTotals s
	INNER JOIN Evidence.vwHouseholdEvidenceSumByCategoryAndRecipient evc WITH (NOLOCK) ON evc.CategoryId = s.CategoryId AND evc.ComplianceYear = @ComplianceYear AND evc.ReceiverOrganisation = s.OrganisationId
	LEFT JOIN [PCS].Scheme sc WITH (NOLOCK) ON sc.OrganisationId = evc.ReceiverOrganisation
WHERE
	(sc.Id = @SchemeId OR @SchemeId IS NULL) 
	AND (sc.CompetentAuthorityId = @AppropriateAuthorityId OR @AppropriateAuthorityId IS NULL)
	AND (sc.OrganisationId = @OrganisationId OR @OrganisationId IS NULL)



UPDATE 
	s
SET
	s.NonHouseholdEvidenceReceivedInTotal = COALESCE(evc.Received, 0),
	s.NonHouseholdEvidenceReuseInTotal = COALESCE(evc.Reused, 0)
FROM
	#EvidenceSummaryWithTotals s
	INNER JOIN Evidence.vwNonHouseholdEvidenceSumByCategoryAndRecipient evc WITH (NOLOCK) ON evc.CategoryId = s.CategoryId AND evc.ComplianceYear = @ComplianceYear AND evc.ReceiverOrganisation = s.OrganisationId
	LEFT JOIN [PCS].Scheme sc WITH (NOLOCK) ON sc.OrganisationId = evc.ReceiverOrganisation
WHERE
	(sc.Id = @SchemeId OR @SchemeId IS NULL) 
	AND (sc.CompetentAuthorityId = @AppropriateAuthorityId OR @AppropriateAuthorityId IS NULL)
	AND (sc.OrganisationId = @OrganisationId OR @OrganisationId IS NULL)

UPDATE 
	s
SET
	s.TransferEvidenceReceivedIn = COALESCE(evc.TransferredReceived, 0),
	s.TransferEvidenceReuseIn = COALESCE(evc.TransferredReused, 0)
FROM
	#EvidenceSummaryWithTotals s
	INNER JOIN Evidence.vwTransferSumByCategoryByRecipient evc WITH (NOLOCK) ON evc.CategoryId = s.CategoryId AND evc.ComplianceYear = @ComplianceYear AND evc.ReceiverOrganisation = s.OrganisationId
	LEFT JOIN [PCS].Scheme sc WITH (NOLOCK) ON sc.OrganisationId = evc.ReceiverOrganisation
WHERE
	(sc.Id = @SchemeId OR @SchemeId IS NULL)
	AND (sc.CompetentAuthorityId = @AppropriateAuthorityId OR @AppropriateAuthorityId IS NULL)
	AND (sc.OrganisationId = @OrganisationId OR @OrganisationId IS NULL)

UPDATE 
	s
SET
	s.TransferEvidenceReceivedOut = COALESCE(evc.TransferredReceived, 0),
	s.TransferEvidenceReuseOut =  COALESCE(evc.TransferredReused, 0)
FROM
	#EvidenceSummaryWithTotals s
	INNER JOIN Evidence.vwTransferSumByCategoryByOriginator evc WITH (NOLOCK) ON evc.CategoryId = s.CategoryId AND evc.ComplianceYear = @ComplianceYear AND evc.TransferOrganisation = s.OrganisationId
	LEFT JOIN [PCS].Scheme sc WITH (NOLOCK) ON sc.OrganisationId = evc.TransferOrganisation
WHERE
	(sc.Id = @SchemeId OR @SchemeId IS NULL)
	AND (sc.CompetentAuthorityId = @AppropriateAuthorityId OR @AppropriateAuthorityId IS NULL)
	AND (sc.OrganisationId = @OrganisationId OR @OrganisationId IS NULL)

-- this is the category 2-10 totals
UPDATE
	s
SET
	s.EvidenceReceivedInTotal = cs.ReceivedTotal,
	s.EvidenceReuseInTotal = cs.ReuseTotal,
	s.NonHouseholdEvidenceReceivedInTotal = NULL,
	s.NonHouseHoldEvidenceReuseInTotal = NULL,
	s.TransferEvidenceReceivedOut = cs.TransferredOutTotal,
	s.TransferEvidenceReceivedIn = cs.TransferredInTotal,
	s.Obligation = cs.ObligationTotal,
	s.TransferEvidenceReuseIn = cs.TransferEvidenceReuseIn,
	s.TransferEvidenceReuseOut = cs.TransferEvidenceReuseOut
FROM
	#EvidenceSummaryWithTotals s
	INNER JOIN
		(SELECT
			SUM(s.Obligation) AS ObligationTotal,
			SUM(s.EvidenceReceivedInTotal) AS ReceivedTotal,
			SUM(s.EvidenceReuseInTotal) AS ReuseTotal,
			SUM(s.TransferEvidenceReceivedIn) As TransferredInTotal,
			SUM(s.TransferEvidenceReceivedOut) As TransferredOutTotal,
			SUM(s.TransferEvidenceReuseIn) AS TransferEvidenceReuseIn,
			SUM(s.TransferEvidenceReuseOut) AS TransferEvidenceReuseOut,
			s.SchemeId
		FROM
			#EvidenceSummaryWithTotals s
		WHERE
			s.CategoryId >= 2 AND s.CategoryId <= 10
		GROUP BY
			s.SchemeId) cs ON cs.SchemeId = s.SchemeId AND s.CategoryId = 1000
		
-- this is the total calculation
UPDATE
	s
SET
	s.EvidenceReceivedInTotal = cs.ReceivedTotal,
	s.EvidenceReuseInTotal = cs.ReuseTotal,
	s.NonHouseholdEvidenceReceivedInTotal = cs.NonHouseHoldReceivedTotal,
	s.NonHouseHoldEvidenceReuseInTotal = cs.NonHouseHoldReuseTotal,
	s.TransferEvidenceReceivedOut = cs.TransferredOutTotal,
	s.TransferEvidenceReceivedIn = cs.TransferredInTotal,
	s.Obligation = cs.ObligationTotal,
	s.TransferEvidenceReuseIn = cs.TransferEvidenceReuseIn,
	s.TransferEvidenceReuseOut = cs.TransferEvidenceReuseOut
FROM
	#EvidenceSummaryWithTotals s
	INNER JOIN
		(SELECT
			SUM(s.Obligation) AS ObligationTotal,
			SUM(s.EvidenceReceivedInTotal) AS ReceivedTotal,
			SUM(s.EvidenceReuseInTotal) AS ReuseTotal,
			SUM(s.NonHouseholdEvidenceReceivedInTotal) AS NonHouseHoldReceivedTotal,
			SUM(s.NonHouseHoldEvidenceReuseInTotal) AS NonHouseHoldReuseTotal,
			SUM(s.TransferEvidenceReceivedIn) As TransferredInTotal,
			SUM(s.TransferEvidenceReceivedOut) As TransferredOutTotal,
			SUM(s.TransferEvidenceReuseIn) AS TransferEvidenceReuseIn,
			SUM(s.TransferEvidenceReuseOut) AS TransferEvidenceReuseOut,
			s.SchemeId
		FROM
			#EvidenceSummaryWithTotals s
		WHERE
			s.CategoryId >= 1 AND s.CategoryId <= 14
		GROUP BY
			s.SchemeId) cs ON cs.SchemeId = s.SchemeId AND s.CategoryId = 1001
	
SELECT * FROM (
SELECT 
	CategoryId,
	CategoryName,
	NULL AS SchemeId,
	'All producer compliance schemes' AS SchemeName,
	'' AS ApprovalNumber,
	SUM(COALESCE(Obligation, 0)) AS Obligation,
	SUM(COALESCE(EvidenceReceivedInTotal, 0) + (COALESCE(TransferEvidenceReceivedIn, 0) - COALESCE(TransferEvidenceReceivedOut, 0))) AS Evidence,
	SUM(COALESCE(EvidenceReuseInTotal, 0) + (COALESCE(TransferEvidenceReuseIn, 0) - COALESCE(TransferEvidenceReuseOut, 0))) AS Reuse,
	CASE CategoryId WHEN 1000 THEN NULL ELSE SUM(COALESCE(NonHouseholdEvidenceReceivedInTotal, 0)) END AS NonHouseHoldEvidence,
	CASE CategoryId WHEN 1000 THEN NULL ELSE SUM(COALESCE(NonHouseHoldEvidenceReuseInTotal, 0)) END AS NonHouseHoldEvidenceReuse,
	SUM(COALESCE(TransferEvidenceReceivedOut, 0)) AS TransferredOut,
	SUM(COALESCE(TransferEvidenceReceivedIn, 0)) AS TransferredIn,
	SUM((COALESCE(EvidenceReceivedInTotal, 0) + (COALESCE(TransferEvidenceReceivedIn, 0) - COALESCE(TransferEvidenceReceivedOut, 0))) - COALESCE(Obligation, 0)) AS ObligationDifference
FROM 
	#EvidenceSummaryWithTotals
WHERE 
	@SchemeId IS NULL AND @OrganisationId IS NULL
GROUP BY
	CategoryId,
	CategoryName

UNION ALL

SELECT 
	CategoryId,
	CategoryName,
	SchemeId,
	SchemeName,
	ApprovalNumber,
	COALESCE(Obligation, 0) AS Obligation,
	COALESCE(EvidenceReceivedInTotal, 0) + (COALESCE(TransferEvidenceReceivedIn, 0) - COALESCE(TransferEvidenceReceivedOut, 0)) AS Evidence,
	COALESCE(EvidenceReuseInTotal, 0) + (COALESCE(TransferEvidenceReuseIn, 0) - COALESCE(TransferEvidenceReuseOut, 0)) AS Reuse,
	CASE st.CategoryId WHEN 1000 THEN NULL ELSE COALESCE(NonHouseholdEvidenceReceivedInTotal, 0) END AS NonHouseHoldEvidence,
	CASE st.CategoryId WHEN 1000 THEN NULL ELSE COALESCE(NonHouseHoldEvidenceReuseInTotal, 0) END AS NonHouseHoldEvidenceReuse,
	COALESCE(TransferEvidenceReceivedOut, 0) AS TransferredOut,
	COALESCE(TransferEvidenceReceivedIn, 0) AS TransferredIn,
	(COALESCE(EvidenceReceivedInTotal, 0) + (COALESCE(TransferEvidenceReceivedIn, 0) - COALESCE(TransferEvidenceReceivedOut, 0))) - COALESCE(Obligation, 0) AS ObligationDifference
FROM 
	#EvidenceSummaryWithTotals st

) x
ORDER BY
	CASE SchemeName WHEN 'All producer compliance schemes' THEN 0 ELSE 1 END,
	SchemeName,
	CategoryId

END
