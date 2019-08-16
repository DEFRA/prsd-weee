IF OBJECT_ID('[AATF].getReturnObligatedCsvData', 'P') IS NOT NULL BEGIN
	DROP PROCEDURE [AATF].[getReturnObligatedCsvData]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Description:	This stored procedure is used to provide the data for the admin report of obligatde data
--				that have/haven't submitted a data return within
--				the limits of the specified parameters.Get the latest submitted return

-- =============================================
CREATE PROCEDURE [AATF].[getReturnObligatedCsvData]
	@ReturnId UNIQUEIDENTIFIER
AS
BEGIN

DECLARE @ReturnId UNIQUEIDENTIFIER
DECLARE @DynamicPivotQuery AS NVARCHAR(MAX)
DECLARE @ColumnName AS NVARCHAR(MAX)

DECLARE @ObligationType TABLE
(
	Obligation INT
)
INSERT INTO @ObligationType SELECT 0 -- Household / B2C
INSERT INTO @ObligationType SELECT 1 -- Non house hold / B2B

DECLARE @FinalTable TABLE
(
	ReturnId UNIQUEIDENTIFIER,
	ComplianceYear INT,
	[Quarter] INT,
	AatfKey UNIQUEIDENTIFIER,
	[AatfName] NVARCHAR(256),
	[AatfApprovalNumber] NVARCHAR(20),
	ObligationType INT,
	CategoryId INT,
	CategoryName NVARCHAR (60),
	TotalSent				DECIMAL(35,3) NULL,
	TotalReused				DECIMAL(35,3) NULL,
	TotalReceived			DECIMAL(35,3) NULL
)


;WITH ObligationData (ObligationType, CategoryId, CategoryName)
AS (
SELECT 
	o.Obligation,
	w.Id,
	w.Name
FROM
	[Lookup].WeeeCategory w, @ObligationType o
)
INSERT INTO @FinalTable
SELECT
	r.Id,
	r.ComplianceYear,
	r.Quarter,
	a.Id,
	a.[Name],
	a.ApprovalNumber,
	o.ObligationType,
	o.CategoryId,
	o.CategoryName,
	NULL,
	NULL,
	NULL
FROM
	[AATF].[Return] r
	INNER JOIN [AATF].ReturnAatf ra ON ra.ReturnId = r.Id
	INNER JOIN [AATF].[AATF] a ON a.Id = ra.AatfId
	, ObligationData o
WHERE
	r.Id = @ReturnId


-- TOTAL RECEIVED PER OBLIGATION TYPE
;WITH TotalReceivedUpdate (AatfId, CategoryId, Tonnage)
AS (
SELECT
	wr.AatfId,
	wra.CategoryId,
	SUM(wra.HouseholdTonnage) AS Tonnage
FROM
	[AATF].WeeeReceived wr
	INNER JOIN [AATF].WeeeReceivedAmount wra ON wra.WeeeReceivedId = wr.Id
	INNER JOIN [AATF].[Return] r ON r.Id = wr.ReturnId
WHERE
	r.Id = @ReturnId
GROUP BY
	wr.AatfId,
	wra.CategoryId
)
UPDATE
	f
SET
	f.TotalReceived = t.Tonnage
FROM
	@FinalTable f
	INNER JOIN TotalReceivedUpdate t ON t.AatfId = f.AatfKey AND t.CategoryId = f.CategoryId AND f.ObligationType = 0

;WITH TotalReceivedUpdate (AatfId, CategoryId, Tonnage)
AS (
SELECT
	wr.AatfId,
	wra.CategoryId,
	SUM(wra.NonHouseHoldTonnage) AS Tonnage
FROM
	[AATF].WeeeReceived wr
	INNER JOIN [AATF].WeeeReceivedAmount wra ON wra.WeeeReceivedId = wr.Id
	INNER JOIN [AATF].[Return] r ON r.Id = wr.ReturnId
WHERE
	r.Id = @ReturnId
GROUP BY
	wr.AatfId,
	wra.CategoryId
)
UPDATE
	f
SET
	f.TotalReceived = t.Tonnage
