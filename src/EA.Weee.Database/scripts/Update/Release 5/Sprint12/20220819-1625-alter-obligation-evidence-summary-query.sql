IF OBJECT_ID('[Evidence].[getObligationEvidenceSummaryTotals]', 'P') IS NOT NULL
	DROP PROC [Evidence].[getObligationEvidenceSummaryTotals]
GO

CREATE PROCEDURE [Evidence].[getObligationEvidenceSummaryTotals]
	@SchemeId UNIQUEIDENTIFIER,
	@ComplianceYear SMALLINT
AS
BEGIN
SET NOCOUNT ON;

IF OBJECT_ID('tempdb..#EvidenceSummary') IS NOT NULL 
BEGIN
	DROP TABLE #EvidenceSummary
END

CREATE TABLE #EvidenceSummary(
	CategoryId INT NOT NULL PRIMARY KEY,
	CategoryName NVARCHAR(60),
	Obligation DECIMAL(28, 3),
	EvidenceReceivedInTotal DECIMAL(28, 3),
	EvidenceReuseInTotal DECIMAL(28, 3),
	TransferEvidenceReceivedIn DECIMAL(28, 3),
	TransferEvidenceReuseIn DECIMAL(28, 3),
	TransferEvidenceReceivedOut DECIMAL(28, 3),
	TransferEvidenceReuseOut DECIMAL(28, 3)
)

INSERT INTO #EvidenceSummary (CategoryId, CategoryName, Obligation, EvidenceReceivedInTotal, EvidenceReuseInTotal, 
TransferEvidenceReceivedIn, TransferEvidenceReuseIn, TransferEvidenceReceivedOut, TransferEvidenceReuseOut)
SELECT
	c.Id,
	c.[Name],
	NULL, 
	NULL, 
	NULL,
	NULL, 
	NULL, 
	NULL,
	NULL
FROM
	Lookup.WeeeCategory c
	

UPDATE s
SET 
	s.Obligation = osa.Obligation
FROM
	#EvidenceSummary s
	LEFT JOIN [PCS].ObligationSchemeAmount osa ON s.CategoryId = osa.CategoryId 
	LEFT JOIN [PCS].ObligationScheme os ON os.Id = osa.ObligationSchemeId
	LEFT JOIN [PCS].Scheme sc ON sc.Id = os.SchemeId AND sc.Id = @SchemeId
WHERE
	os.ComplianceYear = @ComplianceYear
	AND os.SchemeId = @SchemeId

UPDATE 
	s
SET
	s.EvidenceReceivedInTotal = evc.Received,
	s.EvidenceReuseInTotal = evc.Reused
FROM
	#EvidenceSummary s
	INNER JOIN Evidence.vwEvidenceSumByCategory evc WITH (NOLOCK) ON evc.CategoryId = s.CategoryId AND evc.ComplianceYear = @ComplianceYear
	INNER JOIN [PCS].Scheme sc ON sc.OrganisationId = evc.ReceiverOrganisation AND sc.Id = @SchemeId

UPDATE 
	s
SET
	s.TransferEvidenceReceivedIn = evc.TransferredReceived,
	s.TransferEvidenceReuseIn =  evc.TransferredReused
FROM
	#EvidenceSummary s
	INNER JOIN Evidence.vwTransferSumByCategory evc WITH (NOLOCK) ON evc.CategoryId = s.CategoryId AND evc.ComplianceYear = @ComplianceYear
	INNER JOIN [PCS].Scheme sc ON sc.OrganisationId = evc.ReceiverOrganisation AND sc.Id = @SchemeId

UPDATE 
	s
SET
	s.TransferEvidenceReceivedOut = evc.TransferredReceived,
	s.TransferEvidenceReuseOut =  evc.TransferredReused
FROM
	#EvidenceSummary s
	INNER JOIN Evidence.vwTransferSumByCategory evc WITH (NOLOCK) ON evc.CategoryId = s.CategoryId AND evc.ComplianceYear = @ComplianceYear
	INNER JOIN [PCS].Scheme sc ON sc.OrganisationId = evc.TransferOrganisation AND sc.Id = @SchemeId

SELECT 
	CategoryId,
	Obligation,
	CASE WHEN TransferEvidenceReceivedIn IS NULL AND TransferEvidenceReceivedOut IS NULL AND EvidenceReceivedInTotal IS NULL THEN NULL ELSE
		COALESCE(EvidenceReceivedInTotal, 0) + (COALESCE(TransferEvidenceReceivedIn, 0) - COALESCE(TransferEvidenceReceivedOut, 0)) END AS Evidence,
	CASE WHEN EvidenceReuseInTotal IS NULL AND TransferEvidenceReuseIn IS NULL AND TransferEvidenceReuseOut IS NULL THEN NULL ELSE
		COALESCE(EvidenceReuseInTotal, 0) + (COALESCE(TransferEvidenceReuseIn, 0) - COALESCE(TransferEvidenceReuseOut, 0)) END AS Reuse,
	TransferEvidenceReceivedOut AS TransferredOut,
	TransferEvidenceReceivedIn AS TransferredIn,
	CASE WHEN EvidenceReceivedInTotal IS NULL AND TransferEvidenceReceivedIn IS NULL AND TransferEvidenceReceivedOut IS NULL AND Obligation IS NULL THEN NULL ELSE
		(COALESCE(EvidenceReceivedInTotal, 0) + (COALESCE(TransferEvidenceReceivedIn, 0) - COALESCE(TransferEvidenceReceivedOut, 0))) - COALESCE(Obligation, 0) END AS ObligationDifference
FROM 
	#EvidenceSummary

END


