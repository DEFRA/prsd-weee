GO
/****** Object:  StoredProcedure [AATF].[[getAatfAeDetailsCsvData]]    Script Date: 23/08/2019 13:48:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
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
	PanAreaTeam				NVARCHAR(256) NOT NULL,
	EaArea					NVARCHAR(256) NOT NULL,
	Name					NVARCHAR(256) NOT NULL,
	Address					NVARCHAR(500) NOT NULL,
	PostCode				NVARCHAR(10) NOT NULL,
	ApprovalNumber			NVARCHAR(20) NOT NULL,
	ApprovalDate			Datetime2(7) NOT NULL,
	Size					NVARCHAR(20) NOT NULL,
	Status					NVARCHAR(20) NOT NULL,
	ContactName				NVARCHAR(256) NOT NULL,
	ContactPostition		NVARCHAR(256) NOT NULL,
	ContactAddress			NVARCHAR(500) NOT NULL,
	ContactEmail			NVARCHAR(256) NOT NULL,
	ContactPhone			NVARCHAR(20) NOT NULL,
	OrganisationName		NVARCHAR(256) NOT NULL,
	OrganisationAddress		NVARCHAR(500) NOT NULL,
	OrganisationPostcode	NVARCHAR(10) NOT NULL	
)

INSERT INTO @AATF
	SELECT 
		@ComplianceYear,
		ca.Abbreviation,
		pa.Name,
		la.Name,
		a.Name,
		CONCAT(ad.Address1, COALESCE(', ' + NULLIF(ad.Address2, ''), ''), ', ', ad.TownOrCity,COALESCE(', ' + NULLIF(ad.CountyOrRegion, ''), ''), ''),
		ad.Postcode,
		a.ApprovalNumber,
		a.ApprovalDate,
		CASE WHEN a.Size = 1 THEN 'Small' ELSE 'Large' END,
		CASE WHEN a.Status = 1 THEN 'Approved' WHEN a.Status = 2 THEN 'Suspended'  ELSE 'Cancelled' END,
		c.FirstName + ' ' + c.LastName,
		c.Position,
		CONCAT(c.Address1, COALESCE(', ' + NULLIF(c.Address2, ''), ''), ', ', c.TownOrCity,COALESCE(', ' + NULLIF(c.CountyOrRegion, ''), ''), ''),
		c.Email, 
		c.Telephone,
		CASE WHEN o.Name IS NULL THEN o.TradingName ELSE o.Name END,
		CONCAT(oa.Address1, COALESCE(', ' + NULLIF(oa.Address2, ''), ''), ', ', oa.TownOrCity,COALESCE(', ' + NULLIF(oa.CountyOrRegion, ''), ''), ''),
		oa.Postcode
	
	FROM
		AATF.AATF a 
		JOIN Organisation.Organisation o ON a.OrganisationId  = o.Id
		JOIN Organisation.Address oa ON o.BusinessAddressId = oa.Id
		JOIN Lookup.CompetentAuthority ca ON a.CompetentAuthorityId = ca.Id
		JOIN Lookup.PanArea pa on a.PanAreaId = pa.Id
		JOIN Lookup.LocalArea la on a.LocalAreaId = la.Id
		JOIN AATF.Address ad on a.SiteAddressId = ad.Id
		JOIN AATF.Contact c on a.ContactId = c.Id
	WHERE 
		A.ComplianceYear = @ComplianceYear 
		AND A.FacilityType = @FacilityType
		AND a.CompetentAuthorityId = COALESCE(@CA, a.CompetentAuthorityId)
		AND (@Area IS NULL OR a.LocalAreaId = COALESCE(@Area, a.LocalAreaId))
		AND (@PanArea IS NULL OR a.PanAreaId = COALESCE(@PanArea, a.PanAreaId))



SELECT * FROM @AATF

END