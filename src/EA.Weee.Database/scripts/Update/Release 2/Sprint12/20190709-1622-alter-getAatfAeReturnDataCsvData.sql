-- Description:	This stored procedure is used to provide the data for the admin report of aatf/ae
--				that have/haven't submitted a data return within
--				the limits of the specified parameters. The first submitted return should be returned.
-- =============================================
alter PROCEDURE [AATF].[getAatfAeReturnDataCsvData]
	@ComplianceYear INT,
	@Quarter INT,
	@FacilityType INT,
	@ReturnStatus INT,
	@CA UNIQUEIDENTIFIER,
	@Area UNIQUEIDENTIFIER,
	@PanArea UNIQUEIDENTIFIER
AS
BEGIN


SET NOCOUNT ON;

DECLARE @AATF TABLE
(
	AatfId					UNIQUEIDENTIFIER NOT NULL,
	Name					NVARCHAR(256) NOT NULL,
	ApprovalNumber			NVARCHAR(20) NOT NULL,	
	OrganisationName		NVARCHAR(256) NULL,
	CompetentAuthorityAbbr	NVARCHAR(65) NOT NULL,
	CompetentAuthorityId	UNIQUEIDENTIFIER NOT NULL
)

DECLARE @RETURN TABLE
(
	AatfId					UNIQUEIDENTIFIER NOT NULL,
	ReturnId				UNIQUEIDENTIFIER NOT NULL,
	ReturnStatus			INT NOT NULL,
	CreatedDate				DATETIME NOT NULL,
	SubmittedDate			DATETIME NULL,
	SubmittedById			UNIQUEIDENTIFIER NULL
)

--SET THE START AND END DATE

DECLARE @QuarterStartDate date
DECLARE @QuarterEndDate date
DECLARE @StartYear int
DECLARE @EndYear int

	SELECT @StartYear = @ComplianceYear + AddStartYears, @EndYear = @ComplianceYear + AddEndYears FROM [Lookup].[QuarterWindowTemplate] WHERE [Quarter] = @Quarter

	SELECT @QuarterStartDate = DATEFROMPARTS(@StartYear,StartMonth,StartDay), 
	   @QuarterEndDate = DATEFROMPARTS(@EndYear,EndMonth,EndDay)
	   FROM [Lookup].[QuarterWindowTemplate] WHERE [Quarter] = @Quarter

	INSERT INTO @AATF
		SELECT a.id, a.Name, a.ApprovalNumber,
		CASE WHEN o.Name IS NULL THEN o.TradingName
			ELSE o.Name END, ca.Abbreviation, ca.Id
		FROM AATF.AATF a
			JOIN Organisation.Organisation o
		ON a.OrganisationId  = o.Id
			JOIN Lookup.CompetentAuthority ca
		ON a.CompetentAuthorityId = ca.Id
		WHERE A.ComplianceYear = @ComplianceYear 
			AND A.FacilityType = @FacilityType
			AND ApprovalDate BETWEEN @QuarterStartDate AND @QuarterEndDate 
			AND a.CompetentAuthorityId = COALESCE(@CA, a.CompetentAuthorityId)
			AND (@Area IS NULL OR a.LocalAreaId = COALESCE(@Area, a.LocalAreaId))
			AND (@PanArea IS NULL OR a.PanAreaId = COALESCE(@PanArea, a.PanAreaId))


 --Get the returns for the AATF/AE
 
 INSERT INTO @RETURN
 SELECT X.AatfId,
 		X.Id, 
		X.[ReturnStatus], X.[CreatedDate], X.[SubmittedDate], X.[SubmittedById] FROM
		(SELECT 
			a.AatfId,
 			r.Id, 
			r.[ReturnStatus],r.[CreatedDate],r.[SubmittedDate],r.[SubmittedById],
			ROW_NUMBER() OVER
					(
						PARTITION BY a.AatfId
						ORDER BY r.CreatedDate asc
					) AS RowNumber
			FROM 
 			[AATF].WeeeSentOn wso 
			INNER JOIN @AATF A ON wso.AatfId = A.AatfId 
			INNER JOIN [AATF].[Return] r ON r.Id = wso.ReturnId 
			WHERE r.ComplianceYear = @ComplianceYear
			AND R.[Quarter] = @Quarter
			AND r.FacilityType = @FacilityType
		)X
		WHERE X.RowNumber = 1
		AND (@ReturnStatus IS NULL OR X.ReturnStatus = COALESCE(@ReturnStatus, X.ReturnStatus))
	
INSERT INTO @RETURN
 SELECT X.AatfId,
 		X.Id, 
		X.[ReturnStatus], X.[CreatedDate], X.[SubmittedDate], X.[SubmittedById] FROM
		(SELECT 
			a.AatfId,
 			r.Id, 
			r.[ReturnStatus],r.[CreatedDate],r.[SubmittedDate],r.[SubmittedById],
			ROW_NUMBER() OVER
					(
						PARTITION BY a.AatfId
						ORDER BY r.CreatedDate asc
					) AS RowNumber
			FROM 
 			[AATF].WeeeReceived wr 
			INNER JOIN @AATF A ON wr.AatfId = A.AatfId 
			INNER JOIN [AATF].[Return] r ON r.Id = wr.ReturnId 
			WHERE r.ComplianceYear = @ComplianceYear
			AND R.[Quarter] = @Quarter
			AND r.FacilityType = @FacilityType
		)X
		WHERE X.RowNumber = 1
		AND (@ReturnStatus IS NULL OR X.ReturnStatus = COALESCE(@ReturnStatus, X.ReturnStatus))


