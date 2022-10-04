GO

/****** Object:  StoredProcedure [AATF].[getAllAatfSentOnCsvData]    Script Date: 04/10/2022 09:52:12 ******/
DROP PROCEDURE [AATF].[getAllAatfSentOnCsvData]
GO

/****** Object:  StoredProcedure [AATF].[getAllAatfSentOnCsvData]    Script Date: 04/10/2022 09:52:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
CREATE PROCEDURE [AATF].[getAllAatfSentOnCsvData]
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
		) A order by SiteOperatorId

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
		a.CategoryId,
		x.AatfId

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
