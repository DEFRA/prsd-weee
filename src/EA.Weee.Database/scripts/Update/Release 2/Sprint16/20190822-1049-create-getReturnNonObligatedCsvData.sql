IF OBJECT_ID('[AATF].getReturnNonObligatedCsvData', 'P') IS NOT NULL BEGIN
	DROP PROCEDURE [AATF].[getReturnNonObligatedCsvData]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [AATF].[getReturnNonObligatedCsvData]
	@ReturnId UNIQUEIDENTIFIER
AS
BEGIN

DECLARE @FinalTable TABLE
(
	[Year] INT,
	[Quarter] CHAR(2),
	[SubmittedBy] NVARCHAR(500),
	[SubmittedDate] DATETIME,
	[Category] NVARCHAR (60),
	[CategoryId] INT,
	OrganisationName NVARCHAR(256),
	[TotalNonObligatedWeeeReceived]	DECIMAL(35,3) NULL,
	[TotalNonObligatedWeeeReceivedFromDcf] DECIMAL(35,3) NULL
)

INSERT INTO @FinalTable
SELECT
	r.ComplianceYear,
	CONCAT('Q', r.Quarter) AS [Quarter],
	CONCAT(u.FirstName,' ',u.Surname),
	r.SubmittedDate,
	CONCAT(c.Id,'. ', c.Name),
	c.Id,
	CASE WHEN o.Name IS NULL THEN o.TradingName ELSE o.Name END,
	NULL,
	NULL
FROM
	[AATF].[Return] r
	INNER JOIN [Organisation].[Organisation] o ON o.Id = r.OrganisationId
	LEFT JOIN  [Identity].[AspNetUsers] u ON u.id = r.SubmittedById,
	[Lookup].WeeeCategory c
WHERE
	r.Id = @ReturnId

-- non obligated
;WITH TotalNonObligated (CategoryId, Tonnage)
AS (
SELECT
	wr.CategoryId,
	wr.Tonnage
FROM
	[AATF].NonObligatedWeee wr
	INNER JOIN [AATF].[Return] r ON r.Id = wr.ReturnId
WHERE
	r.Id = @ReturnId
	AND wr.Dcf = 0
)
UPDATE
	f
SET
	f.[TotalNonObligatedWeeeReceived] = CASE t.Tonnage WHEN 0 THEN NULL ELSE t.Tonnage END
FROM
	@FinalTable f
	INNER JOIN TotalNonObligated t ON t.CategoryId = f.CategoryId

;WITH TotalNonObligated (CategoryId, Tonnage)
AS (
SELECT
	wr.CategoryId,
	wr.Tonnage
FROM
	[AATF].NonObligatedWeee wr
	INNER JOIN [AATF].[Return] r ON r.Id = wr.ReturnId
WHERE
	r.Id = @ReturnId
	AND wr.Dcf = 1
)
UPDATE
	f
SET
	f.[TotalNonObligatedWeeeReceivedFromDcf] = t.Tonnage
FROM
	@FinalTable f
	INNER JOIN TotalNonObligated t ON t.CategoryId = f.CategoryId

SELECT
	*
FROM
	@FinalTable

END