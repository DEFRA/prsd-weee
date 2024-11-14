SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [Producer].[SpgUKEEEDataByComplianceYear] 
    @ComplianceYear INT
AS
BEGIN

-- Weee category
CREATE TABLE #WeeeCategory
(
    ID int,
    Name nvarchar(250)
)

INSERT INTO #WeeeCategory
(
    ID,
    Name
)
SELECT Id,
        dbo.fn_CategoryName(Id, Name) AS [Name]
FROM [Lookup].WeeeCategory w

-- EEE data	
SELECT EEOA.WeeeCategory,
        DR.Quarter,
        EEOA.ObligationType,
        EEOA.Tonnage
INTO #EEETable
from [PCS].DataReturn DR
    INNER JOIN [PCS].DataReturnVersion DRV
        ON DR.CurrentDataReturnVersionId = DRV.Id
    INNER JOIN [PCS].[EeeOutputReturnVersion] EEORV
        ON DRV.EeeOutputReturnVersionId = EEORV.Id
    INNER JOIN [PCS].[EeeOutputReturnVersionAmount] EEORVA
        ON EEORV.Id = EEORVA.EeeOutputReturnVersionId
    INNER JOIN [PCS].[EeeOutputAmount] EEOA
        ON EEORVA.EeeOuputAmountId = EEOA.Id
    INNER JOIN [Producer].[RegisteredProducer] RP
        ON EEOA.RegisteredProducerId = RP.Id
WHERE DR.ComplianceYear = @ComplianceYear
        AND RP.Removed = 0

UNION ALL

SELECT
    EOA.WeeeCategory,
    4 AS Quarter,
    EOA.ObligationType,
    EOA.Tonnage
FROM
    [Producer].[DirectProducerSubmission] DPS
	INNER JOIN [Producer].[DirectRegistrant] DR ON DR.Id = DPS.DirectRegistrantId
	INNER JOIN [Organisation].[Organisation] O ON O.Id = DR.OrganisationId
	INNER JOIN [Organisation].[Address] OA ON OA.Id = O.BusinessAddressId
	INNER JOIN [Lookup].[Country] OC ON OC.Id = OA.CountryId
	INNER JOIN [Producer].[RegisteredProducer] RP ON DPS.RegisteredProducerId = RP.Id
	INNER JOIN (
		SELECT 
			DirectProducerSubmissionId,
			EeeOutputReturnVersionId,
			Id,
			ROW_NUMBER() OVER (PARTITION BY DirectProducerSubmissionId ORDER BY SubmittedDate DESC) AS RowNum
		FROM [Producer].[DirectProducerSubmissionHistory]
		WHERE SubmittedDate IS NOT NULL
	) DPSH ON DPSH.DirectProducerSubmissionId = DPS.Id AND DPSH.RowNum = 1
	INNER JOIN [PCS].[EeeOutputReturnVersion] EORV ON EORV.Id = DPSH.EeeOutputReturnVersionId
	INNER JOIN [PCS].[EeeOutputReturnVersionAmount] EORVA ON EORVA.EeeOutputReturnVersionId = EORV.Id
	INNER JOIN [PCS].[EeeOutputAmount] EOA ON EOA.Id = EORVA.EeeOuputAmountId
	LEFT JOIN [Producer].[AuthorisedRepresentative] AP ON AP.Id = DR.AuthorisedRepresentativeId
	LEFT JOIN [Producer].[Contact] PC ON PC.Id = AP.OverseasContactId
	LEFT JOIN [Producer].[Address] PA ON PA.Id = PC.AddressId
	LEFT JOIN [Lookup].[Country] AC ON AC.Id = PA.CountryId
WHERE
    RP.Removed = 0 AND
    dps.ComplianceYear = (@ComplianceYear + 1) AND
    dps.PaymentFinished = 1

-- Create pivot table
SELECT EEEData.ID,
        EEEData.Name,
        EEEData.ObligationType,
        [1] AS Q1,
        [2] AS Q2,
        [3] AS Q3,
        [4] AS Q4
INTO #EEEDataTable
FROM
(
    select WC.ID,
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
    Sum(Tonnage)
    FOR Quarter IN ([1], [2], [3], [4])
) AS EEEData


SELECT WC.ID,
        WC.Name,
        EEEDataForB2B.Q1,
        EEEDataForB2B.Q2,
        EEEDataForB2B.Q3,
        EEEDataForB2B.Q4
INTO #EEEDataForB2BTable
FROM #WeeeCategory WC
    LEFT JOIN #EEEDataTable EEEDataForB2B
        ON WC.ID = EEEDataForB2B.ID
            AND EEEDataForB2B.ObligationType = 'B2B'


SELECT WC.ID,
        WC.Name,
        EEEDataForB2C.Q1,
        EEEDataForB2C.Q2,
        EEEDataForB2C.Q3,
        EEEDataForB2C.Q4
INTO #EEEDataForB2CTable
FROM #WeeeCategory WC
    LEFT JOIN #EEEDataTable EEEDataForB2C
        ON WC.ID = EEEDataForB2C.ID
            AND EEEDataForB2C.ObligationType = 'B2C'

-- Final result set
SELECT EEEDataForB2B.Name AS 'Category',
        ISNULL(EEEDataForB2B.Q1, 0) + ISNULL(EEEDataForB2B.Q2, 0) + ISNULL(EEEDataForB2B.Q3, 0)
        + ISNULL(EEEDataForB2B.Q4, 0) AS 'TotalB2BEEE',
        EEEDataForB2B.Q1 AS 'Q1B2BEEE',
        EEEDataForB2B.Q2 AS 'Q2B2BEEE',
        EEEDataForB2B.Q3 AS 'Q3B2BEEE',
        EEEDataForB2B.Q4 AS 'Q4B2BEEE',
        ISNULL(EEEDataForB2C.Q1, 0) + ISNULL(EEEDataForB2C.Q2, 0) + ISNULL(EEEDataForB2C.Q3, 0)
        + ISNULL(EEEDataForB2C.Q4, 0) AS 'TotalB2CEEE',
        EEEDataForB2C.Q1 AS 'Q1B2CEEE',
        EEEDataForB2C.Q2 AS 'Q2B2CEEE',
        EEEDataForB2C.Q3 AS 'Q3B2CEEE',
        EEEDataForB2C.Q4 AS 'Q4B2CEEE'
FROM #EEEDataForB2BTable EEEDataForB2B
    INNER JOIN #EEEDataForB2CTable EEEDataForB2C
        ON EEEDataForB2B.ID = EEEDataForB2C.ID
ORDER BY EEEDataForB2B.ID

END
GO
