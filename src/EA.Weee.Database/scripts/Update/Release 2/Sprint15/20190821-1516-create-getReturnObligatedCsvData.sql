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

DECLARE @DynamicPivotQuery AS NVARCHAR(MAX)
DECLARE @ColumnName AS NVARCHAR(MAX)
DECLARE @HasPcsData BIT
DECLARE @HasSentOnData BIT
DECLARE @AatfReportDate DATETIME
DECLARE @Status INT

SET @HasPcsData = 0
SET @HasSentOnData = 0

DECLARE @ObligationType TABLE
(
	Obligation INT
)
INSERT INTO @ObligationType SELECT 0 -- Household / B2C
INSERT INTO @ObligationType SELECT 1 -- Non house hold / B2B

SELECT
	@AatfReportDate = DATEFROMPARTS(r.ComplianceYear, l.StartMonth, l.StartDay),
	@Status = r.ReturnStatus
FROM
	[Lookup].QuarterWindowTemplate l
	INNER JOIN [AATF].[Return] r ON r.[Quarter] = l.[Quarter]
WHERE
	r.Id = @ReturnId

DECLARE @ReturnAatf TABLE
(
	AatfId UNIQUEIDENTIFIER,
	ReturnId UNIQUEIDENTIFIER
)

IF @Status = 2 BEGIN
INSERT INTO @ReturnAatf (AatfId, ReturnId)
	SELECT	
		AatfId,
		ReturnId
	FROM
		[AATF].ReturnAatf ra
	WHERE
		ra.ReturnId = @ReturnId
END

IF @Status = 1 BEGIN
INSERT INTO @ReturnAatf (AatfId, ReturnId)
	SELECT
		a.Id,
		@ReturnId
	FROM
		[AATF].AATF a 
		INNER JOIN [AATF].[Return] r ON r.OrganisationId = a.OrganisationId
	WHERE
		r.Id = @ReturnId
		AND (a.ApprovalDate IS NOT NULL AND a.ApprovalDate < @AatfReportDate)
END

