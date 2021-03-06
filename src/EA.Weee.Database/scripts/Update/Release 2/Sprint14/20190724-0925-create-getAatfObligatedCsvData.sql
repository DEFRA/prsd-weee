-- Description:	This stored procedure is used to provide the data for the Download link in the aatf data tab
--				that have/haven't submitted a data return within
--				the limits of the specified parameters.
-- =============================================
CREATE PROCEDURE [AATF].[getAatfObligatedCsvData]
	@ComplianceYear INT,
	@Quarter INT,
	@ReturnId UNIQUEIDENTIFIER,
	@AatfId UNIQUEIDENTIFIER

AS
BEGIN

SET NOCOUNT ON;


DECLARE @SUBMITTEDRETURN TABLE
(
	AatfId					UNIQUEIDENTIFIER NOT NULL,
	ReturnId				UNIQUEIDENTIFIER NOT NULL,
	ComplianceYear			INT NOT NULL,
	[Quarter]				INT NOT NULL,
	CreatedDate				DATETIME NOT NULL,
	SubmittedDate			DATETIME NULL,
	SubmittedBy				NVARCHAR(70) NULL,
	Name					NVARCHAR(256) NOT NULL,
	ApprovalNumber			NVARCHAR(20) NOT NULL
)


DECLARE @ObligatedData TABLE
(
	AatfId					UNIQUEIDENTIFIER NOT NULL,
	ReturnId				UNIQUEIDENTIFIER NOT NULL,
	[Quarter]				INT NOT NULL,
	CategoryId				INT NOT NULL,
	TonnageType				NVARCHAR(20) NOT NULL,
	TotalSent				DECIMAL(35,3) NULL,
	TotalReused					DECIMAL(35,3) NULL,
	TotalReceived			DECIMAL(35,3) NULL
)


DECLARE @TotalReused TABLE
(
	AatfId					UNIQUEIDENTIFIER NOT NULL,
	ReturnId				UNIQUEIDENTIFIER NOT NULL,
	[Quarter]				INT NOT NULL,
	CategoryId				INT NOT NULL,
	TonnageType				NVARCHAR(20) NOT NULL,
	Tonnage					DECIMAL(35,3) NULL
)

IF OBJECT_ID('tempdb..#AatfTotalReceivedByScheme') IS NOT NULL
  DROP TABLE #AatfTotalReceivedByScheme

CREATE TABLE #AatfTotalReceivedByScheme 
(
	AatfId					UNIQUEIDENTIFIER NOT NULL,
	ReturnId				UNIQUEIDENTIFIER NOT NULL,
	[Quarter]				INT NOT NULL,
	CategoryId				INT NOT NULL,
	SchemeId				UNIQUEIDENTIFIER NOT NULL,
	TonnageType				NVARCHAR(20) NOT NULL,
	Tonnage					DECIMAL(35,3) NULL,
	SchemeName				NVARCHAR(70)  NULL,
	SchemeApprovalNumber	NVARCHAR(16)  NULL
)

DECLARE @DynamicPivotQuery AS NVARCHAR(MAX)
DECLARE @ColumnNames nvarchar(MAX)
DECLARE @ColumnName nvarchar(25)

--Get the latest submitted returns for the compliance year
INSERT INTO @SUBMITTEDRETURN
SELECT ra.AatfId, ra.ReturnId, r.ComplianceYear, r.[Quarter], r.CreatedDate, r.SubmittedDate,
	   CONCAT (u.FirstName, ' ', u.Surname) as SubmittedBy, a.Name, a.ApprovalNumber
FROM
 [AATF].[ReturnAatf] ra
INNER JOIN [AATF].[Return] r ON r.Id = ra.ReturnId
AND ra.ReturnId = @ReturnId AND ra.AatfId = @AatfId
INNER JOIN AATF.AATF a ON a.Id = ra.AatfId  AND a.FacilityType = r.FacilityType
INNER JOIN [Identity].[AspNetUsers] u ON u.id = r.SubmittedById
WHERE r.ComplianceYear = @ComplianceYear
	AND r.[Quarter] = @Quarter
	AND r.ReturnStatus = 2  -- submitted

