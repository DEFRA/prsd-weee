IF OBJECT_ID('[AATF].getAllAatfReuseSitesCsvData', 'P') IS NOT NULL BEGIN
	DROP PROCEDURE [AATF].[getAllAatfReuseSitesCsvData]
END
GO
-- Description:	This stored procedure is used to provide the data for the admin report of reuse sites
--				for the latest submitted return within
--				the limits of the specified parameters.
-- =============================================
CREATE PROCEDURE [AATF].[getAllAatfReuseSitesCsvData]
	@ComplianceYear INT,
	@CA UNIQUEIDENTIFIER,
	@PanArea UNIQUEIDENTIFIER
AS
BEGIN
w
SET NOCOUNT ON;

---Get reuse sites for the latest submitted return for the compliance year

SELECT 
	X.ComplianceYear,
	CONCAT('Q',X.[Quarter]) AS [Quarter],
	X.CreatedDate,
	X.SubmittedDate,
	X.SubmittedBy,
	X.Name,
	X.ApprovalNumber,
	X.OrgName,
	X.Abbreviation,
	X.PanName,
	X.LaName,
	wrs.AddressId,
	ad.Name AS SiteName,
	CONCAT(ad.Address1, COALESCE(', ' + NULLIF(ad.Address2, ''), ''), ', ', ad.TownOrCity,
		COALESCE(', ' + NULLIF(ad.CountyOrRegion, ''), ''), COALESCE(', ' + NULLIF(ad.Postcode, ''), ''), ', ', c.Name) AS SiteAddress,
	X.AatfId,
	X.ReturnId,
	X.[Quarter] as QuarterValue
FROM (
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
		pa.Name AS PanName,
		la.Name AS LaName,
		ROW_NUMBER() OVER (PARTITION BY ra.AatfId, r.[Quarter] ORDER BY r.[Quarter], r.SubmittedDate DESC) AS RowNumber
FROM
	[AATF].[ReturnAatf] ra
	INNER JOIN [AATF].[Return] r ON r.Id = ra.ReturnId
	INNER JOIN AATF.AATF a ON a.Id = ra.AatfId AND a.FacilityType = r.FacilityType
	INNER JOIN Organisation.Organisation o ON a.OrganisationId = o.Id
	INNER JOIN [Lookup].CompetentAuthority ca ON a.CompetentAuthorityId = ca.Id
	INNER JOIN [Identity].[AspNetUsers] u ON u.id = r.SubmittedById
	LEFT JOIN [Lookup].[LocalArea] la ON la.Id = a.LocalAreaId
	LEFT JOIN [Lookup].[PanArea] pa ON pa.Id = a.PanAreaId
	WHERE r.ComplianceYear = @ComplianceYear
		AND r.ReturnStatus = 2 -- submitted
		AND a.FacilityType = 1 --aatf
		AND a.CompetentAuthorityId = COALESCE(@CA, a.CompetentAuthorityId)
		AND (
			@PanArea IS NULL
			OR a.PanAreaId = COALESCE(@PanArea, a.PanAreaId)
			)		
	) X
	INNER JOIN [AATF].[ReturnReportOn] ro ON ro.ReturnId = X.ReturnId AND ro.ReportOnQuestionId = 3 --reuse option
	INNER JOIN [AATF].WeeeReused wr ON wr.ReturnId = X.ReturnId	AND wr.AatfId = X.AatfId
	INNER JOIN [AATF].WeeeReusedSite wrs ON wrs.WeeeReusedId = wr.Id
	INNER JOIN [AATF].[Address] Ad ON ad.Id = wrs.AddressId
	LEFT JOIN [Lookup].[Country] c ON c.Id = ad.CountryId
WHERE
	X.RowNumber = 1
ORDER BY
	QuarterValue

END
GO


