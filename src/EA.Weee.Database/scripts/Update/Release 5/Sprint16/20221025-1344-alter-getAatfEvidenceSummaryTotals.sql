ALTER PROCEDURE [Evidence].[getAatfEvidenceSummaryTotals]
	@AatfId UNIQUEIDENTIFIER,
	@ComplianceYear SMALLINT
WITH RECOMPILE
AS
BEGIN
SET NOCOUNT ON;

DECLARE @AatfGroupId UNIQUEIDENTIFIER
SELECT @AatfGroupId = AATFId FROM [AATF].AATF WHERE Id = @AatfId;

IF OBJECT_ID('tempdb..#AatfSummaryTotals') IS NOT NULL 
BEGIN
	DROP TABLE #AatfSummaryTotals
END

CREATE TABLE #AatfSummaryTotals(
	CategoryId INT NOT NULL,
	CategoryName NVARCHAR(60) NOT NULL,
	ApprovedReceived DECIMAL(28, 3) NULL,
	ApprovedReused DECIMAL(28, 3) NULL,
	SubmittedReceived DECIMAL(28, 3) NULL,
	SubmittedReused DECIMAL(28, 3) NULL,
	DraftReceived DECIMAL(28, 3) NULL,
	DraftReused DECIMAL(28, 3) NULL
)

INSERT INTO #AatfSummaryTotals (CategoryId, CategoryName)
SELECT
	c.Id,
	c.[Name]
FROM
	Lookup.WeeeCategory c
UNION ALL
SELECT 
	1001 As Id,
	'Total (tonnes)' AS NAME

-- approved totals
;With totals_cte AS
(
SELECT 
	c.CategoryId,
	SUM(nt.Received) AS Received,
	SUM(nt.Reused) AS Reused
FROM
	#AatfSummaryTotals c
	LEFT JOIN Evidence.NoteTonnage nt WITH (NOLOCK) ON c.CategoryId = nt.CategoryId
	LEFT JOIN Evidence.Note n WITH (NOLOCK) ON n.Id = nt.NoteId 
	LEFT JOIN AATF.AATF aa WITH (NOLOCK) ON aa.Id = n.AatfId
WHERE
	n.[Status] = 3 AND 
	aa.AatfId = @AatfGroupId AND 
	n.ComplianceYear = @ComplianceYear AND
	c.CategoryId >= 1 AND c.CategoryId <= 14
GROUP BY
	c.CategoryId
)
UPDATE
	c
SET
	ApprovedReceived = t.Received,
	ApprovedReused = t.Reused
FROM
	#AatfSummaryTotals c
	INNER JOIN totals_cte t ON t.CategoryId = c.CategoryId

-- submitted totals
;With totals_cte AS
(
SELECT 
	c.CategoryId,
	SUM(nt.Received) AS Received,
	SUM(nt.Reused) AS Reused
FROM
	#AatfSummaryTotals c
	LEFT JOIN Evidence.NoteTonnage nt WITH (NOLOCK) ON c.CategoryId = nt.CategoryId
	LEFT JOIN Evidence.Note n WITH (NOLOCK) ON n.Id = nt.NoteId 
	LEFT JOIN AATF.AATF aa WITH (NOLOCK) ON aa.Id = n.AatfId
WHERE
	n.[Status] = 2 AND 
	aa.AatfId = @AatfGroupId AND 
	n.ComplianceYear = @ComplianceYear AND
	c.CategoryId >= 1 AND c.CategoryId <= 14
GROUP BY
	c.CategoryId
)
UPDATE
	c
SET
	SubmittedReceived = t.Received,
	SubmittedReused = t.Reused
FROM
	#AatfSummaryTotals c
	INNER JOIN totals_cte t ON t.CategoryId = c.CategoryId

-- draft totals
;With totals_cte AS
(
SELECT 
	c.CategoryId,
	SUM(nt.Received) AS Received,
	SUM(nt.Reused) AS Reused
FROM
	#AatfSummaryTotals c
	LEFT JOIN Evidence.NoteTonnage nt WITH (NOLOCK) ON c.CategoryId = nt.CategoryId
	LEFT JOIN Evidence.Note n WITH (NOLOCK) ON n.Id = nt.NoteId 
	LEFT JOIN AATF.AATF aa WITH (NOLOCK) ON aa.Id = n.AatfId
WHERE
	n.[Status] = 1 AND 
	aa.AatfId = @AatfGroupId AND 
	n.ComplianceYear = @ComplianceYear AND
	c.CategoryId >= 1 AND c.CategoryId <= 14
GROUP BY
	c.CategoryId
)
UPDATE
	c
SET
	DraftReceived = t.Received,
	DraftReused = t.Reused
FROM
	#AatfSummaryTotals c
	INNER JOIN totals_cte t ON t.CategoryId = c.CategoryId

UPDATE
	s
SET
	s.ApprovedReceived = cs.ApprovedReceivedTotal,
	s.ApprovedReused = cs.ApprovedReusedTotal,
	s.SubmittedReceived = cs.SubmittedReceivedTotal,
	s.SubmittedReused = cs.SubmittedReusedTotal,
	s.DraftReceived = cs.DraftReceivedTotal,
	s.DraftReused = cs.DraftReusedTotal
FROM
	#AatfSummaryTotals s
	INNER JOIN
		(SELECT
			SUM(s.ApprovedReceived) AS ApprovedReceivedTotal,
			SUM(s.ApprovedReused) AS ApprovedReusedTotal,
			SUM(s.SubmittedReceived) AS SubmittedReceivedTotal,
			SUM(s.SubmittedReused) AS SubmittedReusedTotal,
			SUM(s.DraftReceived) AS DraftReceivedTotal,
			SUM(s.DraftReused) As DraftReusedTotal
		FROM
			#AatfSummaryTotals s
		WHERE
			s.CategoryId >= 1 AND s.CategoryId <= 14) cs ON s.CategoryId = 1001

SELECT * FROM #AatfSummaryTotals

END