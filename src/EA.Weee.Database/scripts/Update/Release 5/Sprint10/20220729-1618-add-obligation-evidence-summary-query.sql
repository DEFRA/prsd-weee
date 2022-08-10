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
		CategoryId INT NOT NULL,
		CategoryName NVARCHAR(60),
		Obligation DECIMAL,
		EvidenceReceivedInTotal DECIMAL,
		EvidenceReuseInTotal DECIMAL,
		TransferEvidenceReceivedIn DECIMAL,
		TransferEvidenceReuseIn DECIMAL,
		TransferEvidenceReceivedOut DECIMAL,
		TransferEvidenceReuseOut DECIMAL
	)

INSERT INTO #EvidenceSummary (CategoryId, CategoryName)
SELECT
	c.Id,
	c.[Name]
FROM
	Lookup.WeeeCategory c
	
UPDATE s
SET 
	s.Obligation = osa.Obligation
FROM
	#EvidenceSummary s
	LEFT JOIN [PCS].ObligationSchemeAmount osa ON s.CategoryId = osa.CategoryId 
	LEFT JOIN [PCS].ObligationScheme os ON os.Id = osa.ObligationSchemeId
	LEFT JOIN [PCS].Scheme sc ON sc.Id = os.SchemeId
WHERE
	os.ComplianceYear = @ComplianceYear

UPDATE 
	s
SET
	s.EvidenceReceivedInTotal = evc.Received,
	s.EvidenceReuseInTotal =  evc.Reused
FROM
	#EvidenceSummary s
	INNER JOIN Evidence.vwEvidenceByCategory evc ON evc.CategoryId = s.CategoryId AND evc.ComplianceYear = @ComplianceYear
	INNER JOIN [PCS].Scheme sc ON sc.OrganisationId = evc.ReceiverOrganisation AND sc.Id = @SchemeId

UPDATE 
	s
SET
	s.TransferEvidenceReceivedIn = evc.TransferredReceived,
	s.TransferEvidenceReuseIn =  evc.TransferredReused
FROM
	#EvidenceSummary s
	INNER JOIN Evidence.vwTransferEvidenceByCategory evc ON evc.CategoryId = s.CategoryId AND evc.ComplianceYear = @ComplianceYear
	INNER JOIN [PCS].Scheme sc ON sc.OrganisationId = evc.TransferOrganisation AND sc.Id = @SchemeId

UPDATE 
	s
SET
	s.TransferEvidenceReceivedOut = evc.TransferredReceived,
	s.TransferEvidenceReuseOut =  evc.TransferredReused
FROM
	#EvidenceSummary s
	INNER JOIN Evidence.vwTransferEvidenceByCategory evc ON evc.CategoryId = s.CategoryId AND evc.ComplianceYear = @ComplianceYear
	INNER JOIN [PCS].Scheme sc ON sc.OrganisationId = evc.ReceiverOrganisation AND sc.Id = @SchemeId

SELECT 
	Obligation,
	EvidenceReceivedInTotal + (TransferEvidenceReceivedIn - TransferEvidenceReceivedOut) AS Evidence,
	EvidenceReuseInTotal + (TransferEvidenceReuseIn - TransferEvidenceReuseOut) AS Reuse,
	TransferEvidenceReceivedOut AS TransferredOut,
	TransferEvidenceReceivedIn AS TransferredIn,
	Obligation - (EvidenceReceivedInTotal + (TransferEvidenceReceivedIn - TransferEvidenceReceivedOut)) AS ObligationDifference
FROM 
	#EvidenceSummary

END


