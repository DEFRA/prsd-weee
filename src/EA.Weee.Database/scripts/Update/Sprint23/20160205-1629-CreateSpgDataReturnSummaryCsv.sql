SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
 * This stored procedure aggragates the reported data for a specific
 * scheme and compliance year into a format which is suitable for
 * downloaded by external users.
 *
 * The procedure creates a table into which all WEEE collected, WEEE delivered
 * and EEE output amounts are inserted.
 *
 * The query then pivots this data to provide the sum for each of the 14
 * categories.
 */
CREATE PROCEDURE [PCS].[spgDataReturnSummaryCsv]
	@SchemeID UNIQUEIDENTIFIER,
	@ComplianceYear INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Amounts TABLE
	(
		[Quarter] INT NOT NULL,
		[Type] INT NOT NULL,
		[Category] INT NOT NULL,
		[Source] INT NULL,
		[ObligationType] NVARCHAR(4) NOT NULL,
		[Amount] DECIMAL(28, 3) NOT NULL
	)

	INSERT INTO @Amounts
	SELECT
		DR.[Quarter],
		0,
		WCA.[WeeeCategory],
		WCA.[SourceType],
		WCA.[ObligationType],
		SUM(WCA.Tonnage)
	FROM
		[PCS].[DataReturn] DR
	INNER JOIN
		[PCS].[DataReturnVersion] DRV
			ON DR.CurrentDataReturnVersionId = DRV.Id
	INNER JOIN
		[PCS].[WeeeCollectedReturnVersion] WCRV
			ON DRV.WeeeCollectedReturnVersionId = WCRV.Id
	INNER JOIN
		[PCS].[WeeeCollectedReturnVersionAmount] WCRVA
			ON WCRV.Id = WCRVA.WeeeCollectedReturnVersionId
	INNER JOIN
		[PCS].[WeeeCollectedAmount] WCA
			ON WCRVA.WeeeCollectedAmountId = WCA.Id
	WHERE
		DR.SchemeId = @SchemeID
	AND
		DR.ComplianceYear = @ComplianceYear
	GROUP BY
		DR.[Quarter],
		WCA.[WeeeCategory],
		WCA.[SourceType],
		WCA.[ObligationType]

	INSERT INTO @Amounts
	SELECT
		DR.[Quarter],
		1,
		WDA.[WeeeCategory],
		CASE
			WHEN WDA.AatfDeliveryLocationId IS NOT NULL THEN 0
			WHEN WDA.AeDeliveryLocationId IS NOT NULL THEN 1
		END AS [SourceType],
		WDA.[ObligationType],
		SUM(WDA.Tonnage)
	FROM
		[PCS].[DataReturn] DR
	INNER JOIN
		[PCS].[DataReturnVersion] DRV
			ON DR.CurrentDataReturnVersionId = DRV.Id
	INNER JOIN
		[PCS].[WeeeDeliveredReturnVersion] WDRV
			ON DRV.WeeeDeliveredReturnVersionId = WDRV.Id
	INNER JOIN
		[PCS].[WeeeDeliveredReturnVersionAmount] WDRVA
			ON WDRV.Id = WDRVA.WeeeDeliveredReturnVersionId
	INNER JOIN
		[PCS].[WeeeDeliveredAmount] WDA
			ON WDRVA.WeeeDeliveredAmountId = WDA.Id
	WHERE
		DR.SchemeId = @SchemeID
	AND
		DR.ComplianceYear = @ComplianceYear
	GROUP BY
		DR.[Quarter],
		WDA.[WeeeCategory],
		CASE
			WHEN WDA.AatfDeliveryLocationId IS NOT NULL THEN 0
			WHEN WDA.AeDeliveryLocationId IS NOT NULL THEN 1
		END,
		WDA.[ObligationType]

	INSERT INTO @Amounts
	SELECT
		DR.[Quarter],
		2,
		EOA.[WeeeCategory],
		NULL,
		EOA.[ObligationType],
		SUM(EOA.Tonnage)
	FROM
		[PCS].[DataReturn] DR
	INNER JOIN
		[PCS].[DataReturnVersion] DRV
			ON DR.CurrentDataReturnVersionId = DRV.Id
	INNER JOIN
		[PCS].[EeeOutputReturnVersion] EORV
			ON DRV.EeeOutputReturnVersionId = EORV.Id
	INNER JOIN
		[PCS].[EeeOutputReturnVersionAmount] EORVA
			ON EORV.Id = EORVA.EeeOutputReturnVersionId
	INNER JOIN
		[PCS].[EeeOutputAmount] EOA
			ON EORVA.EeeOuputAmountId = EOA.Id
	WHERE
		DR.SchemeId = @SchemeID
	AND
		DR.ComplianceYear = @ComplianceYear
	GROUP BY
		DR.[Quarter],
		EOA.[WeeeCategory],
		EOA.[ObligationType]
		
	SELECT
		[Quarter],
		[Type],
		[Source],
		[ObligationType],
		[1] AS 'Category1',
		[2] AS 'Category2',
		[3] AS 'Category3',
		[4] AS 'Category4',
		[5] AS 'Category5',
		[6] AS 'Category6',
		[7] AS 'Category7',
		[8] AS 'Category8',
		[9] AS 'Category9',
		[10] AS 'Category10',
		[11] AS 'Category11',
		[12] AS 'Category12',
		[13] AS 'Category13',
		[14] AS 'Category14'
	FROM
	(
		SELECT *
		FROM @Amounts
	) AS SourceTable
	PIVOT
	(
		SUM(Amount) FOR Category IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12], [13], [14])
	) AS PivotTable
	ORDER BY
		[Quarter],
		[Type],
		[Source],
		[ObligationType] DESC

END
GO