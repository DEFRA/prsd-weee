﻿SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Priety Mahajan
-- Create date: 2016 Jan 28
-- Description:	This stored procedure is used to provide the EEE data history for producer.
-- =============================================
ALTER PROCEDURE [Producer].[spgProducerEeeHistoryCsvDataByPRN]
    @PRN  NVARCHAR(50)
AS
BEGIN
SET NOCOUNT ON;

SELECT DISTINCT 
        RP.ProducerRegistrationNumber as 'PRN',
        S.SchemeName,
        S.ApprovalNumber,		
        DR.ComplianceYear,		
        DRV.SubmittedDate,
        DR.Quarter,	
        EEOA.ObligationType,	
        EEOA.WeeeCategory,
        EEOA.Tonnage,
        EEORV.Id
        
INTO #EEETable
    from [PCS].DataReturnVersion DRV

    INNER JOIN [PCS].DataReturn DR
        on DRV.DataReturnId = DR.Id

    INNER JOIN [PCS].[EeeOutputReturnVersion] EEORV
        on DRV.EeeOutputReturnVersionId = EEORV.Id

    INNER JOIN [PCS].[EeeOutputReturnVersionAmount] EEORVA
        on EEORV.Id = EEORVA.EeeOutputReturnVersionId

    INNER JOIN [PCS].[EeeOutputAmount] EEOA
        on EEORVA.EeeOuputAmountId = EEOA.Id
    
    INNER JOIN [PCS].Scheme S
        on DR.SchemeId = S.Id	

    INNER JOIN [Producer].RegisteredProducer RP
        on EEOA.RegisteredProducerId = RP.Id
    where 
        RP.ProducerRegistrationNumber = @PRN

SELECT DISTINCT 
ROW_NUMBER() over
                    (
                    partition by (EeeData.Id)
                    Order by ComplianceYear desc, SubmittedDate desc
                    ) as RowNumber, 
            EeeData.Id, 
            EeeData.PRN,EeeData.ApprovalNumber, EeeData.SchemeName, EeeData.ComplianceYear, EeeData.SubmittedDate, EeeData.Quarter, EeeData.ObligationType,
            [1] AS Cat1, [2] AS Cat2, [3] AS Cat3, [4] AS Cat4, [5] AS Cat5, [6] AS Cat6, [7] AS Cat7, [8] AS Cat8, [9] AS Cat9,
            [10] AS Cat10, [11] AS Cat11, [12] AS Cat12, [13] AS Cat13, [14] AS Cat14
            INTO #ProducerEEEData
            FROM 
            (SELECT *
            FROM #EEETable) AS p
            PIVOT
             (
                    max(p.Tonnage) 
                    for p.WeeeCategory in ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12],[13],[14]) 	
              ) as EeeData



SELECT DISTINCT 
        --Producers.Id,Producers.PRN, 
        Producers.ApprovalNumber, Producers.SchemeName, Producers.ComplianceYear, 		
                        Producers.SubmittedDate,Producers.Quarter,
                            case LatestRecord.SubmissionRecordRank  
                                    WHEN 1 then 'Yes' ELSE 'No' END as 'LatestData',				 
                        B2C.Cat1B2C,B2C.Cat2B2C,B2C.Cat3B2C, B2C.Cat4B2C, B2C.Cat5B2C, B2C.Cat6B2C, B2C.Cat7B2C, B2C.Cat8B2C,B2C.Cat9B2C,
                        B2C.Cat10B2C,B2C.Cat11B2C, B2C.Cat12B2C, B2C.Cat13B2C,B2C.Cat14B2C, 
                        B2B.Cat1B2B,B2B.Cat2B2B,B2B.Cat3B2B, B2B.Cat4B2B, B2B.Cat5B2B, B2B.Cat6B2B, B2B.Cat7B2B, B2B.Cat8B2B,B2B.Cat9B2B,
                B2B.Cat10B2B,B2B.Cat11B2B, B2B.Cat12B2B, B2B.Cat13B2B,B2B.Cat14B2B
        FROM (
                    SELECT 			
                        PE.Id, PE.PRN,  PE.ApprovalNumber, PE.SchemeName, PE.ComplianceYear, PE.SubmittedDate, PE.Quarter
                    FROM  #ProducerEEEData PE		
                    where PE.RowNumber = 1		
                    GROUP BY  Pe.Id, PE.PRN,  PE.ApprovalNumber, PE.SchemeName, PE.ComplianceYear, PE.SubmittedDate, PE.Quarter
             ) AS Producers 
    
      
