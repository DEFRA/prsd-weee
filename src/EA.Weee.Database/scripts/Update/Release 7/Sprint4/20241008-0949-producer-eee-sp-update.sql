SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [Producer].[spgProducerEeeCsvData]
    @ComplianceYear INT,
    @SchemeId UNIQUEIDENTIFIER = NULL,
    @ObligationType NVARCHAR(4)
AS
BEGIN
    -- Create temporary table to store EEE data
    CREATE TABLE #EEETable (
        PRN NVARCHAR(100),
        SchemeId UNIQUEIDENTIFIER,
        ApprovalNumber NVARCHAR(50),
        SchemeName NVARCHAR(255),
        ProducerName NVARCHAR(255),
        ProducerCountry NVARCHAR(100),
        Quarter INT,
        WeeeCategory INT,
        Tonnage DECIMAL(18, 3),
        IsDirectProducer BIT  -- New column to differentiate direct producers
    )

    -- Insert data into temporary table
    INSERT INTO #EEETable
    SELECT 
        RP.ProducerRegistrationNumber AS 'PRN',
        RP.SchemeId,
        S.ApprovalNumber,
        S.SchemeName,
        COALESCE(PBC.Name, PBP.Name, '') AS 'ProducerName',
        COALESCE(ROC_A_C.Name, PPOB_A_C.Name, '') AS 'ProducerCountry',
        DR.Quarter,
        EEOA.WeeeCategory,
        EEOA.Tonnage,
        0 AS IsDirectProducer  -- Not a direct producer
    FROM [PCS].DataReturn DR
    INNER JOIN [PCS].DataReturnVersion DRV ON DR.CurrentDataReturnVersionId = DRV.Id
    INNER JOIN [PCS].[EeeOutputReturnVersion] EEORV ON DRV.EeeOutputReturnVersionId = EEORV.Id
    INNER JOIN [PCS].[EeeOutputReturnVersionAmount] EEORVA ON EEORV.Id = EEORVA.EeeOutputReturnVersionId
    INNER JOIN [PCS].[EeeOutputAmount] EEOA ON EEORVA.EeeOuputAmountId = EEOA.Id
    INNER JOIN [Producer].[RegisteredProducer] RP ON EEOA.RegisteredProducerId = RP.Id
    INNER JOIN [PCS].Scheme S ON RP.SchemeId = S.Id
    INNER JOIN [Producer].[ProducerSubmission] PS ON RP.[CurrentSubmissionId] = PS.[Id]
    LEFT JOIN [Producer].[Business] PB ON PS.ProducerBusinessId = PB.Id
    LEFT JOIN [Producer].[Company] PBC ON PB.CompanyId = PBC.Id
    LEFT JOIN [Producer].[Partnership] PBP ON PB.PartnershipId = PBP.Id
    LEFT JOIN [Producer].[Contact] ROC 
        INNER JOIN [Producer].[Address] ROC_A 
            INNER JOIN [Lookup].[Country] ROC_A_C ON ROC_A.CountryId = ROC_A_C.Id     
        ON ROC.AddressId = ROC_A.Id
    ON PBC.RegisteredOfficeContactId = ROC.Id
    LEFT JOIN [Producer].[Contact] PPOB 
        INNER JOIN [Producer].[Address] PPOB_A 
            INNER JOIN [Lookup].[Country] PPOB_A_C ON PPOB_A.CountryId = PPOB_A_C.Id   
        ON PPOB.AddressId = PPOB_A.Id
    ON PBP.PrincipalPlaceOfBusinessId = PPOB.Id
    WHERE DR.ComplianceYear = @ComplianceYear
        AND RP.Removed = 0
        AND EEOA.ObligationType = @ObligationType
        AND (@SchemeId IS NULL OR S.[Id] = @SchemeId)

    UNION ALL

    SELECT
        RP.ProducerRegistrationNumber as 'PRN',
        dr.Id AS 'SchemeId',
        '' AS 'ApprovalNumber',
        'Direct registrant' AS SchemeName,
        COALESCE(ap.OverseasProducerName, o.[Name], '') AS 'ProducerName',
        COALESCE(ac.Name, oc.Name, '') AS 'ProducerCountry',
        4 AS Quarter,
        eoa.WeeeCategory,
        eoa.Tonnage,
        1 AS IsDirectProducer  -- This is a direct producer
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
        rp.Removed = 0 AND
        dps.Removed = 0 AND
        eoa.ObligationType = @ObligationType AND
        dps.ComplianceYear = (@ComplianceYear + 1) AND
        dps.[Status] = 2

    -- Create pivot table
    SELECT EeeData.PRN, EeeData.ProducerName, EeeData.ProducerCountry, EeeData.SchemeId, 
           EeeData.ApprovalNumber, EeeData.SchemeName, EeeData.Quarter,
           [1] AS Cat1, [2] AS Cat2, [3] AS Cat3, [4] AS Cat4, [5] AS Cat5, [6] AS Cat6, [7] AS Cat7, 
           [8] AS Cat8, [9] AS Cat9, [10] AS Cat10, [11] AS Cat11, [12] AS Cat12, [13] AS Cat13, [14] AS Cat14,
           EeeData.IsDirectProducer
    INTO #ProducerEEEData
    FROM 
    (
        SELECT *
        FROM #EEETable
    ) AS p
    PIVOT
    (
        MAX(p.Tonnage) 
        FOR p.WeeeCategory IN ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12],[13],[14])     
    ) AS EeeData

    -- Final result set
    SELECT 
        Producers.PRN, 
        Producers.ProducerName, 
        Producers.ProducerCountry, 
        Producers.SchemeName, 
        Producers.ApprovalNumber,
        COALESCE(Totals.TotalTonnage, 0) AS 'TotalTonnage',             
        Q1.Cat1Q1, Q2.Cat1Q2, Q3.Cat1Q3, Q4.Cat1Q4,
        Q1.Cat2Q1, Q2.Cat2Q2, Q3.Cat2Q3, Q4.Cat2Q4, 
        Q1.Cat3Q1, Q2.Cat3Q2, Q3.Cat3Q3, Q4.Cat3Q4, 
        Q1.Cat4Q1, Q2.Cat4Q2, Q3.Cat4Q3, Q4.Cat4Q4, 
        Q1.Cat5Q1, Q2.Cat5Q2, Q3.Cat5Q3, Q4.Cat5Q4, 
        Q1.Cat6Q1, Q2.Cat6Q2, Q3.Cat6Q3, Q4.Cat6Q4,  
        Q1.Cat7Q1, Q2.Cat7Q2, Q3.Cat7Q3, Q4.Cat7Q4,  
        Q1.Cat8Q1, Q2.Cat8Q2, Q3.Cat8Q3, Q4.Cat8Q4, 
        Q1.Cat9Q1, Q2.Cat9Q2, Q3.Cat9Q3, Q4.Cat9Q4,  
        Q1.Cat10Q1, Q2.Cat10Q2, Q3.Cat10Q3, Q4.Cat10Q4,  
        Q1.Cat11Q1, Q2.Cat11Q2, Q3.Cat11Q3, Q4.Cat11Q4,  
        Q1.Cat12Q1, Q2.Cat12Q2, Q3.Cat12Q3, Q4.Cat12Q4,  
        Q1.Cat13Q1, Q2.Cat13Q2, Q3.Cat13Q3, Q4.Cat13Q4,  
        Q1.Cat14Q1, Q2.Cat14Q2, Q3.Cat14Q3, Q4.Cat14Q4 
    FROM (
        SELECT PE.PRN, PE.SchemeId, PE.ApprovalNumber, PE.SchemeName, PE.ProducerName, PE.ProducerCountry, PE.IsDirectProducer
        FROM #ProducerEEEData PE    
        GROUP BY PE.PRN, PE.SchemeId, PE.ApprovalNumber, PE.SchemeName, PE.ProducerName, PE.ProducerCountry, PE.IsDirectProducer
    ) AS Producers 
    INNER JOIN (
        SELECT E.PRN, E.SchemeId, SUM(COALESCE(E.Tonnage, 0)) AS 'TotalTonnage'
        FROM #EEETable E
        GROUP BY E.PRN, E.SchemeId
    ) AS Totals ON (Producers.PRN = Totals.PRN AND Producers.SchemeId = Totals.SchemeId)
    LEFT JOIN (
        SELECT EEEData.PRN, EEEData.SchemeId, 
               EEEData.Cat1 AS Cat1Q1, EEEData.Cat2 AS Cat2Q1, EEEData.Cat3 AS Cat3Q1, 
               EEEData.Cat4 AS Cat4Q1, EEEData.Cat5 AS Cat5Q1, EEEData.Cat6 AS Cat6Q1, 
               EEEData.Cat7 AS Cat7Q1, EEEData.Cat8 AS Cat8Q1, EEEData.Cat9 AS Cat9Q1, 
               EEEData.Cat10 AS Cat10Q1, EEEData.Cat11 AS Cat11Q1, EEEData.Cat12 AS Cat12Q1,
               EEEData.Cat13 AS Cat13Q1, EEEData.Cat14 AS Cat14Q1
        FROM #ProducerEEEData EEEData
        WHERE EEEData.Quarter = 1
    ) AS Q1 ON (Producers.PRN = Q1.PRN AND Producers.SchemeId = Q1.SchemeId)
    LEFT JOIN (
        SELECT EEEData.PRN, EEEData.SchemeId,        
               EEEData.Cat1 AS Cat1Q2, EEEData.Cat2 AS Cat2Q2, EEEData.Cat3 AS Cat3Q2, 
               EEEData.Cat4 AS Cat4Q2, EEEData.Cat5 AS Cat5Q2, EEEData.Cat6 AS Cat6Q2, 
               EEEData.Cat7 AS Cat7Q2, EEEData.Cat8 AS Cat8Q2, EEEData.Cat9 AS Cat9Q2, 
               EEEData.Cat10 AS Cat10Q2, EEEData.Cat11 AS Cat11Q2, EEEData.Cat12 AS Cat12Q2,
               EEEData.Cat13 AS Cat13Q2, EEEData.Cat14 AS Cat14Q2
        FROM #ProducerEEEData EEEData
        WHERE EEEData.Quarter = 2
    ) AS Q2 ON (Producers.PRN = Q2.PRN AND Producers.SchemeId = Q2.SchemeId)
    LEFT JOIN (
        SELECT EEEData.PRN, EEEData.SchemeId, 
               EEEData.Cat1 AS Cat1Q3, EEEData.Cat2 AS Cat2Q3, EEEData.Cat3 AS Cat3Q3, 
               EEEData.Cat4 AS Cat4Q3, EEEData.Cat5 AS Cat5Q3, EEEData.Cat6 AS Cat6Q3, 
               EEEData.Cat7 AS Cat7Q3, EEEData.Cat8 AS Cat8Q3, EEEData.Cat9 AS Cat9Q3, 
               EEEData.Cat10 AS Cat10Q3, EEEData.Cat11 AS Cat11Q3, EEEData.Cat12 AS Cat12Q3,
               EEEData.Cat13 AS Cat13Q3, EEEData.Cat14 AS Cat14Q3
        FROM #ProducerEEEData EEEData
        WHERE EEEData.Quarter = 3
    ) AS Q3 ON (Producers.PRN = Q3.PRN AND Producers.SchemeId = Q3.SchemeId)
    LEFT JOIN (
        SELECT EEEData.PRN, EEEData.SchemeId, 
               EEEData.Cat1 AS Cat1Q4, EEEData.Cat2 AS Cat2Q4, EEEData.Cat3 AS Cat3Q4, 
               EEEData.Cat4 AS Cat4Q4, EEEData.Cat5 AS Cat5Q4, EEEData.Cat6 AS Cat6Q4, 
               EEEData.Cat7 AS Cat7Q4, EEEData.Cat8 AS Cat8Q4, EEEData.Cat9 AS Cat9Q4, 
               EEEData.Cat10 AS Cat10Q4, EEEData.Cat11 AS Cat11Q4, EEEData.Cat12 AS Cat12Q4,
               EEEData.Cat13 AS Cat13Q4, EEEData.Cat14 AS Cat14Q4
        FROM #ProducerEEEData EEEData
        WHERE EEEData.Quarter = 4
    ) AS Q4 ON (Producers.PRN = Q4.PRN AND Producers.SchemeId = Q4.SchemeId)
    ORDER BY Producers.IsDirectProducer, SchemeName, ProducerName

    -- Clean up temporary tables
        -- Clean up temporary tables
    DROP TABLE #EEETable
    DROP TABLE #ProducerEEEData
END