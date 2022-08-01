IF OBJECT_ID('[Evidence].[getObligationEvidenceSummaryTotals]', 'P') IS NOT NULL
	DROP PROC [Evidence].[getObligationEvidenceSummaryTotals]
GO

CREATE PROCEDURE [Evidence].[getObligationEvidenceSummaryTotals]
	@PCSId UNIQUEIDENTIFIER,
	@OrgId UNIQUEIDENTIFIER,
	@ComplianceYear SMALLINT
AS
BEGIN
SET NOCOUNT ON;

IF OBJECT_ID('tempdb..#Summary') IS NOT NULL 
BEGIN
	DROP TABLE #Summary
END

CREATE TABLE #Summary(
	CategoryId INT NOT NULL,
	CategoryName NVARCHAR(60),
	Obligation DECIMAL,
	Evidence DECIMAL,
	Reuse DECIMAL,
	TransferredOut DECIMAL,
	TransferredIn DECIMAL,
	ObligationDifference DECIMAL
);

INSERT INTO #Summary (CategoryId, CategoryName)
SELECT
	c.Id,
	c.[Name]
FROM
	Lookup.WeeeCategory c;

UPDATE s
SET 
	s.Obligation = osa.Obligation
FROM
	#Summary s
	LEFT JOIN [PCS].[ObligationSchemeAmount] osa ON s.CategoryId = osa.CategoryId 
WHERE
	osa.ObligationSchemeId IN (SELECT Id FROM [PCS].[ObligationScheme] WHERE SchemeId = @PCSId AND ComplianceYear = @ComplianceYear);

WITH evidence_totals_cte AS
(
	SELECT
		s.CategoryID,
		SUM(nt.Received) AS Evidence,
		SUM(nt.Reused) AS Reused
	FROM
		#Summary s
		LEFT JOIN [Evidence].NoteTonnage nt ON s.CategoryId = nt.CategoryId
		LEFT JOIN [Evidence].Note n ON n.Id = nt.NoteId
	WHERE
		n.Status = 3 AND n.RecipientId = @OrgId AND n.ComplianceYear = @ComplianceYear
	GROUP BY
		s.CategoryId
)

UPDATE 
	s
SET
	s.Evidence = t.Evidence,
	s.Reuse = t.Reused
FROM
	#Summary s
	INNER JOIN evidence_totals_cte t ON t.CategoryId = s.CategoryId;

WITH transfer_out_totals_cte AS
(
	SELECT 
		nt.CategoryId AS CategoryId, SUM(ntt.Received) AS TransferredEvidence
	FROM 
		[Evidence].NoteTonnage nt
		LEFT JOIN [Evidence].NoteTransferTonnage ntt ON ntt.NoteTonnageId = nt.Id
	WHERE
		nt.NoteId IN (SELECT n.Id FROM [Evidence].Note n WHERE n.RecipientId = @OrgId AND n.Status = 3 AND n.ComplianceYear = @ComplianceYear AND NoteType = 1)
	GROUP BY 
		nt.CategoryId
)

UPDATE 
	s
SET
	s.TransferredOut = t.TransferredEvidence
FROM
	#Summary s
	INNER JOIN transfer_out_totals_cte t ON t.CategoryId = s.CategoryId;

WITH transfer_in_totals_cte AS
(
	SELECT 
		nt.CategoryId, SUM(ntt.Received) AS TransferredIn
	FROM 
		[Evidence].NoteTransferTonnage ntt 
		LEFT JOIN [Evidence].NoteTonnage nt ON ntt.NoteTonnageId = nt.Id
	WHERE
		ntt.TransferNoteId IN (SELECT n.Id FROM [Evidence].Note n WHERE n.RecipientId = @OrgId AND n.Status = 3 AND n.ComplianceYear = @ComplianceYear AND NoteType = 2)
	GROUP BY 
		nt.CategoryId
)

UPDATE 
	s
SET
	s.TransferredIn = t.TransferredIn,
	s.ObligationDifference = (Obligation - Evidence)
FROM
	#Summary s
	INNER JOIN transfer_in_totals_cte t ON t.CategoryId = s.CategoryId;

SELECT * FROM #Summary;

END
GO


