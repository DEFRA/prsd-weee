/*
 * This script creates the [PCS].[SpgSchemeWeeeCsv] stored procedure
 * used by the admin report for collected/delivered WEEE.
 */
GO
-- =============================================
-- Author:		Graham Alexander-Thomson
-- Create date: 2016 Jan 07
-- Description:	This stored procedure is used to provide the data for the admin report of WEEE
--				collected and delievered.
--				To reduce the size of the data, the scheme information is split out into a
--				separate result set.
--				The collected amounts and delivered amounts are then returned in two additional
--				result sets.
-- =============================================
CREATE PROCEDURE [PCS].[SpgSchemeWeeeCsv]
	@ComplianceYear INT,
	@ObligationType NVARCHAR(4)
AS
BEGIN
	DECLARE @Collected TABLE
	(
		[SchemeId]		UNIQUEIDENTIFIER,
		[Quarter]		INT,
		[WeeeCategory]	INT,
		[SourceType]	INT,
		[Tonnage]		DECIMAL(38, 3)
	)

	DECLARE @Delivered TABLE
	(
		[SchemeId]					UNIQUEIDENTIFIER,
		[Quarter]					INT,
		[WeeeCategory]				INT,
		[LocationType]				INT,
		[LocationApprovalNumber]	NVARCHAR(50),
		[Tonnage]					DECIMAL(38, 3)
	)

	INSERT INTO
		@Collected
	SELECT
		S.[Id],
		DR.[Quarter],
		WCA.[WeeeCategory],
		WCA.[SourceType],
		WCA.[Tonnage]
	FROM
		[PCS].[DataReturn] DR
	INNER JOIN
		[PCS].[Scheme] S
			ON DR.[SchemeId] = S.[Id]
	INNER JOIN
		[PCS].[DataReturnVersion] DRV
			ON DR.[CurrentDataReturnVersionId] = DRV.[Id]
	INNER JOIN
		[PCS].[WeeeCollectedReturnVersion] WCRV
			ON DRV.[WeeeCollectedReturnVersionId] = WCRV.[Id]
	INNER JOIN
		[PCS].[WeeeCollectedReturnVersionAmount] WCRVA
			ON WCRV.[Id] = WCRVA.[WeeeCollectedReturnVersionId]
	INNER JOIN
		[PCS].[WeeeCollectedAmount] WCA
			ON WCRVA.[WeeeCollectedAmountId] = WCA.[Id]
	WHERE
		DR.[ComplianceYear] = @ComplianceYear
	AND
		WCA.[ObligationType] = @ObligationType

	INSERT INTO
		@Delivered
	SELECT
		S.[Id],
		DR.[Quarter],
		WDA.[WeeeCategory],
		CASE 
			WHEN AATF.[Id] IS NOT NULL THEN 0
			WHEN AE.[Id] IS NOT NULL THEN 1
			ELSE NULL
		END AS 'LocationType',
		CASE 
			WHEN AATF.[Id] IS NOT NULL THEN AATF.[ApprovalNumber]
			WHEN AE.[Id] IS NOT NULL THEN AE.[ApprovalNumber]
			ELSE NULL
		END AS 'LocationApprovalNumber',
		WDA.[Tonnage]
	FROM
		[PCS].[DataReturn] DR
	INNER JOIN
		[PCS].[Scheme] S
			ON DR.[SchemeId] = S.[Id]
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
	LEFT JOIN
		[PCS].[AeDeliveryLocation] AE
			ON WDA.[AeDeliveryLocationId] = AE.[Id]
	WHERE
		DR.[ComplianceYear] = @ComplianceYear
	AND
		WDA.[ObligationType] = @ObligationType

	-- Result set 1 provides information about the schemes referenced by the collected
	-- and delivered result sets.
	SELECT
		S.[Id],
		S.[ApprovalNumber],
		S.[SchemeName]
	FROM
		[PCS].[Scheme] S
	WHERE
		S.Id IN
		(
			SELECT
				[SchemeId]
			FROM
				@Collected
			UNION SELECT
				[SchemeId]
			FROM
				@Delivered
		)

	-- Result set 2 provides the details of WEEE that was collected.
	SELECT
		[SchemeId],
		[Quarter],
		[WeeeCategory],
		[SourceType],
		[Tonnage]
	FROM
		@Collected
	
	-- Result set 3 provides the details of WEEE that was delivered.
	SELECT
		[SchemeId],
		[Quarter],
		[WeeeCategory],
		[LocationType],
		[LocationApprovalNumber],
		[Tonnage]
	FROM
		@Delivered
END
GO
