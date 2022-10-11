SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [Evidence].[getAatfEvidenceSummaryTotals]
	@AatfId UNIQUEIDENTIFIER,
	@ComplianceYear SMALLINT
WITH RECOMPILE
AS
BEGIN
SET NOCOUNT ON;

DECLARE @AatfGroupId UNIQUEIDENTIFIER
SELECT @AatfGroupId = AATFId FROM [AATF].AATF WHERE Id = @AatfId;

IF OBJECT_ID('tempdb..#CategoryTotals') IS NOT NULL 
BEGIN
	DROP TABLE #CategoryTotals
END

CREATE TABLE #CategoryTotals(
	CategoryId INT NOT NULL,
	CategoryName NVARCHAR(60) NOT NULL,
	Received DECIMAL(28, 3) NULL,
	Reused DECIMAL(28, 3) NULL
)

INSERT INTO #CategoryTotals (CategoryId, CategoryName)
SELECT
	c.Id,
	c.[Name]
FROM
	Lookup.WeeeCategory c

;With totals_cte AS
(
SELECT 
	c.CategoryId,
	SUM(nt.Received) AS Received,
	SUM(nt.Reused) AS Reused
FROM
	#CategoryTotals c
	LEFT JOIN Evidence.NoteTonnage nt ON c.CategoryId = nt.CategoryId
	LEFT JOIN Evidence.Note n ON n.Id = nt.NoteId 
	LEFT JOIN AATF.AATF aa ON aa.Id = n.AatfId
WHERE
	n.[Status] = 3 AND aa.AatfId = @AatfGroupId AND n.ComplianceYear = @ComplianceYear
GROUP BY
	c.CategoryId
)
UPDATE
	c
SET
	Received = t.Received,
	Reused = t.Reused
FROM
	#CategoryTotals c
	INNER JOIN totals_cte t ON t.CategoryId = c.CategoryId

SELECT * FROM #CategoryTotals

END