--Total Sent to another AATF / ATF (t)

INSERT INTO @ObligatedData (AatfId, ReturnId, [Quarter], CategoryId, TonnageType, TotalSent)
SELECT u.AatfId, u.ReturnId, u.[Quarter], u.CategoryId, u.Tonnage, SUM(u.value) AS VALUE
FROM (
	SELECT r.AatfId, r.ReturnId, r.[Quarter], wsoa.CategoryId,
	  COALESCE(wsoa.HouseholdTonnage, 0) HouseholdTonnage,
	  COALESCE(wsoa.NonHouseholdTonnage, 0) NonHouseholdTonnage
	FROM @SUBMITTEDRETURN r
	INNER JOIN [AATF].WeeeSentOn wso ON r.ReturnId = wso.ReturnId
		AND wso.AatfId = r.AatfId
	INNER JOIN [AATF].WeeeSentOnAmount wsoa ON wso.Id = wsoa.WeeeSentOnId
	) a
	UNPIVOT(value FOR Tonnage IN (a.HouseholdTonnage, a.NonHouseholdTonnage)) u
GROUP BY AatfId, ReturnId, [Quarter], Tonnage, CategoryId
ORDER BY AatfId, ReturnId, [Quarter], Tonnage, CategoryId



	--Reused as a whole appliance (t)
INSERT INTO @TotalReused
SELECT u.AatfId, u.ReturnId, u.[Quarter], u.CategoryId, u.Tonnage AS TonnageType, SUM(u.value) AS VALUE
FROM (
	SELECT r.AatfId, r.ReturnId, r.[Quarter], wra.CategoryId, 
		COALESCE(wra.HouseholdTonnage, 0) HouseholdTonnage,
		COALESCE(wra.NonHouseholdTonnage, 0) NonHouseholdTonnage
	FROM @SUBMITTEDRETURN r
	INNER JOIN [AATF].WeeeReused wr ON r.ReturnId = wr.ReturnId
		AND wr.AatfId = r.AatfId
	INNER JOIN [AATF].WeeeReusedAmount wra ON wr.Id = wra.WeeeReusedId
	) a
	UNPIVOT(value FOR Tonnage IN (a.HouseholdTonnage ,a.NonHouseholdTonnage)) u
GROUP BY AatfId, ReturnId, [Quarter], Tonnage, CategoryId


--Update for matched records
UPDATE @ObligatedData
SET TotalReused = x.Tonnage
FROM @ObligatedData o
INNER JOIN @TotalReused x ON x.AatfId = o.AatfId
	AND x.ReturnId = o.ReturnId
	AND x.[Quarter] = o.[Quarter]
	AND x.CategoryId = o.CategoryId
	AND x.TonnageType = o.TonnageType



--Insert for non-matched records
INSERT INTO @ObligatedData (AatfId, ReturnId, [Quarter], CategoryId, TonnageType, TotalReused)
SELECT * FROM @TotalReused
EXCEPT
SELECT AatfId, ReturnId, [Quarter], CategoryId, TonnageType, TotalReused FROM @ObligatedData



--Total received from PCS (t)
INSERT INTO #AatfTotalReceivedByScheme
SELECT u.AatfId, u.ReturnId, u.[Quarter], u.CategoryId, u.SchemeId, u.Tonnage, SUM(u.value) AS VALUE, u.SchemeName, u.ApprovalNumber
FROM (
	SELECT wr.SchemeId, r.AatfId, r.ReturnId, r.[Quarter], wra.CategoryId,
	 wra.HouseholdTonnage ,
	 wra.NonHouseholdTonnage, s.SchemeName, s.ApprovalNumber
	FROM @SUBMITTEDRETURN r
	INNER JOIN [AATF].WeeeReceived wr ON r.ReturnId = wr.ReturnId
		AND wr.AatfId = r.AatfId
	INNER JOIN [AATF].WeeeReceivedAmount wra ON wr.Id = wra.WeeeReceivedId
	INNER JOIN [PCS].[Scheme] S ON s.Id = wr.SchemeId
	) a
	UNPIVOT(value FOR Tonnage IN (a.HouseholdTonnage, a.NonHouseholdTonnage)) u
