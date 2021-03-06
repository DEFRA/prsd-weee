ALTER PROCEDURE [AATF].[getAatfAeDetailsCsvData]
	@ComplianceYear INT,
	@FacilityType INT,
	@CA UNIQUEIDENTIFIER,
	@Area UNIQUEIDENTIFIER,
	@PanArea UNIQUEIDENTIFIER
AS
BEGIN

SET NOCOUNT ON;

DECLARE @AATF TABLE
(
	ComplianceYear			INT NOT NULL,
	AppropriateAuthorityAbbr	NVARCHAR(65) NOT NULL,
	PanAreaTeam				NVARCHAR(256) NULL,
	EaArea					NVARCHAR(256) NULL,
	Name					NVARCHAR(256) NOT NULL,
	Address1				NVARCHAR(60) NOT NULL,
	Address2				NVARCHAR(60) NULL,
	TownCity				NVARCHAR(35) NOT NULL,
	CountyRegion			NVARCHAR(35) NULL,
	Country					NVARCHAR(2048) NOT NULL,
	PostCode				NVARCHAR(10) NULL,
	ApprovalNumber			NVARCHAR(20) NOT NULL,
	ApprovalDate			Datetime2(7) NOT NULL,
	Size					NVARCHAR(20) NOT NULL,
	Status					NVARCHAR(20) NOT NULL,
	ContactName				NVARCHAR(70) NOT NULL,
	ContactPosition			NVARCHAR(35) NOT NULL,
	ContactAddress1			NVARCHAR(60) NOT NULL,
	ContactAddress2			NVARCHAR(60) NULL,
	ContactTownCity			NVARCHAR(35) NOT NULL,
	ContactCountyRegion		NVARCHAR(35) NULL,
	ContactCountry			NVARCHAR(2048) NOT NULL,
	ContactPostCode			NVARCHAR(10) NULL,
	ContactEmail			NVARCHAR(256) NOT NULL,
	ContactPhone			NVARCHAR(20) NOT NULL,
	OrganisationName		NVARCHAR(256) NOT NULL,
	OrganisationAddress1	NVARCHAR(60) NOT NULL,
	OrganisationAddress2	NVARCHAR(60) NULL,
	OrganisationTownCity	NVARCHAR(35) NOT NULL,
	OrganisationCountyRegion NVARCHAR(35) NULL,
	OrganisationCountry		NVARCHAR(2048) NOT NULL,
	OrganisationPostcode	NVARCHAR(10) NULL,
	AatfAddress				NVARCHAR(500) NULL,
	OperatorName			NVARCHAR(256) NULL,
	OperatorTradingName		NVARCHAR(256) NULL,
	OperatorAddress			NVARCHAR(500) NULL		
)

INSERT INTO @AATF
	SELECT 
		@ComplianceYear,
		ca.Abbreviation,
		pa.Name,
		la.Name,
		a.Name,
		ad.Address1,
		ad.Address2,
		ad.TownOrCity,
		ad.CountyOrRegion,
		adco.Name,
		ad.Postcode,
		a.ApprovalNumber,
		a.ApprovalDate,
		CASE WHEN a.Size = 1 THEN 'Small' ELSE 'Large' END,
		CASE WHEN a.Status = 1 THEN 'Approved' WHEN a.Status = 2 THEN 'Suspended'  ELSE 'Cancelled' END,
		c.FirstName + ' ' + c.LastName,
		c.Position,
		c.Address1,
		c.Address2,
		c.TownOrCity,
		c.CountyOrRegion,
		conco.Name,
		c.Postcode,
		c.Email, 
		c.Telephone,
		CASE WHEN o.Name IS NULL THEN o.TradingName ELSE o.Name END,
		oa.Address1,
		oa.Address2,
		oa.TownOrCity,
		oa.CountyOrRegion,
		orgco.Name,
		oa.Postcode,
		CONCAT(ad.Address1, COALESCE(', ' + NULLIF(ad.Address2, ''), ''), ', ', ad.TownOrCity,COALESCE(', ' + NULLIF(ad.CountyOrRegion, ''), '')),
		o.Name,
		o.TradingName,
		CONCAT(oa.Address1, COALESCE(', ' + NULLIF(oa.Address2, ''), ''), ', ', oa.TownOrCity,COALESCE(', ' + NULLIF(oa.CountyOrRegion, ''), ''))
	FROM
		AATF.AATF a 
		JOIN Organisation.Organisation o ON a.OrganisationId  = o.Id
		LEFT JOIN Organisation.Address oa ON o.BusinessAddressId = oa.Id
		JOIN Lookup.Country orgco on oa.CountryId = orgco.Id
		JOIN Lookup.CompetentAuthority ca ON a.CompetentAuthorityId = ca.Id
		LEFT JOIN Lookup.PanArea pa on a.PanAreaId = pa.Id
		LEFT JOIN Lookup.LocalArea la on a.LocalAreaId = la.Id
		LEFT JOIN AATF.Address ad on a.SiteAddressId = ad.Id
		JOIN Lookup.Country adco on ad.CountryId = adco.Id
		JOIN AATF.Contact c on a.ContactId = c.Id
		JOIN Lookup.Country conco on c.CountryId = conco.Id
	WHERE 
		A.ComplianceYear = @ComplianceYear 
		AND A.FacilityType = @FacilityType
		AND a.CompetentAuthorityId = COALESCE(@CA, a.CompetentAuthorityId)
		AND (@Area IS NULL OR a.LocalAreaId = COALESCE(@Area, a.LocalAreaId))
		AND (@PanArea IS NULL OR a.PanAreaId = COALESCE(@PanArea, a.PanAreaId))


SELECT * FROM @AATF Order by Name

END
GO


