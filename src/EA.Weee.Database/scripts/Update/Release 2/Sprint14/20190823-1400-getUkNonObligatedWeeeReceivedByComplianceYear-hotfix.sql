/****** Object:  StoredProcedure [AATF].[getUkNonObligatedWeeeReceivedByComplianceYear]    Script Date: 23/08/2019 14:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
ALTER PROCEDURE [AATF].[getUkNonObligatedWeeeReceivedByComplianceYear] 
	@ComplianceYear INT
AS
BEGIN

-- This table will be all the returns we want to get weee data for
DECLARE @FinalReturns TABLE
(
	ReturnId UNIQUEIDENTIFIER,
	[Quarter] INT,
	[Year] INT
);

DECLARE @Quarters TABLE
(
	[Quarter] INT
)
INSERT INTO @Quarters SELECT 1 
INSERT INTO @Quarters SELECT 2 
INSERT INTO @Quarters SELECT 3 
INSERT INTO @Quarters SELECT 4

DECLARE @Totals TABLE
(
	[QuarterId] INT,
	[Quarter] NVARCHAR(10),
	CategoryId INT,
	Category NVARCHAR(60),
	[TotalNonObligatedWeeeReceived] DECIMAL(23,3) NULL, 
	[TotalNonObligatedWeeeReceivedFromDcf] DECIMAL(23,3) NULL
);

-- Insert all returns we want to try an use into a temp table
INSERT INTO @FinalReturns (ReturnId, [Quarter], [Year])
SELECT
	r.Id,
	r.[Quarter],
	r.[ComplianceYear]
FROM
	[Aatf].[Return] r
	INNER JOIN (
			SELECT 	
				ROW_NUMBER() OVER (PARTITION BY r.ComplianceYear, r.[Quarter], r.OrganisationId ORDER BY SubmittedDate DESC) AS rn,
				r.Id
			FROM
				[AATF].[Return] r
			WHERE
				r.FacilityType = 1
				AND r.ReturnStatus = 2 
				AND r.ComplianceYear = @ComplianceYear) r2 ON r2.Id = r.Id AND rn = 1
	WHERE r.ComplianceYear = @ComplianceYear

INSERT INTO @Totals ([Quarter], Category, CategoryId, QuarterId)
SELECT
	CONCAT('Q', q.[Quarter]),
	CONCAT(w.Id, '. ', w.[Name]),
	w.Id,
	q.[Quarter]
FROM
	@Quarters q, [Lookup].WeeeCategory w

INSERT INTO @Totals ([Quarter], Category, CategoryId, QuarterId)
SELECT
	@ComplianceYear,
	CONCAT(w.Id, '. ', w.[Name]),
	w.Id,
	@ComplianceYear
FROM
	[Lookup].WeeeCategory w

;WITH TotalNonObligated (Tonnage, CategoryId, [Quarter])
AS (
SELECT
	SUM(wr.Tonnage),
	wr.CategoryId,
	r.[Quarter]
FROM
	[AATF].NonObligatedWeee wr
	INNER JOIN [AATF].[Return] r ON r.Id = wr.ReturnId
	INNER JOIN @FinalReturns f ON f.ReturnId = r.Id
WHERE
	wr.Dcf = 0 
	AND r.ComplianceYear = @ComplianceYear
GROUP BY
	r.[Quarter],
	wr.CategoryId
)
UPDATE
	t
SET
	t.[TotalNonObligatedWeeeReceived] = tn.Tonnage
FROM
	@Totals t
	INNER JOIN TotalNonObligated tn ON tn.CategoryId = t.CategoryId AND tn.[Quarter] = t.[QuarterId]

;WITH TotalNonObligatedPerYear (Tonnage, CategoryId, [Year])
AS (
SELECT
	SUM(wr.Tonnage),
	wr.CategoryId,
	@ComplianceYear
FROM
	[AATF].NonObligatedWeee wr
	INNER JOIN [AATF].[Return] r ON r.Id = wr.ReturnId
	INNER JOIN @FinalReturns f ON f.ReturnId = r.Id
WHERE
	wr.Dcf = 0 
	AND r.ComplianceYear = @ComplianceYear
GROUP BY
	r.[Quarter],
	wr.CategoryId
)
UPDATE
	t
SET
	t.[TotalNonObligatedWeeeReceived] = tn.Tonnage
FROM
	@Totals t
	INNER JOIN TotalNonObligatedPerYear tn ON tn.CategoryId = t.CategoryId AND tn.[Year] = t.[QuarterId]

;WITH TotalNonObligatedDcf (Tonnage, CategoryId, [Quarter])
AS (
SELECT
	SUM(wr.Tonnage),
	wr.CategoryId,
	r.[Quarter]
FROM
	[AATF].NonObligatedWeee wr
	INNER JOIN [AATF].[Return] r ON r.Id = wr.ReturnId
	INNER JOIN @FinalReturns f ON f.ReturnId = r.Id
WHERE
	wr.Dcf = 1 
	AND r.ComplianceYear = @ComplianceYear
GROUP BY
	r.[Quarter],
	wr.CategoryId
)
UPDATE
	t
SET
	t.[TotalNonObligatedWeeeReceivedFromDcf] = tn.Tonnage
FROM
	@Totals t
	INNER JOIN TotalNonObligatedDcf tn ON tn.CategoryId = t.CategoryId AND tn.[Quarter] = t.[QuarterId]

;WITH TotalNonObligatedDcfPerYear (Tonnage, CategoryId, [Year])
AS (
SELECT
	SUM(wr.Tonnage),
	wr.CategoryId,
	@ComplianceYear
FROM
	[AATF].NonObligatedWeee wr
	INNER JOIN [AATF].[Return] r ON r.Id = wr.ReturnId
	INNER JOIN @FinalReturns f ON f.ReturnId = r.Id
WHERE
	wr.Dcf = 1 
	AND r.ComplianceYear = @ComplianceYear
GROUP BY
	r.[Quarter],
	wr.CategoryId
)
UPDATE
	t
SET
	t.[TotalNonObligatedWeeeReceivedFromDcf] = tn.Tonnage
FROM
	@Totals t
	INNER JOIN TotalNonObligatedDcfPerYear tn ON tn.CategoryId = t.CategoryId AND tn.[Year] = t.[QuarterId]

SELECT
	t.[Quarter],
	t.Category, 
	t.TotalNonObligatedWeeeReceived,
	t.TotalNonObligatedWeeeReceivedFromDcf
FROM
	@Totals t

END