CREATE TABLE ##FinalTable
(
	ReturnId UNIQUEIDENTIFIER,
	[Compliance Year] INT,
	[Quarter] CHAR(2),
	AatfKey UNIQUEIDENTIFIER,
	[Name of AATF] NVARCHAR(256),
	[AATF approval number] NVARCHAR(20),
	[Submitted by] NVARCHAR(500),
	[Submitted date (GMT)] DATETIME,
	[Category] NVARCHAR (60),
	ObligationType INT,
	[Obligation type] CHAR(3),
	CategoryId INT,	
	[Total obligated WEEE received on behalf of PCS(s) (t)]			DECIMAL(35,3) NULL,
	[Total obligated WEEE sent to another AATF / ATF for treatment (t)]				DECIMAL(35,3) NULL,
	[Total obligated WEEE reused as a whole appliance (t)]				DECIMAL(35,3) NULL
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
INSERT INTO ##FinalTable
SELECT
	r.Id,
	r.ComplianceYear,
	CONCAT('Q', r.Quarter) AS [Quarter],
	a.Id,
	a.[Name],
	a.ApprovalNumber,
	CONCAT(u.FirstName,' ',u.Surname),
	r.SubmittedDate,
	CONCAT(o.CategoryId,'. ', o.CategoryName),
	o.ObligationType,
	CASE o.ObligationType WHEN 0 THEN 'B2C' ELSE 'B2B' END AS [Obligation type],
	o.CategoryId,
	NULL,
	NULL,
	NULL
FROM
	[AATF].[Return] r
	INNER JOIN @ReturnAatf ra ON ra.ReturnId = r.Id
	INNER JOIN [AATF].[AATF] a ON a.Id = ra.AatfId
	LEFT JOIN  [Identity].[AspNetUsers] u ON u.id = r.SubmittedById
	, ObligationData o
WHERE
	r.Id = @ReturnId
ORDER BY
	a.[Name]

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
	f.[Total obligated WEEE received on behalf of PCS(s) (t)] = t.Tonnage
FROM
	##FinalTable f
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
	f.[Total obligated WEEE received on behalf of PCS(s) (t)] = t.Tonnage
FROM
	##FinalTable f
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
	f.[Total obligated WEEE sent to another AATF / ATF for treatment (t)]	 = t.Tonnage
FROM
	##FinalTable f
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
	f.[Total obligated WEEE sent to another AATF / ATF for treatment (t)]	 = t.Tonnage
FROM
	##FinalTable f
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
	f.[Total obligated WEEE reused as a whole appliance (t)] = t.Tonnage
FROM
	##FinalTable f
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
	f.[Total obligated WEEE reused as a whole appliance (t)] = t.Tonnage
FROM
	##FinalTable f
	INNER JOIN TotalReusedUpdate t ON t.AatfId = f.AatfKey AND t.CategoryId = f.CategoryId AND f.ObligationType = 1

-- TOTAL RECEIVED PER PCS
SELECT @ColumnName = ISNULL(@ColumnName + ',','') + QUOTENAME(CONCAT('Obligated WEEE received on behalf of ', SchemeName, ' (t)'))
FROM 
(
	SELECT DISTINCT
		s.SchemeName As SchemeName
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
		CONCAT(''Obligated WEEE received on behalf of '', SchemeName, '' (t)'') AS SchemeName,
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
		##PcsObligated
	FROM
		pivotPcsObligated
	PIVOT (MAX(Tonnage) FOR SchemeName IN (' + @ColumnName + ')) AS x'

EXEC (@DynamicPivotQuery)

-- TOTAL SENT ON TO SITE
SET @ColumnName = NULL
SELECT @ColumnName = ISNULL(@ColumnName + ',','') + QUOTENAME(CONCAT('Obligated WEEE sent to ', SiteOperator, ' (t)'))
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
		CONCAT(''Obligated WEEE sent to '', SiteOperator, '' (t)'') AS SiteOperator,
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

	print @DynamicPivotQuery
EXEC (@DynamicPivotQuery)

IF OBJECT_ID('tempdb..##PcsObligated') IS NOT NULL
	SET @HasPcsData = 1

IF OBJECT_ID('tempdb..##SentOnObligated') IS NOT NULL
	SET @HasSentOnData = 1

SET @DynamicPivotQuery = N'
	SELECT
		f.AatfKey,
		f.CategoryId,
		f.[Compliance year],
		f.[Quarter],
		f.[Name of AATF],
		f.[AATF approval number],
		f.[Submitted by],
		f.[Submitted date (GMT)],
		f.Category,
		f.[Obligation type],
		f.[Total obligated WEEE received on behalf of PCS(s) (t)],'

IF @HasPcsData = 1 BEGIN
	SET @DynamicPivotQuery = @DynamicPivotQuery + N'ph.*,'
END

SET @DynamicPivotQuery = @DynamicPivotQuery + N'f.[Total obligated WEEE sent to another AATF / ATF for treatment (t)],'

IF @HasSentOnData = 1 BEGIN
	SET @DynamicPivotQuery = @DynamicPivotQuery + N'so.*,'
END

SET @DynamicPivotQuery = @DynamicPivotQuery + N'f.[Total obligated WEEE reused as a whole appliance (t)]'

SET @DynamicPivotQuery = @DynamicPivotQuery + N'
FROM
	##FinalTable f '

IF @HasPcsData = 1 BEGIN
	SET @DynamicPivotQuery = @DynamicPivotQuery + N'LEFT JOIN ##PcsObligated ph ON ph.CategoryId = f.CategoryId AND ph.AatfId = f.AatfKey AND ph.ObligationType = f.ObligationType '
END
IF @HasSentOnData = 1 BEGIN
	SET @DynamicPivotQuery = @DynamicPivotQuery + N'LEFT JOIN ##SentOnObligated so ON so.CategoryId = f.CategoryId AND so.AatfId = f.AatfKey AND so.ObligationType = f.ObligationType '
END

EXEC (@DynamicPivotQuery)

IF @HasPcsData = 1 BEGIN
	DROP TABLE ##PcsObligated
END
IF @HasSentOnData = 1 BEGIN
	DROP TABLE ##SentOnObligated
END
DROP TABLE ##FinalTable

END