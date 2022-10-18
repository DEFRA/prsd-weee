
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

INSERT INTO #EvidenceSummaryWithTotals (CategoryId, CategoryName, SchemeId, OrganisationId, Obligation, EvidenceReceivedInTotal, EvidenceReuseInTotal, 
TransferEvidenceReceivedIn, TransferEvidenceReuseIn, TransferEvidenceReceivedOut, TransferEvidenceReuseOut)

SELECT c.Id,
	CASE WHEN c.Id < 1000 THEN CAST(c.Id AS NVARCHAR) + '. ' + c.[Name] ELSE c.[Name] END,
	s.Id,
	s.OrganisationId,
	NULL, 
	NULL, 
	NULL,
	NULL, 
	NULL, 
	NULL,
	NULL 
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
	s.Id = @SchemeId OR @SchemeId IS NULL
	AND (s.CompetentAuthorityId = @AppropriateAuthorityId OR @AppropriateAuthorityId IS NULL)
	AND (s.OrganisationId = @OrganisationId OR @OrganisationId IS NULL)

UPDATE s
SET 
	s.Obligation = osa.Obligation
FROM
	#EvidenceSummaryWithTotals s
	LEFT JOIN [PCS].ObligationSchemeAmount osa ON s.CategoryId = osa.CategoryId
	LEFT JOIN [PCS].ObligationScheme os ON os.Id = osa.ObligationSchemeId AND os.SchemeId = s.SchemeId
	LEFT JOIN [PCS].Scheme sch ON sch.Id = os.SchemeId 
WHERE
	os.ComplianceYear = @ComplianceYear
	AND (os.SchemeId = @SchemeId OR @SchemeId IS NULL)
	AND (sch.CompetentAuthorityId = @AppropriateAuthorityId OR @AppropriateAuthorityId IS NULL)
	AND (sch.OrganisationId = @OrganisationId OR @OrganisationId IS NULL)

UPDATE 
	s
SET
	s.EvidenceReceivedInTotal = evc.Received,
	s.EvidenceReuseInTotal = evc.Reused
FROM
	#EvidenceSummaryWithTotals s
	INNER JOIN Evidence.vwHouseholdEvidenceSumByCategoryAndRecipient evc WITH (NOLOCK) ON evc.CategoryId = s.CategoryId AND evc.ComplianceYear = @ComplianceYear AND evc.ReceiverOrganisation = s.OrganisationId
	LEFT JOIN [PCS].Scheme sc ON sc.OrganisationId = evc.ReceiverOrganisation
WHERE
	(sc.Id = @SchemeId OR @SchemeId IS NULL) 
	AND (sc.CompetentAuthorityId = @AppropriateAuthorityId OR @AppropriateAuthorityId IS NULL)
	AND (sc.OrganisationId = @OrganisationId OR @OrganisationId IS NULL)

UPDATE 
	s
SET
	s.NonHouseholdEvidenceReceivedInTotal = evc.Received,
	s.NonHouseholdEvidenceReuseInTotal = evc.Reused
FROM
	#EvidenceSummaryWithTotals s
	INNER JOIN Evidence.vwNonHouseholdEvidenceSumByCategoryAndRecipient evc WITH (NOLOCK) ON evc.CategoryId = s.CategoryId AND evc.ComplianceYear = @ComplianceYear AND evc.ReceiverOrganisation = s.OrganisationId
	LEFT JOIN [PCS].Scheme sc ON sc.OrganisationId = evc.ReceiverOrganisation
WHERE
	(sc.Id = @SchemeId OR @SchemeId IS NULL) 
	AND (sc.CompetentAuthorityId = @AppropriateAuthorityId OR @AppropriateAuthorityId IS NULL)
	AND (sc.OrganisationId = @OrganisationId OR @OrganisationId IS NULL)

UPDATE 
	s
SET
	s.TransferEvidenceReceivedIn = evc.TransferredReceived,
	s.TransferEvidenceReuseIn =  evc.TransferredReused
FROM
	#EvidenceSummaryWithTotals s
	INNER JOIN Evidence.vwTransferSumByCategoryByRecipient evc WITH (NOLOCK) ON evc.CategoryId = s.CategoryId AND evc.ComplianceYear = @ComplianceYear AND evc.ReceiverOrganisation = s.OrganisationId
	LEFT JOIN [PCS].Scheme sc ON sc.OrganisationId = evc.ReceiverOrganisation
