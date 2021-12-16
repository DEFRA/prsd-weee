GO
CREATE FUNCTION fn_CategoryName(@id INT, @name VARCHAR(200))  
RETURNS VARCHAR(200)  
AS  
BEGIN  
DECLARE @categoryName VARCHAR(200)  
SET @categoryName=(CONCAT(CASE 
			WHEN @id <= 9 THEN ('0'  + CAST(@id AS NVARCHAR(2))) 
		ELSE 
			CAST(@id AS NVARCHAR(2)) 
		END, '. ', @name))  
RETURN @categoryName  
END  


GO

/****** Object:  StoredProcedure [AATF].[getAatfObligatedCsvData]    Script Date: 10/12/2021 14:54:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [AATF].[getAatfObligatedCsvData]
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
EXCEPT
SELECT DISTINCT AatfId, ReturnId, [Quarter], CategoryId, TonnageType FROM @ObligatedData
UNION ALL
SELECT s.AatfId, s.ReturnId, s.[Quarter], c.Id,'NonHouseholdTonnage'
FROM [Lookup].WeeeCategory c 
LEFT JOIN @SUBMITTEDRETURN s
ON 1=1 
EXCEPT
SELECT DISTINCT AatfId, ReturnId, [Quarter], CategoryId, TonnageType FROM @ObligatedData
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
@ComplianceYear AS 'Compliance year'
,CONCAT('Q', a.[Quarter]) AS 'Quarter'
,a.Name AS 'Name of AATF'
,a.ApprovalNumber AS 'Approval number'
,a.SubmittedBy AS 'Submitted by'
,a.SubmittedDate AS 'Date submitted (GMT)'
,dbo.fn_CategoryName(c.Id, c.Name) AS Category
,a.Obligation AS 'Obligation type'
,a.TotalSent AS 'Total sent to another AATF / ATF (t)'
,a.TotalReused AS 'Total reused as a whole appliance (t)'
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
		@ComplianceYear AS ' Compliance year'
		,CONCAT('Q', a.[Quarter]) AS 'Quarter'
		,a.Name AS 'Name of AATF'
		,a.ApprovalNumber AS 'Approval number'
		,a.SubmittedBy AS 'Submitted by'
		,a.SubmittedDate AS 'Date submitted (GMT)'
		,dbo.fn_CategoryName(c.Id, c.Name) AS Category
		,a.Obligation AS 'Obligation type'
		,a.TotalSent AS 'Total sent to another AATF / ATF (t)'
		,a.TotalReused AS 'Total reused as a whole appliance (t)'
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

/****** Object:  StoredProcedure [Producer].[SpgUKEEEDataByComplianceYear]    Script Date: 10/12/2021 14:05:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [Producer].[SpgUKEEEDataByComplianceYear]
	@ComplianceYear INT
AS
BEGIN

-- Weee category
CREATE TABLE #WeeeCategory(
 ID int,
 Name nvarchar(250))

INSERT INTO #WeeeCategory (ID, Name)
SELECT
	Id,
	dbo.fn_CategoryName(Id, Name) AS [Name]
FROM
	[Lookup].WeeeCategory w

-- EEE data	
SELECT
		EEOA.WeeeCategory,
		DR.Quarter,
		EEOA.ObligationType,
		EEOA.Tonnage
INTO #EEETable

	from [PCS].DataReturn DR

	INNER JOIN [PCS].DataReturnVersion DRV 
		on DR.CurrentDataReturnVersionId = DRV.Id

	INNER JOIN [PCS].[EeeOutputReturnVersion] EEORV
		on DRV.EeeOutputReturnVersionId = EEORV.Id

	INNER JOIN [PCS].[EeeOutputReturnVersionAmount] EEORVA
		on EEORV.Id = EEORVA.EeeOutputReturnVersionId

	INNER JOIN [PCS].[EeeOutputAmount] EEOA
		on EEORVA.EeeOuputAmountId = EEOA.Id

	INNER JOIN [Producer].[RegisteredProducer] RP
		on EEOA.RegisteredProducerId = RP.Id

	where DR.ComplianceYear = @ComplianceYear
		AND
		RP.Removed = 0

SELECT
EEEData.ID, EEEData.Name, EEEData.ObligationType, [1] AS Q1, [2] AS Q2, [3] AS Q3, [4] AS Q4
INTO #EEEDataTable
 FROM
 (
   select 
		WC.ID,
		WC.Name,
		EE.Quarter,
		EE.ObligationType,
		EE.Tonnage
	from Lookup.WeeeCategory WC
		LEFT JOIN #EEETable EE
		ON WC.ID = EE.WeeeCategory

 ) AS EE
PIVOT
(
  Sum(Tonnage) FOR Quarter IN ([1], [2], [3], [4])
) AS EEEData


SELECT WC.ID, WC.Name, EEEDataForB2B.Q1, EEEDataForB2B.Q2, EEEDataForB2B.Q3, EEEDataForB2B.Q4
INTO #EEEDataForB2BTable
FROM #WeeeCategory WC
	LEFT JOIN #EEEDataTable EEEDataForB2B
		ON WC.ID = EEEDataForB2B.ID AND EEEDataForB2B.ObligationType = 'B2B'


SELECT WC.ID, WC.Name, EEEDataForB2C.Q1, EEEDataForB2C.Q2, EEEDataForB2C.Q3, EEEDataForB2C.Q4
INTO #EEEDataForB2CTable
FROM #WeeeCategory WC
	LEFT JOIN #EEEDataTable EEEDataForB2C
		ON WC.ID = EEEDataForB2C.ID AND EEEDataForB2C.ObligationType = 'B2C'


SELECT 
EEEDataForB2B.Name AS 'Category', 
ISNULL(EEEDataForB2B.Q1, 0) + ISNULL(EEEDataForB2B.Q2, 0) + ISNULL( EEEDataForB2B.Q3, 0) + ISNULL(EEEDataForB2B.Q4, 0) AS 'TotalB2BEEE',
EEEDataForB2B.Q1 AS 'Q1B2BEEE',
EEEDataForB2B.Q2 AS 'Q2B2BEEE',
EEEDataForB2B.Q3 AS 'Q3B2BEEE',
EEEDataForB2B.Q4 AS 'Q4B2BEEE',
ISNULL(EEEDataForB2C.Q1, 0) + ISNULL(EEEDataForB2C.Q2, 0) + ISNULL(EEEDataForB2C.Q3, 0) + ISNULL(EEEDataForB2C.Q4, 0) AS 'TotalB2CEEE',
EEEDataForB2C.Q1 AS 'Q1B2CEEE',
EEEDataForB2C.Q2 AS 'Q2B2CEEE',
EEEDataForB2C.Q3 AS 'Q3B2CEEE',
EEEDataForB2C.Q4 AS 'Q4B2CEEE'
FROM #EEEDataForB2BTable EEEDataForB2B
INNER JOIN #EEEDataForB2CTable EEEDataForB2C
ON EEEDataForB2B.ID = EEEDataForB2C.ID
ORDER BY EEEDataForB2B.ID

END
GO


GO
/****** Object:  StoredProcedure [AATF].[getAllAatfObligatedCsvData]    Script Date: 13/12/2021 09:43:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [AATF].[getAllAatfObligatedCsvData]
	@ComplianceYear INT,
	@AatfName nvarchar(256),
	@ObligationType nvarchar(3),
	@CA UNIQUEIDENTIFIER,
	@PanArea UNIQUEIDENTIFIER,
	@ColumnType INT
AS
BEGIN

SET NOCOUNT ON;

IF OBJECT_ID('tempdb..#SUBMITTEDRETURN') IS NOT NULL
  DROP TABLE #TotalReceivedByScheme

IF OBJECT_ID('tempdb..#ObligatedData') IS NOT NULL
  DROP TABLE #TotalReceivedByScheme

IF OBJECT_ID('tempdb..#TotalReused') IS NOT NULL
  DROP TABLE #TotalReceivedByScheme

CREATE TABLE #SUBMITTEDRETURN
(
	AatfId					UNIQUEIDENTIFIER NOT NULL,
	ReturnId				UNIQUEIDENTIFIER NOT NULL,
	ComplianceYear			INT NOT NULL,
	[Quarter]				INT NOT NULL,
	CreatedDate				DATETIME NOT NULL,
	SubmittedDate			DATETIME NULL,
	SubmittedBy				NVARCHAR(70) NULL,
	Name					NVARCHAR(256) NOT NULL,
	ApprovalNumber			NVARCHAR(20) NOT NULL,	
	OrganisationName		NVARCHAR(256) NOT NULL,
	CompetentAuthorityAbbr	NVARCHAR(65) NOT NULL,
	PanArea					NVARCHAR(200) NULL,
	LocalArea				NVARCHAR(1024) NULL
)


CREATE TABLE #ObligatedData
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


CREATE TABLE #TotalReused
(
	AatfId					UNIQUEIDENTIFIER NOT NULL,
	ReturnId				UNIQUEIDENTIFIER NOT NULL,
	[Quarter]				INT NOT NULL,
	CategoryId				INT NOT NULL,
	TonnageType				NVARCHAR(20) NOT NULL,
	Tonnage					DECIMAL(35,3) NULL
)

IF OBJECT_ID('tempdb..#TotalReceivedByScheme') IS NOT NULL
  DROP TABLE #TotalReceivedByScheme

CREATE TABLE #TotalReceivedByScheme 
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
INSERT INTO #SUBMITTEDRETURN
SELECT X.AatfId, X.ReturnId, X.ComplianceYear, X.[Quarter], X.CreatedDate, X.SubmittedDate,
	   X.SubmittedBy, X.Name, X.ApprovalNumber,X.OrgName,
	    X.Abbreviation, X.PName, X.LaName FROM
(
SELECT ra.AatfId, ra.ReturnId, r.ComplianceYear, r.[Quarter], r.CreatedDate, r.SubmittedDate,
	   CONCAT (u.FirstName, ' ', u.Surname) as SubmittedBy, a.Name, a.ApprovalNumber, CASE 
		WHEN o.Name IS NULL
			THEN o.TradingName
		ELSE o.Name
		END as OrgName, ca.Abbreviation, pa.Name as PName, la.Name as LaName,ROW_NUMBER() OVER
						(
							PARTITION BY ra.AatfId, r.[Quarter]
							ORDER BY r.[Quarter],r.SubmittedDate desc
						) AS RowNumber
FROM
 [AATF].[ReturnAatf] ra
INNER JOIN [AATF].[Return] r ON r.Id = ra.ReturnId
INNER JOIN AATF.AATF a ON a.Id = ra.AatfId  AND a.FacilityType = r.FacilityType
INNER JOIN Organisation.Organisation o ON a.OrganisationId = o.Id
INNER JOIN [Lookup].CompetentAuthority ca ON a.CompetentAuthorityId = ca.Id
INNER JOIN [Identity].[AspNetUsers] u ON u.id = r.SubmittedById
LEFT JOIN [Lookup].[LocalArea] la ON la.Id = a.LocalAreaId
LEFT JOIN [Lookup].[PanArea] pa ON pa.Id = a.PanAreaId
WHERE r.ComplianceYear = @ComplianceYear
	AND r.ReturnStatus = 2  -- submitted
	AND a.FacilityType = 1  --aatf
	AND a.CompetentAuthorityId = COALESCE(@CA, a.CompetentAuthorityId)
	AND (
		@PanArea IS NULL
		OR a.PanAreaId = COALESCE(@PanArea, a.PanAreaId)
		)
	AND (
		@AatfName IS NULL
		OR a.Name LIKE '%' + COALESCE(@AatfName, a.Name) + '%'
		)
) X
WHERE X.RowNumber = 1
OPTION (OPTIMIZE FOR (@ComplianceYear UNKNOWN))

--Total Sent to another AATF / ATF (t)

INSERT INTO #ObligatedData (AatfId, ReturnId, [Quarter], CategoryId, TonnageType, TotalSent)
SELECT u.AatfId, u.ReturnId, u.[Quarter], u.CategoryId, u.Tonnage, SUM(u.value) AS VALUE
FROM (
	SELECT r.AatfId, r.ReturnId, r.[Quarter], wsoa.CategoryId,
	  COALESCE(wsoa.HouseholdTonnage, 0) HouseholdTonnage,
	  COALESCE(wsoa.NonHouseholdTonnage, 0) NonHouseholdTonnage
	FROM #SUBMITTEDRETURN r
	INNER JOIN [AATF].WeeeSentOn wso ON r.ReturnId = wso.ReturnId
		AND wso.AatfId = r.AatfId
	INNER JOIN [AATF].WeeeSentOnAmount wsoa ON wso.Id = wsoa.WeeeSentOnId
	) a
	UNPIVOT(value FOR Tonnage IN (a.HouseholdTonnage, a.NonHouseholdTonnage)) u
GROUP BY AatfId, ReturnId, [Quarter], Tonnage, CategoryId
ORDER BY AatfId, ReturnId, [Quarter], Tonnage, CategoryId



	--Reused as a whole appliance (t)
INSERT INTO #TotalReused
SELECT u.AatfId, u.ReturnId, u.[Quarter], u.CategoryId, u.Tonnage AS TonnageType, SUM(u.value) AS VALUE
FROM (
	SELECT r.AatfId, r.ReturnId, r.[Quarter], wra.CategoryId, 
		COALESCE(wra.HouseholdTonnage, 0) HouseholdTonnage,
		COALESCE(wra.NonHouseholdTonnage, 0) NonHouseholdTonnage
	FROM #SUBMITTEDRETURN r
	INNER JOIN [AATF].WeeeReused wr ON r.ReturnId = wr.ReturnId
		AND wr.AatfId = r.AatfId
	INNER JOIN [AATF].WeeeReusedAmount wra ON wr.Id = wra.WeeeReusedId
	) a
	UNPIVOT(value FOR Tonnage IN (a.HouseholdTonnage ,a.NonHouseholdTonnage)) u
GROUP BY AatfId, ReturnId, [Quarter], Tonnage, CategoryId

--Update for matched records
UPDATE #ObligatedData
SET TotalReused = x.Tonnage
FROM #ObligatedData o
INNER JOIN #TotalReused x ON x.AatfId = o.AatfId
	AND x.ReturnId = o.ReturnId
	AND x.[Quarter] = o.[Quarter]
	AND x.CategoryId = o.CategoryId
	AND x.TonnageType = o.TonnageType

--Insert for non-matched records
INSERT INTO #ObligatedData (AatfId, ReturnId, [Quarter], CategoryId, TonnageType, TotalReused)
SELECT * FROM #TotalReused
EXCEPT
SELECT AatfId, ReturnId, [Quarter], CategoryId, TonnageType, TotalReused FROM #ObligatedData

--Total received from PCS (t)
INSERT INTO #TotalReceivedByScheme
SELECT u.AatfId, u.ReturnId, u.[Quarter], u.CategoryId, u.SchemeId, u.Tonnage, SUM(u.value) AS VALUE, u.SchemeName, u.ApprovalNumber
FROM (
	SELECT wr.SchemeId, r.AatfId, r.ReturnId, r.[Quarter], wra.CategoryId,
	 COALESCE(wra.HouseholdTonnage, 0) HouseholdTonnage,
	 COALESCE(wra.NonHouseholdTonnage, 0) NonHouseholdTonnage, s.SchemeName, s.ApprovalNumber
	FROM #SUBMITTEDRETURN r
	INNER JOIN [AATF].WeeeReceived wr ON r.ReturnId = wr.ReturnId
		AND wr.AatfId = r.AatfId
	INNER JOIN [AATF].WeeeReceivedAmount wra ON wr.Id = wra.WeeeReceivedId
	INNER JOIN [PCS].[Scheme] S ON s.Id = wr.SchemeId
	) a
	UNPIVOT(value FOR Tonnage IN (a.HouseholdTonnage, a.NonHouseholdTonnage)) u
GROUP BY AatfId, ReturnId, [Quarter], Tonnage, CategoryId, u.SchemeId, u.SchemeName, u.ApprovalNumber

--Update for matched records

UPDATE #ObligatedData
SET TotalReceived = x.Tonnage
FROM #ObligatedData o
INNER JOIN (
	SELECT AatfId, ReturnId, [Quarter], CategoryId, TonnageType, SUM(tonnage) AS Tonnage
	FROM #TotalReceivedByScheme
	GROUP BY AatfId, ReturnId, [Quarter], TonnageType, CategoryId
	) x ON x.AatfId = o.AatfId
	AND x.ReturnId = o.ReturnId
	AND x.[Quarter] = o.[Quarter]
	AND x.CategoryId = o.CategoryId
	AND x.TonnageType = o.TonnageType

--Insert for non-matched records
INSERT INTO #ObligatedData (AatfId, ReturnId, [Quarter], CategoryId, TonnageType, TotalReceived)
SELECT *
FROM (
	SELECT AatfId, ReturnId, [Quarter], CategoryId, TonnageType, SUM(tonnage) AS Tonnage
	FROM #TotalReceivedByScheme
	GROUP BY AatfId, ReturnId, [Quarter], TonnageType, CategoryId
	) y
EXCEPT
SELECT AatfId, ReturnId, [Quarter], CategoryId, TonnageType, TotalReceived FROM #ObligatedData

-------------End of Total Obligated data by AATF-----------------------------
--Insert for nil and no data submitted return
IF EXISTS(SELECT * FROM #SUBMITTEDRETURN) 
BEGIN
INSERT INTO #ObligatedData (AatfId, ReturnId, [Quarter], CategoryId, TonnageType)
SELECT s.AatfId, s.ReturnId, s.[Quarter], c.Id,'HouseholdTonnage'
FROM [Lookup].WeeeCategory c 
LEFT JOIN #SUBMITTEDRETURN s
ON 1=1 
EXCEPT
SELECT DISTINCT AatfId, ReturnId, [Quarter], CategoryId, TonnageType FROM #ObligatedData
UNION ALL
SELECT s.AatfId, s.ReturnId, s.[Quarter], c.Id,'NonHouseholdTonnage'
FROM [Lookup].WeeeCategory c 
LEFT JOIN #SUBMITTEDRETURN s
ON 1=1 
EXCEPT
SELECT DISTINCT AatfId, ReturnId, [Quarter], CategoryId, TonnageType FROM #ObligatedData
END
-----------------------------------------------------------------------------------

DECLARE @COUNT INT

SELECT @COUNT = COUNT(*) FROM #TotalReceivedByScheme

IF @COUNT > 0
BEGIN

------------- Obligated data by schemes
IF @ColumnType = 1
	BEGIN

	SET @ColumnName = 'SchemeName'

	SELECT @ColumnNames = ISNULL(@ColumnNames + ',', '') + QUOTENAME(SchemeName)
	FROM (
		SELECT DISTINCT SchemeName
		FROM #TotalReceivedByScheme
		) A
	END
ELSE
	BEGIN
	SET @ColumnName = 'SchemeApprovalNumber'
	SELECT @ColumnNames = ISNULL(@ColumnNames + ',', '') + QUOTENAME(SchemeApprovalNumber)
	FROM (
		SELECT DISTINCT SchemeApprovalNumber
		FROM #TotalReceivedByScheme
		) A
	END

SET @DynamicPivotQuery = N'

IF OBJECT_ID(''tempdb..##Results'') IS NOT NULL
  DROP TABLE ##Results

SELECT * into ##Results
FROM
	(
	SELECT AatfId,ReturnId,[Quarter] AS ''Q'',CategoryId,TonnageType, ' + @ColumnNames + '
	FROM (SELECT AatfId,ReturnId,[Quarter],CategoryId,TonnageType,Tonnage,'+ @ColumnName +' FROM #TotalReceivedByScheme) as t
	PIVOT (MAX(Tonnage) FOR '+ @ColumnName +' in (' + @ColumnNames + ')) AS PVTTable
	)a'

EXEC sp_executesql @DynamicPivotQuery; 

IF OBJECT_ID('tempdb..#temp') IS NOT NULL
  DROP TABLE #temp

select * into #temp from ##Results;

DROP TABLE ##Results;

SELECT 
 a.CompetentAuthorityAbbr AS 'Appropriate authority'
,a.PanArea AS 'WROS Pan Area Team'
,a.LocalArea AS 'EA Area'
,@ComplianceYear AS 'Compliance year'
,CONCAT('Q', a.[Quarter]) AS 'Quarter'
,a.SubmittedBy AS 'Submitted by'
,a.SubmittedDate AS 'Date submitted (GMT)'
,a.OrganisationName AS 'Organisation name'
,a.Name AS 'Name of AATF'
,a.ApprovalNumber AS 'Approval number'
,dbo.fn_CategoryName(c.Id, c.Name) AS Category
,a.Obligation AS 'Obligation type'
,a.TotalSent AS 'Total sent to another AATF / ATF (t)'
,a.TotalReused AS 'Total reused as a whole appliance (t)'
,a.TotalReceived AS 'Total received on behalf of PCS(s) (t)'
, x.*
FROM
	(
	SELECT
	 o.AatfId, r.CompetentAuthorityAbbr, r.PanArea,  r.[Quarter], r.SubmittedBy, r.SubmittedDate,r.LocalArea,
	 r.OrganisationName, r.Name, r.ApprovalNumber, o.CategoryId, o.returnId,o.TonnageType,
	 CASE WHEN o.TonnageType = 'HouseholdTonnage' THEN 'B2C'
	 ELSE 'B2B' END AS Obligation,
	 o.TotalSent, o.TotalReused, o.TotalReceived
	FROM #ObligatedData o		
	LEFT JOIN #SUBMITTEDRETURN r ON r.AatfId = o.AatfId
		AND R.[Quarter] = o.[Quarter]
		AND R.ReturnId = o.ReturnId
	) a
	INNER JOIN [Lookup].WeeeCategory c ON a.CategoryId = c.Id
	LEFT JOIN #temp x ON x.AatfId = a.AatfId
		AND x.ReturnId = a.ReturnId
		AND x.Q = a.[Quarter]
		AND x.CategoryId = a.CategoryId
		AND x.TonnageType = a.TonnageType
WHERE (@ObligationType IS NULL OR a.Obligation like '%' + COALESCE(@ObligationType, a.Obligation) + '%')
ORDER BY a.CompetentAuthorityAbbr, a.[Quarter], a.Name, a.SubmittedDate, a.TonnageType, a.CategoryId


DROP Table #temp
END
ELSE
	BEGIN

		SELECT 
		 a.CompetentAuthorityAbbr AS 'Appropriate authority'
		,a.PanArea AS 'WROS Pan Area Team'
		,a.LocalArea AS 'EA Area'
		,@ComplianceYear AS 'Year'
		,CONCAT('Q', a.[Quarter]) AS 'Quarter'
		,a.SubmittedBy AS 'Submitted by'
		,a.SubmittedDate AS 'Date submitted (GMT)'
		,a.OrganisationName AS 'Organisation name'
		,a.Name AS 'Name of AATF'
		,a.ApprovalNumber AS 'Approval number'
		,dbo.fn_CategoryName(c.Id, c.Name) AS Category
		,a.Obligation
		,a.TotalSent AS 'Total sent to another AATF / ATF (t)'
		,a.TotalReused AS 'Reused as a whole appliance (t)'
		,a.TotalReceived AS 'Total received on behalf of PCS(s) (t)'
		FROM
			(
			SELECT
			 o.AatfId, r.CompetentAuthorityAbbr, r.PanArea,  r.[Quarter], r.SubmittedBy, r.SubmittedDate,r.LocalArea,
			 r.OrganisationName, r.Name, r.ApprovalNumber, o.CategoryId, o.returnId,o.TonnageType,
			 CASE WHEN o.TonnageType = 'HouseholdTonnage' THEN 'B2C'
			 ELSE 'B2B' END AS Obligation,
			 o.TotalSent, o.TotalReused, o.TotalReceived
			FROM #ObligatedData o		
			LEFT JOIN #SUBMITTEDRETURN r ON r.AatfId = o.AatfId
				AND R.[Quarter] = o.[Quarter]
				AND R.ReturnId = o.ReturnId
			) a
			INNER JOIN [Lookup].WeeeCategory c ON a.CategoryId = c.Id
		WHERE (@ObligationType IS NULL OR a.Obligation like '%' + COALESCE(@ObligationType, a.Obligation) + '%')
		ORDER BY a.CompetentAuthorityAbbr, a.[Quarter], a.Name, a.SubmittedDate, a.TonnageType, a.CategoryId
	END

DROP Table #TotalReceivedByScheme
DROP TABLE #SUBMITTEDRETURN
DROP TABLE #ObligatedData
DROP TABLE #TotalReused

END


GO
/****** Object:  StoredProcedure [AATF].[getPcsAatfDiscrepancyCsvData]    Script Date: 13/12/2021 10:23:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [AATF].[getPcsAatfDiscrepancyCsvData]
	@ComplianceYear INT,
	@Quarter INT,
	@ObligationType nvarchar(3)
AS
BEGIN

CREATE TABLE #ObligationTypeTable
(
	Obligation CHAR(4)
)

INSERT INTO #ObligationTypeTable SELECT 'B2B'
INSERT INTO #ObligationTypeTable SELECT 'B2C'

CREATE TABLE #SubmittedReturn
(
	AatfId					UNIQUEIDENTIFIER NOT NULL,
	ReturnId				UNIQUEIDENTIFIER NOT NULL,
	ComplianceYear			INT NOT NULL,
	[Quarter]				INT NOT NULL,
	Name					NVARCHAR(256) NOT NULL,
	ApprovalNumber			NVARCHAR(20) NOT NULL,	
	CompetentAuthorityAbbr	NVARCHAR(65) NOT NULL
)

	CREATE TABLE #PCSDelivered
	(
		[SchemeId]					UNIQUEIDENTIFIER,
		ComplianceYear				INT NOT NULL,
		ApprovalNumber				NVARCHAR(16)  NULL,	
		SchemeName					NVARCHAR(70)  NULL,
		CompetentAuthorityAbbr		NVARCHAR(65) NOT NULL,
		[Quarter]					INT,
		[WeeeCategory]				INT,
		[ObligationType]			NVARCHAR(4),
		[AatfApprovalNumber]		NVARCHAR(50),
		AatfName					NVARCHAR(256) NULL,
		AatfCompetentAuthorityAbbr	NVARCHAR(65) NULL,
		[PcsTonnage]				DECIMAL(38, 3) NULL
	)

	CREATE TABLE #AatfObligated
	(
		[SchemeId]					UNIQUEIDENTIFIER,
		SchemeName					NVARCHAR(70)  NULL,
		ComplianceYear				INT NOT NULL,
		Name						NVARCHAR(256) NOT NULL,
		AatfApprovalNumber			NVARCHAR(16)  NULL,	
		CompetentAuthorityAbbr		NVARCHAR(65) NOT NULL,
		[Quarter]					INT,
		[CategoryId]				INT,
		[Obligation]				NVARCHAR(4),
		[ApprovalNumber]			NVARCHAR(50),
		PcsCompetentAuthorityAbbr	NVARCHAR(65) NULL,
		[AatfTonnage]				DECIMAL(38, 3)
	)

INSERT INTO #SubmittedReturn
SELECT X.AatfId, X.ReturnId, X.ComplianceYear, X.[Quarter], X.Name, X.ApprovalNumber, X.Abbreviation
 FROM
	(
	SELECT ra.AatfId, ra.ReturnId, r.ComplianceYear, r.[Quarter], r.CreatedDate, r.SubmittedDate,
			a.Name, a.ApprovalNumber, ca.Abbreviation, ROW_NUMBER() OVER
							(
								PARTITION BY ra.AatfId, r.[Quarter]
								ORDER BY r.[Quarter],r.SubmittedDate desc
							) AS RowNumber
	FROM
	 [AATF].[ReturnAatf] ra
	INNER JOIN [AATF].[Return] r ON r.Id = ra.ReturnId
	INNER JOIN AATF.AATF a ON a.Id = ra.AatfId  AND a.FacilityType = r.FacilityType
	INNER JOIN [Lookup].CompetentAuthority ca ON a.CompetentAuthorityId = ca.Id

	WHERE r.ComplianceYear = @ComplianceYear
		AND r.ReturnStatus = 2  -- submitted
		AND a.FacilityType = 1  --aatf	
		AND (@Quarter = 0 or r.[Quarter] = @Quarter)
	)X
WHERE X.RowNumber = 1


INSERT INTO #AatfObligated
SELECT u.SchemeId, u.SchemeName, u.ComplianceYear,  u.Name, u.AatfApprovalNumber, u.CompetentAuthorityAbbr, u.[Quarter], u.CategoryId, CASE WHEN u.Tonnage = 'HouseholdTonnage' THEN 'B2C'
			 ELSE 'B2B' END AS Obligation,
  u.ApprovalNumber, u.Abbreviation, SUM(u.value) AS AatfTonnage
FROM (
	SELECT r.ComplianceYear, r.Name, r.ApprovalNumber as AatfApprovalNumber, r.CompetentAuthorityAbbr, wr.SchemeId, r.AatfId, r.ReturnId, r.[Quarter], wra.CategoryId,
	 ISNULL(wra.HouseholdTonnage,0) HouseholdTonnage ,
	 ISNULL(wra.NonHouseholdTonnage,0) NonHouseholdTonnage, s.SchemeName, s.ApprovalNumber, ca.Abbreviation
	FROM #SubmittedReturn r
	INNER JOIN [AATF].WeeeReceived wr ON r.ReturnId = wr.ReturnId
		AND wr.AatfId = r.AatfId
	INNER JOIN [AATF].WeeeReceivedAmount wra ON wr.Id = wra.WeeeReceivedId
	INNER JOIN [PCS].[Scheme] S ON s.Id = wr.SchemeId
	INNER JOIN [Lookup].CompetentAuthority ca ON ca.Id = s.CompetentAuthorityId 
	) a
	UNPIVOT(value FOR Tonnage IN (a.HouseholdTonnage, a.NonHouseholdTonnage)) u
GROUP BY ComplianceYear, Name, AatfApprovalNumber, CompetentAuthorityAbbr, AatfId, ReturnId, [Quarter], Tonnage, CategoryId, u.SchemeId, u.SchemeName, u.ApprovalNumber, u.Abbreviation


;WITH ObligationData (ObligationType, CategoryId, CategoryName)
AS (
SELECT 
	o.Obligation,
	w.Id,
	w.Name
FROM
	[Lookup].WeeeCategory w, #ObligationTypeTable o
)
INSERT INTO #PCSDelivered
SELECT DISTINCT
		S.Id,
		DR.[ComplianceYear],
		S.[ApprovalNumber],
		S.[SchemeName],
		ca.Abbreviation,
		DR.[Quarter],
		o.CategoryId,
		o.ObligationType,
		AATF.[ApprovalNumber] AS 'AatfApprovalNumber',
		a.Name,
		ca1.Abbreviation,
		0
	FROM
		[PCS].[DataReturn] DR
	INNER JOIN
		[PCS].[Scheme] S
			ON DR.[SchemeId] = S.[Id]
	INNER JOIN [Lookup].CompetentAuthority ca ON s.CompetentAuthorityId = ca.Id
	INNER JOIN
		[PCS].[DataReturnVersion] DRV
			ON DR.[CurrentDataReturnVersionId] = DRV.[Id]
	INNER JOIN
		[PCS].[WeeeDeliveredReturnVersion] WDRV
			ON DRV.[WeeeDeliveredReturnVersionId] = WDRV.[Id]
	INNER JOIN
		[PCS].[WeeeDeliveredReturnVersionAmount] WDRVA
			ON WDRV.[Id] = WDRVA.[WeeeDeliveredReturnVersionId]
	INNER JOIN
		[PCS].[WeeeDeliveredAmount] WDA
			ON WDRVA.[WeeeDeliveredAmountId] = WDA.[Id] AND WDA.[AatfDeliveryLocationId] IS NOT NULL
	INNER JOIN
		[PCS].[AatfDeliveryLocation] AATF
			ON WDA.[AatfDeliveryLocationId] = AATF.[Id] 
	LEFT JOIN AATF.AATF a ON a.ApprovalNumber = AATF.ApprovalNumber AND A.ComplianceYear = @ComplianceYear AND a.FacilityType = 1
	LEFT JOIN [Lookup].CompetentAuthority ca1 ON ca1.Id = a.CompetentAuthorityId
	, ObligationData o
	WHERE
		DR.[ComplianceYear] = @ComplianceYear
		AND (@Quarter = 0 or DR.[Quarter] = @Quarter)
	ORDER BY
		dr.[Quarter],
		o.ObligationType,
		o.CategoryId,
		s.SchemeName,
		a.[Name]

	UPDATE
		#PCSDelivered
	SET
		PcsTonnage = WDA.[Tonnage]
	FROM
		[PCS].[DataReturn] DR
	INNER JOIN
		[PCS].[Scheme] S
			ON DR.[SchemeId] = S.[Id]
	INNER JOIN [Lookup].CompetentAuthority ca ON s.CompetentAuthorityId = ca.Id
	INNER JOIN
		[PCS].[DataReturnVersion] DRV
			ON DR.[CurrentDataReturnVersionId] = DRV.[Id]
	INNER JOIN
		[PCS].[WeeeDeliveredReturnVersion] WDRV
			ON DRV.[WeeeDeliveredReturnVersionId] = WDRV.[Id]
	INNER JOIN
		[PCS].[WeeeDeliveredReturnVersionAmount] WDRVA
			ON WDRV.[Id] = WDRVA.[WeeeDeliveredReturnVersionId]
	INNER JOIN
		[PCS].[WeeeDeliveredAmount] WDA
			ON WDRVA.[WeeeDeliveredAmountId] = WDA.[Id] AND WDA.[AatfDeliveryLocationId] IS NOT NULL
	INNER JOIN
		[PCS].[AatfDeliveryLocation] AATF
			ON WDA.[AatfDeliveryLocationId] = AATF.[Id]
	LEFT JOIN AATF.AATF a ON a.ApprovalNumber = AATF.ApprovalNumber AND A.ComplianceYear = @ComplianceYear AND a.FacilityType = 1
	INNER JOIN #PCSDelivered p ON p.SchemeId = s.Id AND p.[Quarter] = dr.[Quarter] AND p.WeeeCategory = wda.WeeeCategory AND p.ObligationType = wda.ObligationType AND p.AatfApprovalNumber = AATF.ApprovalNumber
	WHERE
		DR.[ComplianceYear] = @ComplianceYear
		AND (@Quarter = 0 or DR.[Quarter] = @Quarter)


		SELECT 
		@ComplianceYear AS ComplianceYear,
		CASE WHEN A.[Quarter] IS NULL THEN P.[Quarter] ELSE A.[Quarter] END AS [Quarter], 
		CASE WHEN A.Obligation IS NULL THEN P.ObligationType ELSE A.Obligation END AS ObligationType,			
		CASE WHEN A.CategoryId IS NULL THEN c1.Id ELSE c.Id END AS CategoryValue, 
		CASE WHEN A.CategoryId IS NULL THEN dbo.fn_CategoryName(c1.Id, c1.Name) ELSE dbo.fn_CategoryName(c.Id, c.Name) END AS Category, 
		CASE WHEN A.[Quarter] IS NULL THEN CONCAT('Q', P.[Quarter]) ELSE CONCAT('Q', A.[Quarter]) END AS QuarterValue,
		CASE WHEN P.SchemeName IS NULL THEN A.SchemeName ELSE P.SchemeName END AS SchemeNameValue, 
		CASE WHEN P.ApprovalNumber IS NULL THEN A.ApprovalNumber ELSE P.ApprovalNumber END AS PcsApprovalNumber, 
		CASE WHEN P.CompetentAuthorityAbbr IS NULL THEN A.PcsCompetentAuthorityAbbr ELSE P.CompetentAuthorityAbbr END AS PcsAbbreviation,
		CASE WHEN A.Name IS NULL THEN P.AatfName ELSE A.Name END AS AatfName, 
		CASE WHEN A.AatfApprovalNumber IS NULL THEN P.AatfApprovalNumber ELSE A.AatfApprovalNumber END AS AatfApprovalNumber,
		CASE WHEN A.CompetentAuthorityAbbr IS NULL THEN P.AatfCompetentAuthorityAbbr ELSE A.CompetentAuthorityAbbr END AS AatfAbbreviation, 
		ISNULL(P.PcsTonnage,0) PcsTonnage, ISNULL(A.AatfTonnage,0) AatfTonnage,
		ISNULL(P.PcsTonnage,0) - ISNULL(A.AatfTonnage,0) as DifferenceTonnage,
		P.WeeeCategory, A.CategoryId, a.[Name]
		FROM
		#AatfObligated A FULL OUTER JOIN
		#PCSDelivered P ON  P.SchemeId = A.SchemeId
		AND P.[Quarter] = A.[Quarter]
		AND P.AatfApprovalNumber = A.AatfApprovalNumber
		AND P.ApprovalNumber = A.ApprovalNumber
		AND P.WeeeCategory = A.CategoryId
		AND P.ObligationType = a.Obligation
		LEFT JOIN [Lookup].WeeeCategory c ON A.CategoryId = c.Id
		LEFT JOIN [Lookup].WeeeCategory c1 ON P.WeeeCategory = c1.Id
		WHERE ISNULL(P.PcsTonnage, 0) != ISNULL(A.AatfTonnage, 0)
		AND
		(@ObligationType IS NULL
			OR 
			P.ObligationType LIKE '%' + COALESCE(@ObligationType, P.ObligationType) + '%'
			OR A.Obligation LIKE '%' + COALESCE(@ObligationType, A.Obligation) + '%'
			)
		ORDER BY
			[Quarter],
			CASE (CASE WHEN A.Obligation IS NULL THEN P.ObligationType ELSE A.Obligation END) WHEN 'B2C' THEN 0 ELSE 1 END,
			CategoryValue,
			SchemeNameValue,
			AatfName


DROP TABLE #SubmittedReturn
DROP TABLE #AatfObligated
DROP TABLE #PCSDelivered
DROP TABLE #ObligationTypeTable

END

GO
/****** Object:  StoredProcedure [AATF].[getReturnNonObligatedCsvData]    Script Date: 13/12/2021 10:49:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [AATF].[getReturnNonObligatedCsvData]
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
	dbo.fn_CategoryName(c.Id, c.Name),
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
	f.[TotalNonObligatedWeeeReceivedFromDcf] = CASE t.Tonnage WHEN 0 THEN NULL ELSE t.Tonnage END
FROM
	@FinalTable f
	INNER JOIN TotalNonObligated t ON t.CategoryId = f.CategoryId

SELECT
	*
FROM
	@FinalTable

END


GO
/****** Object:  StoredProcedure [AATF].[getReturnObligatedCsvData]    Script Date: 13/12/2021 10:52:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [AATF].[getReturnObligatedCsvData]
	@ReturnId UNIQUEIDENTIFIER
AS
BEGIN

DECLARE @DynamicPivotQuery AS NVARCHAR(MAX)
DECLARE @ColumnName AS NVARCHAR(MAX)
DECLARE @HasPcsData BIT
DECLARE @HasSentOnData BIT
DECLARE @SelectedReceived BIT
DECLARE @SelectedSentOn BIT
DECLARE @SelectedReused BIT
DECLARE @AatfReportDate DATETIME
DECLARE @Status INT

SET @SelectedReceived = 0
SET @SelectedSentOn = 0
SET @SelectedReused = 0
SET @HasPcsData = 0
SET @HasSentOnData = 0

IF EXISTS (SELECT * FROM [AATF].ReturnReportOn WHERE ReturnId = @ReturnId AND ReportOnQuestionId = 1) BEGIN
	SET @SelectedReceived = 1
END

IF EXISTS (SELECT * FROM [AATF].ReturnReportOn WHERE ReturnId = @ReturnId AND ReportOnQuestionId = 2) BEGIN
	SET @SelectedSentOn = 1
END

IF EXISTS (SELECT * FROM [AATF].ReturnReportOn WHERE ReturnId = @ReturnId AND ReportOnQuestionId = 3) BEGIN
	SET @SelectedReused = 1
END

DECLARE @ObligationType TABLE
(
	Obligation INT
)
INSERT INTO @ObligationType SELECT 0 -- Household / B2C
INSERT INTO @ObligationType SELECT 1 -- Non house hold / B2B

SELECT
	@AatfReportDate = DATEFROMPARTS(CASE AddStartYears WHEN 1 THEN r.ComplianceYear + 1 ELSE r.ComplianceYear END, l.StartMonth, l.StartDay),
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
		ra.AatfId,
		ReturnId
	FROM
		[AATF].ReturnAatf ra
		INNER JOIN [AATF].AATF a ON a.Id = ra.AatfId
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
			AND a.ComplianceYear = r.ComplianceYear AND a.FacilityType = r.FacilityType
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
	dbo.fn_CategoryName(o.CategoryId, o.CategoryName),	
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
		[AATF].ReturnScheme rs
		INNER JOIN [PCS].Scheme s ON s.Id = rs.SchemeId
	WHERE
		 rs.ReturnId = @ReturnId) AS SchemeNames

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
		f.[Obligation type]'

IF @SelectedReceived = 1 BEGIN
	SET @DynamicPivotQuery = CONCAT(@DynamicPivotQuery, ', f.[Total obligated WEEE received on behalf of PCS(s) (t)]')
END		

IF @HasPcsData = 1 AND @SelectedReceived = 1 BEGIN
	SET @DynamicPivotQuery = CONCAT(@DynamicPivotQuery, ', ph.*')
END

IF @SelectedSentOn = 1 BEGIN
	SET @DynamicPivotQuery = CONCAT(@DynamicPivotQuery, ', f.[Total obligated WEEE sent to another AATF / ATF for treatment (t)]')
END

IF @HasSentOnData = 1 AND @SelectedSentOn = 1 BEGIN
	SET @DynamicPivotQuery = CONCAT(@DynamicPivotQuery, ', so.*')
END

IF @SelectedReused = 1 BEGIN
	SET @DynamicPivotQuery = CONCAT(@DynamicPivotQuery, N', f.[Total obligated WEEE reused as a whole appliance (t)]')
END

SET @DynamicPivotQuery = CONCAT(@DynamicPivotQuery, '
FROM
	##FinalTable f ')

IF @HasPcsData = 1 BEGIN
	SET @DynamicPivotQuery = CONCAT(@DynamicPivotQuery, N'LEFT JOIN ##PcsObligated ph ON ph.CategoryId = f.CategoryId AND ph.AatfId = f.AatfKey AND ph.ObligationType = f.ObligationType ')
END
IF @HasSentOnData = 1 BEGIN
	SET @DynamicPivotQuery = CONCAT(@DynamicPivotQuery, N'LEFT JOIN ##SentOnObligated so ON so.CategoryId = f.CategoryId AND so.AatfId = f.AatfKey AND so.ObligationType = f.ObligationType ')
END

SET @DynamicPivotQuery = CONCAT(@DynamicPivotQuery, N'
	ORDER BY
		f.[Name of AATF]')

EXEC (@DynamicPivotQuery)

IF @HasPcsData = 1 BEGIN
	DROP TABLE ##PcsObligated
END
IF @HasSentOnData = 1 BEGIN
	DROP TABLE ##SentOnObligated
END
DROP TABLE ##FinalTable

END


GO
/****** Object:  StoredProcedure [AATF].[getUkNonObligatedWeeeReceivedByComplianceYear]    Script Date: 13/12/2021 10:56:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
	dbo.fn_CategoryName(w.Id, w.[Name]),	
	w.Id,
	q.[Quarter]
FROM
	@Quarters q, [Lookup].WeeeCategory w

INSERT INTO @Totals ([Quarter], Category, CategoryId, QuarterId)
SELECT
	@ComplianceYear,
	dbo.fn_CategoryName(w.Id, w.[Name]),
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

GO
/****** Object:  StoredProcedure [AATF].[getAllAatfSentOnCsvData]    Script Date: 16/12/2021 09:55:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--				the limits of the specified parameters.Get the latest submitted return
-- =============================================
ALTER PROCEDURE [AATF].[getAllAatfSentOnCsvData]
	@ComplianceYear INT,
	@ObligationType nvarchar(3),
	@CA UNIQUEIDENTIFIER,
	@PanArea UNIQUEIDENTIFIER
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
	ApprovalNumber			NVARCHAR(20) NOT NULL,	
	OrganisationName		NVARCHAR(256) NOT NULL,
	CompetentAuthorityAbbr	NVARCHAR(65) NOT NULL,
	PanArea					NVARCHAR(200) NULL,
	LocalArea				NVARCHAR(1024) NULL
)

IF OBJECT_ID('tempdb..#AatfSentOnData') IS NOT NULL
  DROP TABLE #AatfSentOnData

CREATE TABLE #AatfSentOnData
(
	AatfId					UNIQUEIDENTIFIER NOT NULL,
	ReturnId				UNIQUEIDENTIFIER NOT NULL,
	[Quarter]				INT NOT NULL,
	CategoryId				INT NOT NULL,
	TonnageType				NVARCHAR(20) NOT NULL,
	TotalSent				DECIMAL(35,3) NULL,
	OperatorAddressId		UNIQUEIDENTIFIER NULL,
	SiteAddressId			UNIQUEIDENTIFIER NULL,
	SiteOperatorId			INT NULL,
	SiteOperatorData		NVARCHAR(MAX) null
)

DECLARE @SiteOperator TABLE
(
	SiteOperatorId			INT NOT NULL IDENTITY(1,1),
	SiteOperatorData		NVARCHAR(MAX) NOT null
)

DECLARE @DynamicPivotQuery AS NVARCHAR(MAX)
DECLARE @ColumnNames nvarchar(MAX)

--Get the latest submitted returns for the compliance year that has ReportOnQuestions 2
INSERT INTO @SUBMITTEDRETURN
SELECT 
	X.AatfId,
	X.ReturnId,
	X.ComplianceYear,
	X.[Quarter],
	X.CreatedDate,
	X.SubmittedDate,
	X.SubmittedBy,
	X.Name,
	X.ApprovalNumber,
	X.OrgName,
	X.Abbreviation,
	X.PName,
	X.LaName
FROM
(
	SELECT
		ra.AatfId,
		ra.ReturnId,
		r.ComplianceYear,
		r.[Quarter],
		r.CreatedDate,
		r.SubmittedDate,
		CONCAT (u.FirstName, ' ', u.Surname) AS SubmittedBy,
		a.Name,
		a.ApprovalNumber,
		CASE WHEN o.Name IS NULL THEN o.TradingName	ELSE o.Name	END AS OrgName,
		ca.Abbreviation,
		pa.Name AS PName,
		la.Name AS LaName,
		ROW_NUMBER() OVER (PARTITION BY ra.AatfId, r.[Quarter] ORDER BY r.[Quarter],r.SubmittedDate DESC) AS RowNumber
FROM
	[AATF].[ReturnAatf] ra
	INNER JOIN [AATF].[Return] r ON r.Id = ra.ReturnId
	INNER JOIN AATF.AATF a ON a.Id = ra.AatfId  AND a.FacilityType = r.FacilityType
	INNER JOIN Organisation.Organisation o ON a.OrganisationId = o.Id
	INNER JOIN [Lookup].CompetentAuthority ca ON a.CompetentAuthorityId = ca.Id
	INNER JOIN [Identity].[AspNetUsers] u ON u.id = r.SubmittedById
	LEFT JOIN [Lookup].[LocalArea] la ON la.Id = a.LocalAreaId
	LEFT JOIN [Lookup].[PanArea] pa ON pa.Id = a.PanAreaId
WHERE
	r.ComplianceYear = @ComplianceYear
	AND r.ReturnStatus = 2  -- submitted
	AND a.FacilityType = 1  --aatf
	AND a.CompetentAuthorityId = COALESCE(@CA, a.CompetentAuthorityId)
	AND (
		@PanArea IS NULL
		OR a.PanAreaId = COALESCE(@PanArea, a.PanAreaId)
		)
) X
	INNER JOIN [AATF].[ReturnReportOn] ro on ro.ReturnId = X.ReturnId AND ro.ReportOnQuestionId = 2
WHERE
	X.RowNumber = 1

--Total Sent to another AATF / ATF (t)
INSERT INTO #AatfSentOnData (AatfId, ReturnId, [Quarter], CategoryId, TonnageType, TotalSent, OperatorAddressId, SiteAddressId)
SELECT 
	u.AatfId,
	u.ReturnId,
	u.[Quarter],
	u.CategoryId,
	u.Tonnage,
	SUM(u.value) AS VALUE,
	OperatorAddressId,
	SiteAddressId
FROM (
		SELECT
			r.AatfId,
			r.ReturnId,
			r.[Quarter],
			wsoa.CategoryId,
			COALESCE(wsoa.HouseholdTonnage, 0) HouseholdTonnage,
			COALESCE(wsoa.NonHouseholdTonnage, 0) NonHouseholdTonnage,
			wso.OperatorAddressId,
			wso.SiteAddressId
		FROM
			@SUBMITTEDRETURN r
			INNER JOIN [AATF].WeeeSentOn wso ON r.ReturnId = wso.ReturnId AND wso.AatfId = r.AatfId
			INNER JOIN [AATF].WeeeSentOnAmount wsoa ON wso.Id = wsoa.WeeeSentOnId
	) a
	UNPIVOT(value FOR Tonnage IN (a.HouseholdTonnage, a.NonHouseholdTonnage)) u
	GROUP BY AatfId, ReturnId, [Quarter], Tonnage, CategoryId, OperatorAddressId, SiteAddressId
	ORDER BY AatfId, ReturnId, [Quarter], Tonnage, CategoryId, OperatorAddressId, SiteAddressId

-------------End of Total Sent to Obligated data by AATF-----------------------------

--Address concatenation

UPDATE
	#AatfSentOnData
SET 
	SiteOperatorData = x.SiteOperator
FROM #AatfSentOnData a
INNER JOIN (
	SELECT 
		o.AatfId,
		o.ReturnId,
		o.[Quarter],
		o.CategoryId,
		o.TonnageType, 
		CONCAT(sa.Name, ', ', sa.Address1, COALESCE(', ' + NULLIF(sa.Address2, ''), ''), ', ', sa.TownOrCity, COALESCE(', ' + NULLIF(sa.CountyOrRegion, ''), ''),
		COALESCE(', ' + NULLIF(sa.Postcode, ''), ''), ', ', sc.Name, ', ', pa.Name, ', ', pa.Address1, COALESCE(', ' + NULLIF(pa.Address2, ''), ''), ', ', pa.TownOrCity,  
		COALESCE(', ' + NULLIF(pa.CountyOrRegion, ''), ''), COALESCE(', ' + NULLIF(pa.Postcode, ''), ''), ', ', oc.Name) AS SiteOperator
	FROM 
		#AatfSentOnData o
		LEFT JOIN AATF.[Address] pa ON pa.Id = o.OperatorAddressId
		LEFT JOIN AATF.[Address] sa ON sa.Id = o.SiteAddressId
		LEFT JOIN [Lookup].[Country] oc ON oc.Id = pa.CountryId
		LEFT JOIN [Lookup].[Country] sc ON sc.Id = pa.CountryId
	) X ON X.AatfId = a.AatfId
			AND X.ReturnId = A.ReturnId
			AND X.[Quarter] = a.[Quarter]
			AND X.CategoryId = a.CategoryId
			AND X.TonnageType = a.TonnageType

INSERT INTO @SiteOperator(SiteOperatorData)
SELECT DISTINCT SiteOperatorData from #AatfSentOnData

UPDATE #AatfSentOnData
SET 
	SiteOperatorId = s.SiteOperatorId
FROM
	#AatfSentOnData a
	LEFT JOIN @SiteOperator s ON s.SiteOperatorData = a.SiteOperatorData

-----------------------------------------------------
--INSERT FOR NIL AND NO DATA SUBMITTED RETURN
IF EXISTS (
		SELECT *
		FROM @SUBMITTEDRETURN
		)
BEGIN
	INSERT INTO #AatfSentOnData (AatfId, ReturnId, [Quarter], CategoryId, TonnageType, SiteOperatorId)
	SELECT 
		s.AatfId,
		s.ReturnId,
		s.[Quarter],
		c.Id,
		'HouseholdTonnage',
		0
	FROM 
		[Lookup].WeeeCategory c
		LEFT JOIN @SUBMITTEDRETURN s ON 1 = 1
	EXCEPT
		SELECT DISTINCT AatfId, ReturnId, [Quarter], CategoryId, TonnageType, SiteOperatorId FROM #AatfSentOnData
	
	UNION ALL
	
	SELECT
		s.AatfId,
		s.ReturnId,
		s.[Quarter],
		c.Id,
		'NonHouseholdTonnage',
		0
	FROM 
		[Lookup].WeeeCategory c
		LEFT JOIN @SUBMITTEDRETURN s ON 1 = 1
	EXCEPT
		SELECT DISTINCT AatfId, ReturnId, [Quarter], CategoryId, TonnageType, SiteOperatorId FROM #AatfSentOnData

END

-----------------------------------------------------------------------------------
DECLARE @COUNT INT

SELECT @COUNT = COUNT(*) FROM @SiteOperator

IF @COUNT > 0
BEGIN

	SELECT @ColumnNames = 
	ISNULL(@ColumnNames + ',', '') + QUOTENAME(SiteOperatorId)
	FROM (
		SELECT DISTINCT SiteOperatorId
		FROM #AatfSentOnData
		) A

	SET @DynamicPivotQuery = N'
	IF OBJECT_ID(''tempdb..##ResultsSentOn'') IS NOT NULL
	  DROP TABLE ##ResultsSentOn

	SELECT * into ##ResultsSentOn
	FROM
		(
		SELECT AatfId,ReturnId,[Quarter] AS ''Q'',CategoryId,TonnageType, ' + @ColumnNames + '
		FROM (SELECT AatfId,ReturnId,[Quarter],CategoryId,TonnageType,TotalSent,SiteOperatorId FROM #AatfSentOnData) as t
		PIVOT (MAX(TotalSent) FOR SiteOperatorId in (' + @ColumnNames + ')) AS PVTTable
		)a'

	EXEC sp_executesql @DynamicPivotQuery; 

	IF OBJECT_ID('tempdb..#tempSentOn') IS NOT NULL
	  DROP TABLE #tempSentOn

	SELECT * INTO #tempSentOn FROM ##ResultsSentOn;

	DROP TABLE ##ResultsSentOn

	SELECT 
		a.CompetentAuthorityAbbr AS 'Appropriate authority',
		a.PanArea AS 'WROS Pan Area Team',
		a.LocalArea AS 'EA Area',
		@ComplianceYear AS 'Compliance year',
		CONCAT('Q', a.[Quarter]) AS 'Quarter',
		a.SubmittedBy AS 'Submitted by',
		a.SubmittedDate AS 'Date submitted (GMT)',
		a.OrganisationName AS 'Organisation name',
		a.Name AS 'Name of AATF',
		a.ApprovalNumber AS 'Approval number',
		dbo.fn_CategoryName(c.Id, c.Name) AS Category,
		a.Obligation AS 'Obligation type' ,
		a.TotalSent AS 'Total sent to another AATF / ATF (t)',
		x.*
	FROM
		(
		SELECT
			o.AatfId,
			r.CompetentAuthorityAbbr,
			r.PanArea,
			r.[Quarter],
			r.SubmittedBy,
			r.SubmittedDate,
			r.LocalArea,
			r.OrganisationName,
			r.Name,
			r.ApprovalNumber,
			o.CategoryId,
			o.returnId,
			o.TonnageType, 
			CASE WHEN o.TonnageType = 'HouseholdTonnage' THEN 'B2C'	ELSE 'B2B' END AS Obligation,
			SUM(o.TotalSent) AS TotalSent
		FROM
			#AatfSentOnData o
			INNER JOIN @SUBMITTEDRETURN r ON r.AatfId = o.AatfId
				AND R.[Quarter] = o.[Quarter]
				AND R.ReturnId = o.ReturnId
		GROUP BY
			o.AatfId,
			r.CompetentAuthorityAbbr,
			r.PanArea,
			r.[Quarter],
			r.SubmittedBy,
			r.SubmittedDate,
			r.LocalArea, 
			r.OrganisationName,
			r.Name,
			r.ApprovalNumber,
			o.CategoryId,
			o.returnId,
			o.TonnageType
			) a
		INNER JOIN [Lookup].WeeeCategory c ON a.CategoryId = c.Id
		LEFT JOIN #tempSentOn x ON x.AatfId = a.AatfId
			AND x.ReturnId = a.ReturnId
			AND x.Q = a.[Quarter]
			AND x.CategoryId = a.CategoryId
			AND x.TonnageType = a.TonnageType
	WHERE (
			@ObligationType IS NULL
			OR a.Obligation LIKE '%' + COALESCE(@ObligationType, a.Obligation) + '%'
			)
	ORDER BY 
		a.CompetentAuthorityAbbr,
		a.[Quarter],
		a.Name,
		a.SubmittedDate,
		a.TonnageType,
		a.CategoryId

	DROP Table #tempSentOn
END
ELSE

BEGIN
	SELECT 
		a.CompetentAuthorityAbbr AS 'Appropriate authority',
		a.PanArea AS 'WROS Pan Area Team',
		a.LocalArea AS 'EA Area',
		@ComplianceYear AS 'Compliance year',
		CONCAT ('Q', a.[Quarter]) AS 'Quarter',
		a.SubmittedBy AS 'Submitted by', 
		a.SubmittedDate AS 'Date submitted (GMT)',
		a.OrganisationName AS 'Organisation name',
		a.Name AS 'Name of AATF', 
		a.ApprovalNumber AS 'Approval number',
		dbo.fn_CategoryName(c.Id, c.Name) AS Category,
		a.Obligation AS 'Obligation type',
		a.TotalSent AS 'Total sent to another AATF / ATF (t)'
	FROM (
			SELECT DISTINCT 
				o.AatfId,
				r.CompetentAuthorityAbbr,
				r.PanArea,
				r.[Quarter],
				r.SubmittedBy,
				r.SubmittedDate,
				r.LocalArea, 
				r.OrganisationName,
				r.Name,
				r.ApprovalNumber,
				o.CategoryId,
				o.returnId,
				o.TonnageType, 
				CASE WHEN o.TonnageType = 'HouseholdTonnage' THEN 'B2C'	ELSE 'B2B' END AS Obligation,
				SUM(o.TotalSent) AS TotalSent
		FROM 
			#AatfSentOnData o
			INNER JOIN @SUBMITTEDRETURN r ON r.AatfId = o.AatfId
				AND R.[Quarter] = o.[Quarter]
				AND R.ReturnId = o.ReturnId
		GROUP BY
			o.AatfId,
			r.CompetentAuthorityAbbr,
			r.PanArea,
			r.[Quarter],
			r.SubmittedBy,
			r.SubmittedDate,
			r.LocalArea, 
			r.OrganisationName,
			r.Name,
			r.ApprovalNumber,
			o.CategoryId,
			o.returnId,
			o.TonnageType
		) a
		INNER JOIN [Lookup].WeeeCategory c ON a.CategoryId = c.Id
		WHERE (
			@ObligationType IS NULL
			OR a.Obligation LIKE '%' + COALESCE(@ObligationType, a.Obligation) + '%'
			)
		ORDER BY
			a.CompetentAuthorityAbbr,
			a.[Quarter],
			a.Name,
			a.SubmittedDate,
			a.TonnageType,
			a.CategoryId
END

SELECT * FROM @SiteOperator

END


GO
/****** Object:  StoredProcedure [AATF].[getNonObligatedWeeeReceived]    Script Date: 16/12/2021 10:08:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [AATF].[getNonObligatedWeeeReceived]
	@ComplianceYear INT,
	@OrganisationName VARCHAR(256) = NULL
AS
BEGIN
	SET NOCOUNT ON;

DECLARE @FinalReturns TABLE
(
	ReturnId UNIQUEIDENTIFIER,
	[Quarter] INT,
	[Year] INT,
	[OrganisationId] UNIQUEIDENTIFIER,
	CategoryId INT,
	CategoryName NVARCHAR(60)
);

WITH FinalReturns (Id, [Quarter], [Year], OrganisationId)
AS
(
	SELECT
		r.Id,
		r.[Quarter],
		r.ComplianceYear,
		r.OrganisationId
	FROM
		[AATF].[Return] r
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
		INNER JOIN [Organisation].Organisation o ON o.Id = r.OrganisationId
WHERE
	@OrganisationName IS NULL OR ((o.[TradingName] LIKE '%' + @OrganisationName + '%' AND o.OrganisationType = 2) OR (o.[Name] LIKE '%' + @OrganisationName + '%' AND o.OrganisationType != 2)))

INSERT INTO @FinalReturns
SELECT
	r.Id,
	r.[Quarter],
	r.[Year],
	r.OrganisationId,
	w.Id,
	w.[Name]
FROM
	FinalReturns r, [Lookup].WeeeCategory w

DECLARE @NonObligatedResults TABLE
(
	CategoryId INT,
	TotalNonObligatedWeeeReceived DECIMAL(23,3),
	TotalNonObligatedWeeeReceivedFromDcf DECIMAL(23,3),
	ReturnId UNIQUEIDENTIFIER
)

INSERT INTO @NonObligatedResults (CategoryId, ReturnId)
SELECT 
	w.Id,
	non.ReturnId
 FROM
	[Lookup].WeeeCategory w, [AATF].[NonObligatedWeee] non 
WHERE
	w.Id = non.CategoryId
 GROUP BY
	w.Id,
	non.ReturnId

;WITH NonObligatedUpdateDcf (Id, Dcf, ReturnId)
AS (
SELECT 
	w.Id,
	CONVERT(DECIMAL(28,3), CASE WHEN non.Dcf = 1 THEN non.Tonnage ELSE NULL END), 
	non.ReturnId
 FROM
	[Lookup].WeeeCategory w, [AATF].[NonObligatedWeee] non 
WHERE
	w.Id = non.CategoryId
	AND non.Dcf = 1
 GROUP BY
	w.Id,
	non.ReturnId,
	CONVERT(DECIMAL(28,3), CASE WHEN non.Dcf = 1 THEN non.Tonnage ELSE NULL END)
)
UPDATE
	@NonObligatedResults
SET
	TotalNonObligatedWeeeReceivedFromDcf = Dcf
FROM
	@NonObligatedResults n
	INNER JOIN NonObligatedUpdateDcf nu ON nu.ReturnId = n.ReturnId AND nu.Id = n.CategoryId

;WITH NonObligatedUpdate (Id, Tonnage, ReturnId)
AS (
SELECT 
	w.Id,
	CONVERT(DECIMAL(28,3), CASE WHEN non.Dcf = 0 THEN non.Tonnage ELSE NULL END), 
	non.ReturnId
 FROM
	[Lookup].WeeeCategory w, [AATF].[NonObligatedWeee] non 
WHERE
	w.Id = non.CategoryId
	AND non.Dcf = 0
 GROUP BY
	w.Id,
	non.ReturnId,
	CONVERT(DECIMAL(28,3), CASE WHEN non.Dcf = 0 THEN non.Tonnage ELSE NULL END)
)
UPDATE
	@NonObligatedResults
SET
	TotalNonObligatedWeeeReceived = Tonnage
FROM
	@NonObligatedResults n
	INNER JOIN NonObligatedUpdate nu ON nu.ReturnId = n.ReturnId AND nu.Id = n.CategoryId

SELECT
	r.ComplianceYear AS [Year],
	CONCAT('Q', r.[Quarter]) AS [Quarter],
	r.SubmittedDate,
	CONCAT(u.FirstName , ' ', u.Surname) AS SubmittedBy,
	CASE WHEN o.Name IS NULL THEN o.TradingName ELSE o.Name END AS OrganisationName,
	dbo.fn_CategoryName(fr.CategoryId, fr.CategoryName) AS Category,	
	nr.TotalNonObligatedWeeeReceived,
	nr.TotalNonObligatedWeeeReceivedFromDcf,
	fr.ReturnId,
	fr.CategoryId
FROM
	@FinalReturns fr
	LEFT JOIN @NonObligatedResults nr ON nr.ReturnId = fr.ReturnId AND fr.CategoryId = nr.CategoryId
	INNER JOIN [AATF].[Return] r ON fr.ReturnId = r.Id
	INNER JOIN [Organisation].Organisation o ON o.Id = r.OrganisationId
	INNER JOIN [Identity].AspNetUsers u ON r.SubmittedById = u.Id
ORDER BY
	r.ComplianceYear, r.[Quarter], OrganisationName, fr.CategoryId
END