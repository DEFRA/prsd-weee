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
ORDER BY
	r.ComplianceYear, r.[Quarter], OrganisationName
END
GO


