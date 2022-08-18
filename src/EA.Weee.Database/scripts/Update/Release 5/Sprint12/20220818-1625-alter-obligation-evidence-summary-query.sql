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
	0, 
	0, 
	0,
	0, 
	0, 
	0,
	0
FROM
	Lookup.WeeeCategory c
	

UPDATE s
SET 
	s.Obligation = ISNULL(osa.Obligation, 0)
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
	s.EvidenceReceivedInTotal = ISNULL(evc.Received, 0),
	s.EvidenceReuseInTotal = ISNULL(evc.Reused, 0)
FROM
	#EvidenceSummary s
	INNER JOIN Evidence.vwEvidenceSumByCategory evc WITH (NOLOCK) ON evc.CategoryId = s.CategoryId AND evc.ComplianceYear = @ComplianceYear
	INNER JOIN [PCS].Scheme sc ON sc.OrganisationId = evc.ReceiverOrganisation AND sc.Id = @SchemeId

UPDATE 
	s
SET
	s.TransferEvidenceReceivedIn = ISNULL(evc.TransferredReceived, 0),
	s.TransferEvidenceReuseIn =  ISNULL(evc.TransferredReused, 0)
FROM
	#EvidenceSummary s
	INNER JOIN Evidence.vwTransferEvidenceSumByCategory evc WITH (NOLOCK) ON evc.CategoryId = s.CategoryId AND evc.ComplianceYear = @ComplianceYear
	INNER JOIN [PCS].Scheme sc ON sc.OrganisationId = evc.ReceiverOrganisation AND sc.Id = @SchemeId

UPDATE 
	s
SET
	s.TransferEvidenceReceivedOut = ISNULL(evc.TransferredReceived, 0),
	s.TransferEvidenceReuseOut =  ISNULL(evc.TransferredReused, 0)
FROM
	#EvidenceSummary s
	INNER JOIN Evidence.vwTransferEvidenceSumByCategory evc WITH (NOLOCK) ON evc.CategoryId = s.CategoryId AND evc.ComplianceYear = @ComplianceYear
	INNER JOIN [PCS].Scheme sc ON sc.OrganisationId = evc.TransferOrganisation AND sc.Id = @SchemeId

SELECT 
	CategoryId,
	Obligation,
	EvidenceReceivedInTotal + (TransferEvidenceReceivedIn - TransferEvidenceReceivedOut) AS Evidence,
	EvidenceReuseInTotal + (TransferEvidenceReuseIn - TransferEvidenceReuseOut) AS Reuse,
	TransferEvidenceReceivedOut AS TransferredOut,
	TransferEvidenceReceivedIn AS TransferredIn,
	(EvidenceReceivedInTotal + (TransferEvidenceReceivedIn - TransferEvidenceReceivedOut)) - Obligation AS ObligationDifference
FROM 
	#EvidenceSummary

END