GROUP BY AatfId, ReturnId, [Quarter], Tonnage, CategoryId, u.SchemeId, u.SchemeName, u.ApprovalNumber



--Update for matched records

UPDATE @ObligatedData
SET TotalReceived = x.Tonnage
FROM @ObligatedData o
INNER JOIN (
	SELECT AatfId, ReturnId, [Quarter], CategoryId, TonnageType, SUM(tonnage) AS Tonnage
	FROM #AatfTotalReceivedByScheme
	GROUP BY AatfId, ReturnId, [Quarter], TonnageType, CategoryId
	) x ON x.AatfId = o.AatfId
	AND x.ReturnId = o.ReturnId
	AND x.[Quarter] = o.[Quarter]
	AND x.CategoryId = o.CategoryId
	AND x.TonnageType = o.TonnageType


--Insert for non-matched records
INSERT INTO @ObligatedData (AatfId, ReturnId, [Quarter], CategoryId, TonnageType, TotalReceived)
SELECT *
FROM (
	SELECT AatfId, ReturnId, [Quarter], CategoryId, TonnageType, SUM(tonnage) AS Tonnage
	FROM #AatfTotalReceivedByScheme
	WHERE AatfId NOT IN (SELECT DISTINCT AatfId FROM @ObligatedData)
	GROUP BY AatfId, ReturnId, [Quarter], TonnageType, CategoryId
	) y
EXCEPT
SELECT AatfId, ReturnId, [Quarter], CategoryId, TonnageType, TotalReceived FROM @ObligatedData

-------------End of Total Obligated data by AATF-----------------------------
--Insert for nil and no data submitted return
IF EXISTS(SELECT * FROM @SUBMITTEDRETURN) 
BEGIN
INSERT INTO @ObligatedData (AatfId, ReturnId, [Quarter], CategoryId, TonnageType)
SELECT s.AatfId, s.ReturnId, s.[Quarter], c.Id,'HouseholdTonnage'
FROM [Lookup].WeeeCategory c 
LEFT JOIN @SUBMITTEDRETURN s
ON 1=1 
WHERE s.AatfId NOT IN (SELECT DISTINCT AatfId FROM @ObligatedData)
UNION ALL
SELECT s.AatfId, s.ReturnId, s.[Quarter], c.Id,'NonHouseholdTonnage'
FROM [Lookup].WeeeCategory c 
LEFT JOIN @SUBMITTEDRETURN s
ON 1=1 
WHERE s.AatfId NOT IN (SELECT DISTINCT AatfId FROM @ObligatedData)
END
-----------------------------------------------------------------------------------

DECLARE @COUNT INT

SELECT @COUNT = COUNT(*) FROM #AatfTotalReceivedByScheme

IF @COUNT > 0
BEGIN

------------- Obligated data by schemes

SELECT @ColumnNames = ISNULL(@ColumnNames + ',', '') + QUOTENAME(SchemeName)
FROM (
	SELECT DISTINCT SchemeName
	FROM #AatfTotalReceivedByScheme
	) A

SET @DynamicPivotQuery = N'