--B2C 
LEFT JOIN (
SELECT DISTINCT EEEData.Id, EEEData.PRN, EEEData.ApprovalNumber, EEEData.ComplianceYear, EEEData.SubmittedDate, EEEData.Quarter, 
        EEEData.Cat1 as Cat1B2C, 
        EEEData.Cat2 as Cat2B2C, 		
        EEEData.Cat3 as Cat3B2C, 
        EEEData.Cat4 as Cat4B2C, 
        EEEData.Cat5 as Cat5B2C, 		
        EEEData.Cat6 as Cat6B2C, 
        EEEData.Cat7 as Cat7B2C, 
        EEEData.Cat8 as Cat8B2C, 
        EEEData.Cat9 as Cat9B2C, 
        EEEData.Cat10 as Cat10B2C, 
        EEEData.Cat11 as Cat11B2C, 
        EEEData.Cat12 as Cat12B2C,
        EEEData.cat13 as Cat13B2C, 
        EEEData.Cat14 as Cat14B2C
     from #ProducerEEEData EEEData
     where EEEData.ObligationType = 'B2C'
) as B2C on (Producers.PRN= B2C.PRN AND Producers.ApprovalNumber = B2C.ApprovalNumber 
                and Producers.ComplianceYear = B2C.ComplianceYear and Producers.Quarter = B2C.Quarter 
                and Producers.SubmittedDate = B2C.SubmittedDate)

--B2B 
LEFT JOIN (
SELECT DISTINCT EEEData.Id, EEEData.PRN, EEEData.ApprovalNumber, EEEData.ComplianceYear, EEEData.Quarter,EEEDAta.SubmittedDate,
        EEEData.Cat1 as Cat1B2B, 
        EEEData.Cat2 as Cat2B2B, 		
        EEEData.Cat3 as Cat3B2B, 
        EEEData.Cat4 as Cat4B2B, 
        EEEData.Cat5 as Cat5B2B, 		
        EEEData.Cat6 as Cat6B2B, 
        EEEData.Cat7 as Cat7B2B, 
        EEEData.Cat8 as Cat8B2B, 
        EEEData.Cat9 as Cat9B2B, 
        EEEData.Cat10 as Cat10B2B, 
        EEEData.Cat11 as Cat11B2B, 
        EEEData.Cat12 as Cat12B2B,
        EEEData.cat13 as Cat13B2B, 
        EEEData.Cat14 as Cat14B2B
     from #ProducerEEEData EEEData
     where EEEData.ObligationType = 'B2B'
) as B2B on (Producers.Id = B2B.Id and Producers.PRN= B2B.PRN AND Producers.ApprovalNumber = B2B.ApprovalNumber 
            and Producers.ComplianceYear = B2B.ComplianceYear and Producers.Quarter = B2B.Quarter  and Producers.SubmittedDate = B2B.SubmittedDate)

INNER JOIN(			 
            SELECT PE.ApprovalNumber, PE.ComplianceYear, PE.Quarter,PE.SubmittedDate,				
            DENSE_RANK() over
            (
            Partition by PE.ApprovalNumber, PE.ComplianceYear, PE.Quarter
            order by ComplianceYear desc, SubmittedDate desc
            ) as SubmissionRecordRank
            from #ProducerEEEData PE			
          ) as LatestRecord on (Producers.ApprovalNumber = LatestRecord.ApprovalNumber 
            and Producers.ComplianceYear = LatestRecord.ComplianceYear and Producers.Quarter = LatestRecord.Quarter
            and Producers.SubmittedDate = LatestRecord.SubmittedDate
            )
Order by ComplianceYear desc, SubmittedDate desc
END