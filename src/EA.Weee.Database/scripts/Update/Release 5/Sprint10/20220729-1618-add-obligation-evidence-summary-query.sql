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
	Evidence DECIMAL,
	Reuse DECIMAL,
	TransferredOut DECIMAL,
	TransferredIn DECIMAL,
	ObligationDifference DECIMAL
);

INSERT INTO #EvidenceSummary (CategoryId, CategoryName)
SELECT
	c.Id,
	c.[Name]
FROM
	Lookup.WeeeCategory c;

UPDATE s
SET 
	s.Obligation = osa.Obligation
FROM
	#EvidenceSummary s
	LEFT JOIN [PCS].ObligationSchemeAmount osa ON s.CategoryId = osa.CategoryId 
	LEFT JOIN [PCS].ObligationScheme os ON os.Id = osa.ObligationSchemeId
	LEFT JOIN [PCS].Scheme sc ON sc.Id = os.SchemeId
WHERE
	os.ComplianceYear = @ComplianceYear;

WITH transfer_out_totals_cte AS
(
	SELECT 
		nt.CategoryId AS CategoryId, SUM(ntt.Received) AS TransferredEvidence
	FROM 
		[Evidence].NoteTonnage nt
		LEFT JOIN [Evidence].NoteTransferTonnage ntt ON ntt.NoteTonnageId = nt.Id
		LEFT JOIN [Evidence].Note n ON n.Id = ntt.TransferNoteId
		LEFT JOIN [Organisation].Organisation o ON o.Id = n.OrganisationId
		LEFT JOIN [PCS].Scheme sc ON sc.OrganisationId = o.Id
	WHERE
		n.Status = 3 AND 
		n.NoteType = 2 AND
		sc.Id = @SchemeId AND 
		n.ComplianceYear = @ComplianceYear AND
		n.WasteType = 1
	GROUP BY 
		nt.CategoryId
)

UPDATE 
	s
SET
	s.TransferredOut = t.TransferredEvidence
FROM
	#EvidenceSummary s
	INNER JOIN transfer_out_totals_cte t ON t.CategoryId = s.CategoryId;

WITH transfer_in_totals_cte AS
(
	SELECT 
		nt.CategoryId, SUM(ntt.Received) AS TransferredIn
	FROM 
		[Evidence].NoteTonnage nt 
		LEFT JOIN [Evidence].NoteTransferTonnage ntt ON ntt.NoteTonnageId = nt.Id
		LEFT JOIN [Evidence].Note n ON n.Id = ntt.TransferNoteId
		LEFT JOIN [Organisation].Organisation o ON o.Id = n.RecipientId
		LEFT JOIN [PCS].Scheme sc ON sc.OrganisationId = o.Id
	WHERE
		n.Status = 3 AND 
		n.NoteType = 2 AND
		sc.Id = @SchemeId AND 
		n.ComplianceYear = @ComplianceYear AND
		n.WasteType = 1
	GROUP BY 
		nt.CategoryId
)

UPDATE 
	s
SET
	s.TransferredIn = t.TransferredIn,
	s.ObligationDifference = (Obligation - Evidence)
FROM
	#EvidenceSummary s
	INNER JOIN transfer_in_totals_cte t ON t.CategoryId = s.CategoryId;

WITH evidence_totals_cte AS
(
	SELECT
		s.CategoryID,
		SUM(nt.Received) AS Evidence,
		SUM(nt.Reused) AS Reused
	FROM
		#EvidenceSummary s
		LEFT JOIN [Evidence].NoteTonnage nt ON s.CategoryId = nt.CategoryId 
		LEFT JOIN [Evidence].Note n ON n.Id = nt.NoteId
		LEFT JOIN [Organisation].Organisation o ON o.Id = n.OrganisationId
		LEFT JOIN [PCS].Scheme sc ON sc.OrganisationId = o.Id
	WHERE
		n.Status = 3 AND 
		sc.Id = @SchemeId AND 
		n.ComplianceYear = @ComplianceYear AND
		n.WasteType = 1
	GROUP BY
		s.CategoryId
)

UPDATE 
	s
SET
	s.Evidence = t.Evidence,
	s.Reuse = t.Reused
FROM
	#EvidenceSummary s
	INNER JOIN evidence_totals_cte t ON t.CategoryId = s.CategoryId;

SELECT * FROM #EvidenceSummary;




