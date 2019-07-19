-- Description:	This stored procedure is used to provide the data for the admin report of obligatde data
--				that have/haven't submitted a data return within
--				the limits of the specified parameters.Get the latest submitted return

-- =============================================
CREATE PROCEDURE [AATF].[getAllAatfObligatedCsvData]
	@ComplianceYear INT,
	@AatfName nvarchar(256),
	@ObligationType nvarchar(3),
	@CA UNIQUEIDENTIFIER,
	@PanArea UNIQUEIDENTIFIER,
	@ColumnType INT
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
INSERT INTO @SUBMITTEDRETURN
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
SELECT *
FROM @TotalReused
WHERE AatfId NOT IN (SELECT DISTINCT AatfId FROM @ObligatedData)


--Total received from PCS (t)
INSERT INTO #TotalReceivedByScheme
SELECT u.AatfId, u.ReturnId, u.[Quarter], u.CategoryId, u.SchemeId, u.Tonnage, SUM(u.value) AS VALUE, u.SchemeName, u.ApprovalNumber
FROM (
	SELECT wr.SchemeId, r.AatfId, r.ReturnId, r.[Quarter], wra.CategoryId,
	 COALESCE(wra.HouseholdTonnage, 0) HouseholdTonnage,
	 COALESCE(wra.NonHouseholdTonnage, 0) NonHouseholdTonnage, s.SchemeName, s.ApprovalNumber
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
	FROM #TotalReceivedByScheme
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
	FROM #TotalReceivedByScheme
	WHERE AatfId NOT IN (SELECT DISTINCT AatfId FROM @ObligatedData)
	GROUP BY AatfId, ReturnId, [Quarter], TonnageType, CategoryId
	) y

-------------End of Total Obligated data by AATF-----------------------------


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
,a.PanArea AS 'WROS pan area team'
,a.LocalArea AS 'EA Area'
,@ComplianceYear AS 'Year'
,a.[Quarter] AS 'Quarter'
,a.SubmittedBy AS 'Submitted by'
,a.SubmittedDate AS 'Date submitted (GMT)'
,a.OrganisationName AS 'Organisation name'
,a.Name AS 'Name of AATF'
,a.ApprovalNumber AS 'Approval number'
,CONCAT(c.Id, '.', c.Name) AS Category
,a.Obligation
,a.TotalSent AS 'Total sent to another AATF / ATF (t)'
,a.TotalReused AS 'Reused as a whole appliance (t)'
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
	FROM @ObligatedData o		
	LEFT JOIN @SUBMITTEDRETURN r ON r.AatfId = o.AatfId
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
ORDER BY a.CompetentAuthorityAbbr, a.[Quarter], a.SubmittedDate, a.TonnageType, a.CategoryId


DROP Table #temp
END
ELSE
	BEGIN

		SELECT 
		 a.CompetentAuthorityAbbr AS 'Appropriate authority'
		,a.PanArea AS 'WROS pan area team'
		,a.LocalArea AS 'EA Area'
		,@ComplianceYear AS 'Year'
		,a.[Quarter] AS 'Quarter'
		,a.SubmittedBy AS 'Submitted by'
		,a.SubmittedDate AS 'Date submitted (GMT)'
		,a.OrganisationName AS 'Organisation name'
		,a.Name AS 'Name of AATF'
		,a.ApprovalNumber AS 'Approval number'
		,CONCAT(c.Id, '.', c.Name) AS Category
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
			FROM @ObligatedData o		
			LEFT JOIN @SUBMITTEDRETURN r ON r.AatfId = o.AatfId
				AND R.[Quarter] = o.[Quarter]
				AND R.ReturnId = o.ReturnId
			) a
			INNER JOIN [Lookup].WeeeCategory c ON a.CategoryId = c.Id
		WHERE (@ObligationType IS NULL OR a.Obligation like '%' + COALESCE(@ObligationType, a.Obligation) + '%')
		ORDER BY a.CompetentAuthorityAbbr, a.[Quarter], a.SubmittedDate, a.TonnageType, a.CategoryId
	END

DROP Table #TotalReceivedByScheme


END


GO