FROM
	@FinalTable f
	INNER JOIN TotalReceivedUpdate t ON t.AatfId = f.AatfKey AND t.CategoryId = f.CategoryId AND f.ObligationType = 1

-- TOTAL SENT ON PER OBLIGATION TYPE
;WITH TotalSentOnUpdate (AatfId, CategoryId, Tonnage)
AS (
SELECT
	ws.AatfId,
	wsa.CategoryId,
	SUM(wsa.HouseholdTonnage) AS Tonnage
FROM
	[AATF].WeeeSentOn ws
	INNER JOIN [AATF].WeeeSentOnAmount wsa ON wsa.WeeeSentOnId = ws.Id
	INNER JOIN [AATF].[Return] r ON r.Id = ws.ReturnId
WHERE
	r.Id = @ReturnId
GROUP BY
	ws.AatfId,
	wsa.CategoryId
)
UPDATE
	f
SET
	f.TotalSent = t.Tonnage
FROM
	@FinalTable f
	INNER JOIN TotalSentOnUpdate t ON t.AatfId = f.AatfKey AND t.CategoryId = f.CategoryId AND f.ObligationType = 0

;WITH TotalSentOnUpdate (AatfId, CategoryId, Tonnage)
AS (
SELECT
	ws.AatfId,
	wsa.CategoryId,
	SUM(wsa.NonHouseholdTonnage) AS Tonnage
FROM
	[AATF].WeeeSentOn ws
	INNER JOIN [AATF].WeeeSentOnAmount wsa ON wsa.WeeeSentOnId = ws.Id
	INNER JOIN [AATF].[Return] r ON r.Id = ws.ReturnId
WHERE
	r.Id = @ReturnId
GROUP BY
	ws.AatfId,
	wsa.CategoryId
)
UPDATE
	f
SET
	f.TotalSent = t.Tonnage
FROM
	@FinalTable f
	INNER JOIN TotalSentOnUpdate t ON t.AatfId = f.AatfKey AND t.CategoryId = f.CategoryId AND f.ObligationType = 1

-- TOTAL REUSED PER OBLIGATION TYPE
;WITH TotalReusedUpdate (AatfId, CategoryId, Tonnage)
AS (
SELECT
	wr.AatfId,
	wra.CategoryId,
	wra.HouseholdTonnage AS Tonnage
FROM
	[AATF].WeeeReused wr
	INNER JOIN [AATF].WeeeReusedAmount wra ON wra.WeeeReusedId = wr.Id
	INNER JOIN [AATF].[Return] r ON r.Id = wr.ReturnId
WHERE
	r.Id = @ReturnId
)
UPDATE
	f
SET
	f.TotalReused = t.Tonnage
FROM
	@FinalTable f
	INNER JOIN TotalReusedUpdate t ON t.AatfId = f.AatfKey AND t.CategoryId = f.CategoryId AND f.ObligationType = 0

;WITH TotalReusedUpdate (AatfId, CategoryId, Tonnage)
AS (
SELECT
	wr.AatfId,
	wra.CategoryId,
	wra.NonHouseholdTonnage AS Tonnage
FROM
	[AATF].WeeeReused wr
	INNER JOIN [AATF].WeeeReusedAmount wra ON wra.WeeeReusedId = wr.Id
	INNER JOIN [AATF].[Return] r ON r.Id = wr.ReturnId
WHERE
	r.Id = @ReturnId
)
UPDATE
	f
SET
	f.TotalReused = t.Tonnage
FROM
	@FinalTable f
	INNER JOIN TotalReusedUpdate t ON t.AatfId = f.AatfKey AND t.CategoryId = f.CategoryId AND f.ObligationType = 1

