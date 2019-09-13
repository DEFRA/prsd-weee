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
	ComplianceYear			NVARCHAR(10)  NULL,
	AppropriateAuthorityAbbr	NVARCHAR(65) NOT NULL,
	PanAreaTeam				NVARCHAR(256) NULL,
	EaArea					NVARCHAR(256) NULL,
	Name					NVARCHAR(256) NOT NULL,
	Address1				NVARCHAR(60)  NULL,
	Address2				NVARCHAR(60)  NULL,
	TownCity				NVARCHAR(35)  NULL,
	CountyRegion			NVARCHAR(35)  NULL,
	Country					NVARCHAR(2048) NULL,
	PostCode				NVARCHAR(10) NULL,
	ApprovalNumber			NVARCHAR(20)  NULL,
	ApprovalDate			Datetime2(7)  NULL,
	Size					NVARCHAR(20) NULL,
	Status					NVARCHAR(20) NOT NULL,
	FirstName				NVARCHAR(35)  NULL,
	LastName				NVARCHAR(35)  NULL,
	ContactPosition			NVARCHAR(35)  NULL,
	ContactAddress1			NVARCHAR(60)  NULL,
	ContactAddress2			NVARCHAR(60) NULL,
	ContactTownCity			NVARCHAR(35)  NULL,
	ContactCountyRegion		NVARCHAR(35) NULL,
	ContactCountry			NVARCHAR(2048)  NULL,
	ContactPostCode			NVARCHAR(10) NULL,
	ContactEmail			NVARCHAR(256)  NULL,
	ContactPhone			NVARCHAR(20)  NULL,
	OrganisationName		NVARCHAR(256)  NULL,
	OrganisationAddress1	NVARCHAR(60) NOT NULL,
	OrganisationAddress2	NVARCHAR(60) NULL,
	OrganisationTownCity	NVARCHAR(35) NOT NULL,
	OrganisationCountyRegion NVARCHAR(35) NULL,
	OrganisationCountry		NVARCHAR(2048) NOT NULL,
	OrganisationPostcode	NVARCHAR(10) NULL,
	AatfAddress				NVARCHAR(500) NULL,
	OperatorName			NVARCHAR(256) NULL,
	OperatorTradingName		NVARCHAR(256) NULL,
	OperatorAddress			NVARCHAR(500) NULL,
	OrganisationType		INT NOT NULL,
	CompanyRegistrationNumber	NVARCHAR(15) NULL,
	OrganisationTelephone	NVARCHAR(20) NOT NULL,
	OrganisationEmail		NVARCHAR(256) NOT NULL,
	RecordType				NVARCHAR(4) NOT NULL,
	IbisCustomerReference	NVARCHAR(10) NULL,
	ObligationType			NVARCHAR(4) NULL
)

IF (@FacilityType = 4 OR @FacilityType != 3)
BEGIN
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
		c.FirstName,
		c.LastName,
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
		CONCAT(oa.Address1, COALESCE(', ' + NULLIF(oa.Address2, ''), ''), ', ', oa.TownOrCity,COALESCE(', ' + NULLIF(oa.CountyOrRegion, ''), '')),
		o.OrganisationType,
		o.CompanyRegistrationNumber,
		oa.Telephone,
		oa.Email,
		CASE WHEN A.FacilityType = 1 THEN 'AATF' ELSE 'AE' END,
		NULL, NULL 
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
		AND (@FacilityType = 4  OR A.FacilityType = @FacilityType)
		AND a.CompetentAuthorityId = COALESCE(@CA, a.CompetentAuthorityId)
		AND (@Area IS NULL OR a.LocalAreaId = COALESCE(@Area, a.LocalAreaId))
		AND (@PanArea IS NULL OR a.PanAreaId = COALESCE(@PanArea, a.PanAreaId))
END

IF (@FacilityType = 4 OR @FacilityType = 3)
BEGIN
	INSERT INTO @AATF (AppropriateAuthorityAbbr, Name, ApprovalNumber, Status, IbisCustomerReference, [ObligationType], FirstName, LastName, ContactPosition,
	OrganisationType, OperatorName, OperatorTradingName, CompanyRegistrationNumber, OrganisationAddress1,   
	OrganisationAddress2, OrganisationTownCity, OrganisationCountyRegion, OrganisationCountry, OrganisationPostcode, OrganisationTelephone, OrganisationEmail, RecordType,
	ContactAddress1, ContactAddress2, ContactTownCity, ContactCountyRegion, ContactCountry, ContactPostCode, ContactEmail, ContactPhone)
	SELECT ca.Abbreviation, s.SchemeName, s.ApprovalNumber, 
	CASE s.SchemeStatus
		WHEN 1 THEN 'Pending'
		WHEN 2 THEN 'Approved'
		WHEN 3 THEN 'Rejected'
		ELSE 'Withdrawn' END, 	
	s.IbisCustomerReference, 
	CASE s.ObligationType 
		WHEN 1 THEN 'B2B'
		WHEN 2 THEN 'B2C'
		WHEN 3 THEN 'Both'
		ELSE 'None' END,
	oc.FirstName, oc.LastName, oc.Position,
	o.OrganisationType, o.Name,o.TradingName, o.CompanyRegistrationNumber,
	oa.Address1, oa.Address2, oa.TownOrCity,oa.CountyOrRegion, orgco.Name, oa.Postcode,oa.Telephone, oa.Email, 'PCS',
	oca.Address1, oca.Address2, oca.TownOrCity,oca.CountyOrRegion, sco.Name,oca.Postcode, oca.Email, oca.Telephone
	FROM 
	[PCS].[Scheme] s
	JOIN Organisation.Organisation o ON s.OrganisationId  = o.Id
	LEFT JOIN Organisation.Address oa ON o.BusinessAddressId = oa.Id
	JOIN Lookup.Country orgco on oa.CountryId = orgco.Id
	LEFT JOIN Lookup.CompetentAuthority ca ON s.CompetentAuthorityId = ca.Id
	LEFT JOIN [Organisation].[Contact]  oc ON s.ContactId = oc.Id
	LEFT JOIN Organisation.Address oca ON s.AddressId = oca.Id
	JOIN Lookup.Country sco on oca.CountryId = sco.Id
	WHERE
	 s.CompetentAuthorityId = COALESCE(@CA, s.CompetentAuthorityId)
END

SELECT * FROM @AATF Order by Name

END	
GO