IF OBJECT_ID(''tempdb..##AatfResults'') IS NOT NULL
  DROP TABLE ##AatfResults

SELECT * into ##AatfResults
FROM
	(
	SELECT AatfId,ReturnId,[Quarter] AS ''Q'',CategoryId,TonnageType, ' + @ColumnNames + '
	FROM (SELECT AatfId,ReturnId,[Quarter],CategoryId,TonnageType,Tonnage,SchemeName FROM #AatfTotalReceivedByScheme) as t
	PIVOT (MAX(Tonnage) FOR SchemeName in (' + @ColumnNames + ')) AS PVTTable
	)a'

EXEC sp_executesql @DynamicPivotQuery; 

IF OBJECT_ID('tempdb..#Aatftemp') IS NOT NULL
  DROP TABLE #Aatftemp

select * into #Aatftemp from ##AatfResults;

DROP TABLE ##AatfResults;

SELECT 
@ComplianceYear AS 'Year'
,CONCAT('Q', a.[Quarter]) AS 'Quarter'
,a.Name AS 'Name of AATF'
,a.ApprovalNumber AS 'Approval number'
,a.SubmittedBy AS 'Submitted by'
,a.SubmittedDate AS 'Date submitted (GMT)'
,CONCAT(c.Id, '.', c.Name) AS Category
,a.Obligation
,a.TotalSent AS 'Total sent to another AATF / ATF (t)'
,a.TotalReused AS 'Reused as a whole appliance (t)'
,a.TotalReceived AS 'Total received on behalf of PCS(s) (t)'
, x.*
FROM
	(
	SELECT
	 o.AatfId, r.[Quarter], r.SubmittedBy, r.SubmittedDate, r.Name, r.ApprovalNumber, o.CategoryId, o.returnId,o.TonnageType,
	 CASE WHEN o.TonnageType = 'HouseholdTonnage' THEN 'B2C'
	 ELSE 'B2B' END AS Obligation,
	 o.TotalSent, o.TotalReused, o.TotalReceived
	FROM @ObligatedData o		
	LEFT JOIN @SUBMITTEDRETURN r ON r.AatfId = o.AatfId
		AND R.[Quarter] = o.[Quarter]
		AND R.ReturnId = o.ReturnId
	) a
	INNER JOIN [Lookup].WeeeCategory c ON a.CategoryId = c.Id
	LEFT JOIN #Aatftemp x ON x.AatfId = a.AatfId
		AND x.ReturnId = a.ReturnId
		AND x.Q = a.[Quarter]
		AND x.CategoryId = a.CategoryId
		AND x.TonnageType = a.TonnageType
ORDER BY a.[Quarter], a.Name, a.SubmittedDate, a.TonnageType, a.CategoryId


DROP Table #Aatftemp
END
ELSE
	BEGIN

		SELECT 
		@ComplianceYear AS 'Year'
		,CONCAT('Q', a.[Quarter]) AS 'Quarter'
		,a.Name AS 'Name of AATF'
		,a.ApprovalNumber AS 'Approval number'
		,a.SubmittedBy AS 'Submitted by'
		,a.SubmittedDate AS 'Date submitted (GMT)'
		,CONCAT(c.Id, '.', c.Name) AS Category
		,a.Obligation
		,a.TotalSent AS 'Total sent to another AATF / ATF (t)'
		,a.TotalReused AS 'Reused as a whole appliance (t)'
		,a.TotalReceived AS 'Total received on behalf of PCS(s) (t)'
		FROM
			(
			SELECT
			 o.AatfId, r.[Quarter], r.SubmittedBy, r.SubmittedDate, r.Name, r.ApprovalNumber, o.CategoryId, o.returnId,o.TonnageType,
			 CASE WHEN o.TonnageType = 'HouseholdTonnage' THEN 'B2C'
			 ELSE 'B2B' END AS Obligation,
			 o.TotalSent, o.TotalReused, o.TotalReceived
			FROM @ObligatedData o		
			LEFT JOIN @SUBMITTEDRETURN r ON r.AatfId = o.AatfId
				AND R.[Quarter] = o.[Quarter]
				AND R.ReturnId = o.ReturnId
			) a
			INNER JOIN [Lookup].WeeeCategory c ON a.CategoryId = c.Id
		ORDER BY a.[Quarter], a.Name, a.SubmittedDate, a.TonnageType, a.CategoryId
	END

DROP Table #AatfTotalReceivedByScheme


END



GO

