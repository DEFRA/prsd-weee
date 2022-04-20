IF OBJECT_ID('[[Evidence]].[getAatfEvidenceSummaryTotals]', 'P') IS NOT NULL
	DROP PROC [Evidence].[getAatfEvidenceSummaryTotals]
GO

CREATE PROCEDURE [Evidence].[getAatfEvidenceSummaryTotals]
	@AatfId UNIQUEIDENTIFIER
AS
BEGIN
SET NOCOUNT ON;

WITH records_cte
AS
(
	SELECT 
		c.Id,
		c.[Name],
		nt.Received,
		nt.Reused
	FROM
		Evidence.Note n 
		INNER JOIN Evidence.NoteTonnage nt ON n.Id = nt.NoteId
		INNER JOIN Lookup.WeeeCategory c ON c.Id = nt.CategoryId
	WHERE	
		n.[Status] = 3
		AND n.AatfId = @AatfId
),
totals_cte AS
(
SELECT 
	rc.Id AS CategoryId,
	rc.[Name] AS CategoryName,
	SUM(rc.Received) AS Received,
	SUM(rc.Reused) As Reused
FROM
	records_cte rc
GROUP BY
	rc.Id,
	rc.[Name]
)
SELECT * FROM totals_cte

END
GO