-- TOTAL RECEIVED PER PCS
SELECT @ColumnName = ISNULL(@ColumnName + ',','') + QUOTENAME(SchemeName)
FROM 
(
	SELECT DISTINCT
		s.SchemeName
	FROM
		[AATF].WeeeReceived wr
		INNER JOIN [PCS].Scheme s ON s.Id = wr.SchemeId
	WHERE
		 wr.ReturnId = @ReturnId) AS SchemeNames

	SET @DynamicPivotQuery = 
	N'
	WITH pivotPcsObligated AS
	(
	SELECT
		CategoryId,
		Tonnage,
		SchemeName,
		AatfId,
		CASE 
            WHEN Tonnages = ''HouseholdTonnage'' THEN 0
            WHEN Tonnages= ''NonHouseholdTonnage'' THEN 1
        END AS ObligationType
	FROM
	(
	SELECT
		wra.CategoryId,
		wra.HouseholdTonnage,
		wra.NonHouseholdTonnage,
		s.SchemeName,
		wr.AatfId
	FROM
		[AATF].WeeeReceived wr
		INNER JOIN [AATF].WeeeReceivedAmount wra ON wra.WeeeReceivedId = wr.Id
		INNER JOIN [PCS].Scheme s ON s.Id = wr.SchemeId
	WHERE
		wr.ReturnId = ''' + CONVERT(NVARCHAR(50), @ReturnId) + '''
	) AS x
	UNPIVOT
	(
		Tonnage FOR Tonnages IN (HouseholdTonnage, NonHouseholdTonnage)
	) AS y
	)
	SELECT
		CategoryId, AatfId, ' + @ColumnName + ', ObligationType
	INTO 
		##PcsObligatedHouseHold
	FROM
		pivotPcsObligated
	PIVOT (MAX(Tonnage) FOR SchemeName IN (' + @ColumnName + ')) AS x'

EXEC (@DynamicPivotQuery)

-- TOTAL SENT ON TO SITE
SELECT @ColumnName = ISNULL(@ColumnName + ',','') + QUOTENAME(SiteOperator)
FROM 
(
	SELECT DISTINCT
		sa.Name AS SiteOperator
	FROM
		[AATF].WeeeSentOn ws
		LEFT JOIN AATF.[Address] sa ON sa.Id = ws.SiteAddressId
	WHERE
		 ws.ReturnId = @ReturnId) AS SiteAddresses

SET @DynamicPivotQuery = 
	N'
	WITH pivotSentOnObligated AS
	(
	SELECT
		CategoryId,
		Tonnage,
		SiteOperator,
		AatfId,
		CASE 
            WHEN Tonnages = ''HouseholdTonnage'' THEN 0
            WHEN Tonnages= ''NonHouseholdTonnage'' THEN 1
        END AS ObligationType
	FROM
	(
	SELECT
		wsa.CategoryId,
		wsa.HouseholdTonnage,
		wsa.NonHouseholdTonnage,
		sa.Name AS SiteOperator,
		wso.AatfId
	FROM
		[AATF].WeeeSentOn wso
		INNER JOIN [AATF].WeeeSentOnAmount wsa ON wsa.WeeeSentOnId = wso.Id
		LEFT JOIN AATF.[Address] sa ON sa.Id = wso.SiteAddressId
	WHERE
		wso.ReturnId = ''' + CONVERT(NVARCHAR(50), @ReturnId) + '''
	) AS x
	UNPIVOT
	(
		Tonnage FOR Tonnages IN (HouseholdTonnage, NonHouseholdTonnage)
	) AS y
	)
	SELECT
		CategoryId, AatfId, ' + @ColumnName + ', ObligationType
	INTO 
		##SentOnObligated
	FROM
		pivotSentOnObligated
	PIVOT (MAX(Tonnage) FOR SiteOperator IN (' + @ColumnName + ')) AS x'

PRINT @DynamicPivotQuery
EXEC (@DynamicPivotQuery)

--SELECT * FROM ##SentOnObligated
SELECT
	f.*,
	ph.*,
	so.*
FROM
	@FinalTable f
	LEFT JOIN ##PcsObligatedHouseHold ph ON ph.CategoryId = f.CategoryId AND ph.AatfId = f.AatfKey AND ph.ObligationType = f.ObligationType
	LEFT JOIN ##SentOnObligated so ON so.CategoryId = f.CategoryId AND so.AatfId = f.AatfKey AND so.ObligationType = f.ObligationType

DROP TABLE ##PcsObligatedHouseHold
DROP TABLE ##SentOnObligated

END