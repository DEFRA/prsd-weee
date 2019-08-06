IF OBJECT_ID('[AATF].getNonObligatedWeeeReceived', 'P') IS NOT NULL BEGIN
	DROP PROCEDURE [AATF].getNonObligatedWeeeReceived
END

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
CREATE PROCEDURE [AATF].getNonObligatedWeeeReceived
	@ComplianceYear INT,
	@Authority UNIQUEIDENTIFIER = NULL,
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
	(@OrganisationName IS NULL OR (o.[Name] LIKE '%' + @OrganisationName + '%' OR o.[TradingName] LIKE '%' + @OrganisationName + '%'))
)
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

INSERT INTO @NonObligatedResults
SELECT
	w.Id,
	CONVERT(DECIMAL(28,3), CASE WHEN SUM(CASE WHEN non.Dcf = 0 THEN non.Tonnage ELSE 0 END) IS NULL THEN 0 ELSE SUM(CASE WHEN non.Dcf = 0 THEN non.Tonnage ELSE 0 END) END), 
	CONVERT(DECIMAL(28,3), CASE WHEN SUM(CASE WHEN non.Dcf = 1 THEN non.Tonnage ELSE 0 END) IS NULL THEN 0 ELSE SUM(CASE WHEN non.Dcf = 1 THEN non.Tonnage ELSE 0 END) END),
	non.ReturnId
 FROM
	[Lookup].WeeeCategory w, [AATF].[NonObligatedWeee] non 
WHERE
	w.Id = non.CategoryId
 GROUP BY
	w.Id,
	non.ReturnId

SELECT
	'' AS Authority,
	'' AS PatArea,
	'' AS Area,
	r.ComplianceYear AS [Year],
	CONCAT('Q', r.[Quarter]) AS [Quarter],
	r.SubmittedDate,
	CONCAT(u.FirstName , ' ', u.Surname) AS SubmittedBy,
	CASE o.OrganisationType WHEN 3 THEN o.TradingName ELSE o.[Name] END AS OrganisationName,
	CONCAT(fr.CategoryId, '. ', fr.CategoryName) AS Category,
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
END
GO
