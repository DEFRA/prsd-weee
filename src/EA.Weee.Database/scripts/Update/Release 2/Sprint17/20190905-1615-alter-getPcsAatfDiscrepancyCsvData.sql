
ALTER PROCEDURE [AATF].[getPcsAatfDiscrepancyCsvData]
	@ComplianceYear INT,
	@Quarter INT,
	@ObligationType nvarchar(3)
AS
BEGIN


DECLARE @SUBMITTEDRETURN TABLE
(
	AatfId					UNIQUEIDENTIFIER NOT NULL,
	ReturnId				UNIQUEIDENTIFIER NOT NULL,
	ComplianceYear			INT NOT NULL,
	[Quarter]				INT NOT NULL,
	Name					NVARCHAR(256) NOT NULL,
	ApprovalNumber			NVARCHAR(20) NOT NULL,	
	CompetentAuthorityAbbr	NVARCHAR(65) NOT NULL
)

	DECLARE @PCSDelivered TABLE
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
		[PcsTonnage]				DECIMAL(38, 3)
	)

	DECLARE @AatfObligated TABLE
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

INSERT INTO @SUBMITTEDRETURN
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


INSERT INTO @AatfObligated
SELECT u.SchemeId, u.SchemeName, u.ComplianceYear,  u.Name, u.AatfApprovalNumber, u.CompetentAuthorityAbbr, u.[Quarter], u.CategoryId, CASE WHEN u.Tonnage = 'HouseholdTonnage' THEN 'B2C'
			 ELSE 'B2B' END AS Obligation,
  u.ApprovalNumber, u.Abbreviation, SUM(u.value) AS AatfTonnage
FROM (
	SELECT r.ComplianceYear, r.Name, r.ApprovalNumber as AatfApprovalNumber, r.CompetentAuthorityAbbr, wr.SchemeId, r.AatfId, r.ReturnId, r.[Quarter], wra.CategoryId,
	 ISNULL(wra.HouseholdTonnage,0) HouseholdTonnage ,
	 ISNULL(wra.NonHouseholdTonnage,0) NonHouseholdTonnage, s.SchemeName, s.ApprovalNumber, ca.Abbreviation
	FROM @SUBMITTEDRETURN r
	INNER JOIN [AATF].WeeeReceived wr ON r.ReturnId = wr.ReturnId
		AND wr.AatfId = r.AatfId
	INNER JOIN [AATF].WeeeReceivedAmount wra ON wr.Id = wra.WeeeReceivedId
	INNER JOIN [PCS].[Scheme] S ON s.Id = wr.SchemeId
	INNER JOIN [Lookup].CompetentAuthority ca ON ca.Id = s.CompetentAuthorityId 
	) a
	UNPIVOT(value FOR Tonnage IN (a.HouseholdTonnage, a.NonHouseholdTonnage)) u
GROUP BY ComplianceYear, Name, AatfApprovalNumber, CompetentAuthorityAbbr, AatfId, ReturnId, [Quarter], Tonnage, CategoryId, u.SchemeId, u.SchemeName, u.ApprovalNumber, u.Abbreviation




INSERT INTO @PCSDelivered
SELECT
		S.Id,
		DR.[ComplianceYear],
		S.[ApprovalNumber],
		S.[SchemeName],
		ca.Abbreviation,
		DR.[Quarter],
		WDA.[WeeeCategory],
		WDA.[ObligationType],
		AATF.[ApprovalNumber] AS 'AatfApprovalNumber',
		a.Name,
		ca1.Abbreviation,
		WDA.[Tonnage]
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
			ON WDRVA.[WeeeDeliveredAmountId] = WDA.[Id]
	LEFT JOIN
		[PCS].[AatfDeliveryLocation] AATF
			ON WDA.[AatfDeliveryLocationId] = AATF.[Id]
	LEFT JOIN AATF.AATF a ON a.ApprovalNumber = AATF.ApprovalNumber AND A.ComplianceYear = @ComplianceYear
	LEFT JOIN [Lookup].CompetentAuthority ca1 ON ca1.Id = a.CompetentAuthorityId
	WHERE
		DR.[ComplianceYear] = @ComplianceYear
		AND (@Quarter = 0 or DR.[Quarter] = @Quarter)


		SELECT 
		@ComplianceYear AS ComplianceYear,
		CASE WHEN A.[Quarter] IS NULL THEN P.[Quarter] ELSE A.[Quarter] END AS [Quarter], 
		CASE WHEN A.Obligation IS NULL THEN P.ObligationType ELSE A.Obligation END AS ObligationType,		
		CASE WHEN A.CategoryId IS NULL THEN c1.Id ELSE c.Id END AS CategoryValue, 
		CASE WHEN A.CategoryId IS NULL THEN CONCAT(c1.Id, '. ', c1.Name) ELSE CONCAT(c.Id, '. ', c.Name) END AS Category, 
		CASE WHEN A.[Quarter] IS NULL THEN CONCAT('Q', P.[Quarter]) ELSE CONCAT('Q', A.[Quarter]) END AS QuarterValue,
		CASE WHEN P.SchemeName IS NULL THEN A.SchemeName ELSE P.SchemeName END AS SchemeNameValue, 
		CASE WHEN P.ApprovalNumber IS NULL THEN A.ApprovalNumber ELSE P.ApprovalNumber END AS PcsApprovalNumber, 
		CASE WHEN P.CompetentAuthorityAbbr IS NULL THEN A.PcsCompetentAuthorityAbbr ELSE P.CompetentAuthorityAbbr END AS PcsAbbreviation,
		CASE WHEN A.Name IS NULL THEN P.AatfName ELSE A.Name END AS AatfName, 
		A.AatfApprovalNumber,
		CASE WHEN A.CompetentAuthorityAbbr IS NULL THEN P.AatfCompetentAuthorityAbbr ELSE A.CompetentAuthorityAbbr END AS AatfAbbreviation, 
		ISNULL(P.PcsTonnage,0) PcsTonnage, ISNULL(A.AatfTonnage,0) AatfTonnage,
		ISNULL(P.PcsTonnage,0) - ISNULL(A.AatfTonnage,0) as DifferenceTonnage,
		P.WeeeCategory, A.CategoryId
		FROM
		@AatfObligated A FULL OUTER JOIN
		@PCSDelivered P ON  P.SchemeId = A.SchemeId
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
			OR ObligationType LIKE '%' + COALESCE(@ObligationType, ObligationType) + '%')
		ORDER BY [Quarter],ObligationType,CategoryValue,SchemeNameValue,AatfName


END
GO