INSERT INTO @RETURN
 SELECT X.AatfId,
 		X.Id, 
		X.[ReturnStatus], X.[CreatedDate], X.[SubmittedDate], X.[SubmittedById] FROM
		(SELECT 
			a.AatfId,
 			r.Id, 
			r.[ReturnStatus],r.[CreatedDate],r.[SubmittedDate],r.[SubmittedById],
			ROW_NUMBER() OVER
					(
						PARTITION BY a.AatfId
						ORDER BY r.CreatedDate asc
					) AS RowNumber
			FROM 
 			[AATF].WeeeReused wr 
			INNER JOIN @AATF A ON wr.AatfId = A.AatfId 
			INNER JOIN [AATF].[Return] r ON r.Id = wr.ReturnId 
			WHERE r.ComplianceYear = @ComplianceYear
			AND R.[Quarter] = @Quarter
			AND r.FacilityType = @FacilityType
		)X
		WHERE X.RowNumber = 1
		AND (@ReturnStatus IS NULL OR X.ReturnStatus = COALESCE(@ReturnStatus, X.ReturnStatus))


--Due to aatf/ae approval date updates check if returns are missed 

INSERT INTO @AATF
		SELECT a.id, a.Name, a.ApprovalNumber,
		CASE WHEN o.Name IS NULL THEN o.TradingName
			ELSE o.Name END, ca.Abbreviation, ca.Id
		FROM 
			(SELECT ra.AatfId FROM [AATF].[Return] r 
				JOIN [AATF].[ReturnAatf] ra ON 
				ra.[ReturnId] = r.Id
						WHERE r.ComplianceYear = @ComplianceYear
						AND R.[Quarter] = @Quarter
				AND r.id NOT IN (select ReturnId from @RETURN) 
			) X
			JOIN AATF.AATF a 
			ON a.id= X.AatfId
				JOIN Organisation.Organisation o
			ON a.OrganisationId  = o.Id
				JOIN Lookup.CompetentAuthority ca
			ON a.CompetentAuthorityId = ca.Id
				WHERE A.FacilityType = @FacilityType
				AND a.CompetentAuthorityId = COALESCE(@CA, a.CompetentAuthorityId)
				AND (@Area IS NULL OR a.LocalAreaId = COALESCE(@Area, a.LocalAreaId))
				AND (@PanArea IS NULL OR a.PanAreaId = COALESCE(@PanArea, a.PanAreaId))
			AND a.id NOT IN (SELECT AatfId FROM @AATF)

INSERT INTO @RETURN	
	SELECT X.AatfId,
 		X.Id, 
		X.[ReturnStatus], X.[CreatedDate], X.[SubmittedDate], X.[SubmittedById] FROM
		(SELECT ra.AatfId, r.Id,r.[ReturnStatus], r.[CreatedDate], r.[SubmittedDate], r.[SubmittedById],
		ROW_NUMBER() OVER
					(
						PARTITION BY ra.AatfId
						ORDER BY r.CreatedDate asc
					) AS RowNumber
		
		 FROM [AATF].[Return] r 
		JOIN [AATF].[ReturnAatf] ra ON 
		ra.[ReturnId] = r.Id
				WHERE r.ComplianceYear = @ComplianceYear
				AND R.[Quarter] = @Quarter
		AND r.id NOT IN (select ReturnId from @RETURN) 
		) X
		JOIN AATF.AATF a
		ON a.id= X.AatfId
		WHERE X.RowNumber = 1
		AND A.FacilityType = @FacilityType
		AND a.CompetentAuthorityId = COALESCE(@CA, a.CompetentAuthorityId)
		AND (@Area IS NULL OR a.LocalAreaId = COALESCE(@Area, a.LocalAreaId))
		AND (@PanArea IS NULL OR a.PanAreaId = COALESCE(@PanArea, a.PanAreaId))

	SELECT DISTINCT 
		a.AatfId,			
		a.Name,
		a.ApprovalNumber,		
		a.OrganisationName,
		CASE WHEN r.ReturnStatus = 1 then 'Started'
		WHEN r.ReturnStatus = 2 then 'Submitted'
		ELSE 'Not Started'
		END AS ReturnStatus,
		r.CreatedDate,
		r.SubmittedDate,
		CONCAT(u.FirstName,' ',u.Surname) as 'SubmittedBy',	
		a.CompetentAuthorityAbbr
	 FROM @AATF A
	 LEFT JOIN @RETURN r
		ON r.AatfId = A.AatfId
	 LEFT JOIN  [Identity].[AspNetUsers] u
		ON u.id = r.SubmittedById
	 ORDER BY A.Name

END
GO


