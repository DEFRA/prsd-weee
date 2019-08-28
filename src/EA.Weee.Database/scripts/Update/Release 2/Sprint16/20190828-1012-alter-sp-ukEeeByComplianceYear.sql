IF OBJECT_ID('[Producer].[SpgUKEEEDataByComplianceYear]', 'P') IS NOT NULL BEGIN
	DROP PROCEDURE [Producer].[SpgUKEEEDataByComplianceYear]
END
GO
GO
CREATE PROCEDURE [Producer].[SpgUKEEEDataByComplianceYear]
	@ComplianceYear INT
AS
BEGIN

-- Weee category
CREATE TABLE #WeeeCategory(
 ID int,
 Name nvarchar(250))

INSERT INTO #WeeeCategory (ID, Name)
SELECT
	Id,
	CONCAT(Id, '. ', [Name])
FROM
	[Lookup].WeeeCategory w

-- EEE data	
SELECT
		EEOA.WeeeCategory,
		DR.Quarter,
		EEOA.ObligationType,
		EEOA.Tonnage
INTO #EEETable

	from [PCS].DataReturn DR

	INNER JOIN [PCS].DataReturnVersion DRV 
		on DR.CurrentDataReturnVersionId = DRV.Id

	INNER JOIN [PCS].[EeeOutputReturnVersion] EEORV
		on DRV.EeeOutputReturnVersionId = EEORV.Id

	INNER JOIN [PCS].[EeeOutputReturnVersionAmount] EEORVA
		on EEORV.Id = EEORVA.EeeOutputReturnVersionId

	INNER JOIN [PCS].[EeeOutputAmount] EEOA
		on EEORVA.EeeOuputAmountId = EEOA.Id

	INNER JOIN [Producer].[RegisteredProducer] RP
		on EEOA.RegisteredProducerId = RP.Id

	where DR.ComplianceYear = @ComplianceYear
		AND
		RP.Removed = 0

SELECT
EEEData.ID, EEEData.Name, EEEData.ObligationType, [1] AS Q1, [2] AS Q2, [3] AS Q3, [4] AS Q4
INTO #EEEDataTable
 FROM
 (
   select 
		WC.ID,
		WC.Name,
		EE.Quarter,
		EE.ObligationType,
		EE.Tonnage
	from Lookup.WeeeCategory WC
		LEFT JOIN #EEETable EE
		ON WC.ID = EE.WeeeCategory

 ) AS EE
PIVOT
(
  Sum(Tonnage) FOR Quarter IN ([1], [2], [3], [4])
) AS EEEData


SELECT WC.ID, WC.Name, EEEDataForB2B.Q1, EEEDataForB2B.Q2, EEEDataForB2B.Q3, EEEDataForB2B.Q4
INTO #EEEDataForB2BTable
FROM #WeeeCategory WC
	LEFT JOIN #EEEDataTable EEEDataForB2B
		ON WC.ID = EEEDataForB2B.ID AND EEEDataForB2B.ObligationType = 'B2B'


SELECT WC.ID, WC.Name, EEEDataForB2C.Q1, EEEDataForB2C.Q2, EEEDataForB2C.Q3, EEEDataForB2C.Q4
INTO #EEEDataForB2CTable
FROM #WeeeCategory WC
	LEFT JOIN #EEEDataTable EEEDataForB2C
		ON WC.ID = EEEDataForB2C.ID AND EEEDataForB2C.ObligationType = 'B2C'


SELECT 
EEEDataForB2B.Name AS 'Category', 
ISNULL(EEEDataForB2B.Q1, 0) + ISNULL(EEEDataForB2B.Q2, 0) + ISNULL( EEEDataForB2B.Q3, 0) + ISNULL(EEEDataForB2B.Q4, 0) AS 'TotalB2BEEE',
EEEDataForB2B.Q1 AS 'Q1B2BEEE',
EEEDataForB2B.Q2 AS 'Q2B2BEEE',
EEEDataForB2B.Q3 AS 'Q3B2BEEE',
EEEDataForB2B.Q4 AS 'Q4B2BEEE',
ISNULL(EEEDataForB2C.Q1, 0) + ISNULL(EEEDataForB2C.Q2, 0) + ISNULL(EEEDataForB2C.Q3, 0) + ISNULL(EEEDataForB2C.Q4, 0) AS 'TotalB2CEEE',
EEEDataForB2C.Q1 AS 'Q1B2CEEE',
EEEDataForB2C.Q2 AS 'Q2B2CEEE',
EEEDataForB2C.Q3 AS 'Q3B2CEEE',
EEEDataForB2C.Q4 AS 'Q4B2CEEE'
FROM #EEEDataForB2BTable EEEDataForB2B
INNER JOIN #EEEDataForB2CTable EEEDataForB2C
ON EEEDataForB2B.ID = EEEDataForB2C.ID
ORDER BY EEEDataForB2B.ID

END