WHERE
	(sc.Id = @SchemeId OR @SchemeId IS NULL)
	AND (sc.CompetentAuthorityId = @AppropriateAuthorityId OR @AppropriateAuthorityId IS NULL)
	AND (sc.OrganisationId = @OrganisationId OR @OrganisationId IS NULL)

UPDATE 
	s
SET
	s.TransferEvidenceReceivedOut = evc.TransferredReceived,
	s.TransferEvidenceReuseOut =  evc.TransferredReused
FROM
	#EvidenceSummaryWithTotals s
	INNER JOIN Evidence.vwTransferSumByCategoryByOriginator evc WITH (NOLOCK) ON evc.CategoryId = s.CategoryId AND evc.ComplianceYear = @ComplianceYear AND evc.TransferOrganisation = s.OrganisationId
	LEFT JOIN [PCS].Scheme sc ON sc.OrganisationId = evc.TransferOrganisation
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
	s.NonHouseholdEvidenceReceivedInTotal = cs.NonHouseHoldReceivedTotal,
	s.NonHouseHoldEvidenceReuseInTotal = cs.NonHouseHoldReuseTotal,
	s.TransferEvidenceReceivedOut = cs.TransferredOutTotal,
	s.TransferEvidenceReceivedIn = cs.TransferredInTotal,
	s.Obligation = cs.ObligationTotal
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
	s.Obligation = cs.ObligationTotal
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
			s.SchemeId
		FROM
			#EvidenceSummaryWithTotals s
		WHERE
			s.CategoryId >= 1 AND s.CategoryId <= 14
		GROUP BY
			s.SchemeId) cs ON cs.SchemeId = s.SchemeId AND s.CategoryId = 1001

SELECT 
	CategoryId,
	CategoryName,
	SchemeId,
	s.SchemeName,
	s.ApprovalNumber,
	COALESCE(Obligation, 0) AS Obligation,
	COALESCE(EvidenceReceivedInTotal, 0) + (COALESCE(TransferEvidenceReceivedIn, 0) - COALESCE(TransferEvidenceReceivedOut, 0)) AS Evidence,
	COALESCE(EvidenceReuseInTotal, 0) + (COALESCE(TransferEvidenceReuseIn, 0) - COALESCE(TransferEvidenceReuseOut, 0)) AS Reuse,
	COALESCE(NonHouseholdEvidenceReceivedInTotal, 0) As NonHouseHoldEvidence,
	COALESCE(NonHouseHoldEvidenceReuseInTotal, 0) AS NonHouseHoldEvidenceReuse,
	COALESCE(TransferEvidenceReceivedOut, 0) AS TransferredOut,
	COALESCE(TransferEvidenceReceivedIn, 0) AS TransferredIn,
	(COALESCE(EvidenceReceivedInTotal, 0) + (COALESCE(TransferEvidenceReceivedIn, 0) - COALESCE(TransferEvidenceReceivedOut, 0))) - COALESCE(Obligation, 0) AS ObligationDifference
FROM 
	#EvidenceSummaryWithTotals st
	INNER JOIN [PCS].Scheme s ON s.Id = st.SchemeId

UNION ALL

SELECT 
	CategoryId,
	CategoryName,
	NULL,
	'-' AS SchemeName,
	'-' AS ApprovalNumber,
	SUM(COALESCE(Obligation, 0)),
	SUM(COALESCE(EvidenceReceivedInTotal, 0) + (COALESCE(TransferEvidenceReceivedIn, 0) - COALESCE(TransferEvidenceReceivedOut, 0))) AS Evidence,
	SUM(COALESCE(EvidenceReuseInTotal, 0) + (COALESCE(TransferEvidenceReuseIn, 0) - COALESCE(TransferEvidenceReuseOut, 0))) AS Reuse,
	SUM(COALESCE(NonHouseholdEvidenceReceivedInTotal, 0)) As NonHouseHoldEvidence,
	SUM(COALESCE(NonHouseHoldEvidenceReuseInTotal, 0)) AS NonHouseHoldEvidenceReuse,
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
ORDER BY
	SchemeName,
	CategoryId

END
