SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Priety Mahajan
-- Modified date: 2016 Feb 04
-- Description:	This stored procedure is used to provide the EEE data history for producer.
-- =============================================
ALTER PROCEDURE [Producer].[spgProducerEeeHistoryCsvDataByPRN]
    @PRN  NVARCHAR(50)
AS
BEGIN
SET NOCOUNT ON;
DECLARE @ProducerEEEHistoryReturns TABLE
	(	
		PRN NVARCHAR(50),
		Rownumber INT,
		[LatestData] NVARCHAR(10),							
		[ApprovalNumber] NVARCHAR(50),
		[SchemeName] NVARCHAR(50),
		[ComplianceYear] INT,
		[Quarter]		INT,
		[SubmittedDate] datetime,
		[Cat1B2C] decimal(28,3) NULL,	[Cat2B2C] decimal(28,3) NULL,	[Cat3B2C] decimal(28,3) NULL,	[Cat4B2C] decimal(28,3) NULL,	
		[Cat5B2C] decimal(28,3) NULL,	[Cat6B2C] decimal(28,3) NULL,	[Cat7B2C] decimal(28,3) NULL,	[Cat8B2C] decimal(28,3) NULL,	
		[Cat9B2C] decimal(28,3) NULL,	[Cat10B2C] decimal(28,3) NULL,[Cat11B2C] decimal(28,3) NULL,[Cat12B2C] decimal(28,3) NULL,	
		[Cat13B2C] decimal(28,3) NULL,[Cat14B2C] decimal(28,3) NULL,	
		[Cat1B2B] decimal(28,3) NULL,	[Cat2B2B] decimal(28,3) NULL,	[Cat3B2B] decimal(28,3) NULL,	[Cat4B2B] decimal(28,3) NULL,	
		[Cat5B2B] decimal(28,3) NULL,	[Cat6B2B] decimal(28,3) NULL,	[Cat7B2B] decimal(28,3) NULL,	[Cat8B2B] decimal(28,3) NULL,	
		[Cat9B2B] decimal(28,3) NULL,	[Cat10B2B] decimal(28,3) NULL,[Cat11B2B] decimal(28,3) NULL,[Cat12B2B] decimal(28,3) NULL,	
		[Cat13B2B] decimal(28,3) NULL,[Cat14B2B] decimal(28,3) NULL	
	)

SELECT  EEORVA.EeeOutputReturnVersionId as 'Id',	
		DENSE_RANK() over
					(
					partition by (EEOA.Id)
					Order by DR.ComplianceYear, DRV.SubmittedDate 
					) as RowNumber, 		
		
        RP.ProducerRegistrationNumber as 'PRN',
        S.SchemeName,
        S.ApprovalNumber,		
        DR.ComplianceYear,		
        DRV.SubmittedDate,
        DR.Quarter,	
        EEOA.ObligationType,	
        EEOA.WeeeCategory,
        EEOA.Tonnage
INTO #EEETable
	FROM [PCS].[EeeOutputAmount] EEOA
   
    inner JOIN [PCS].[EeeOutputReturnVersionAmount] EEORVA
        on EEOA.Id = EEORVA.EeeOuputAmountId
	
	inner join [PCS].EeeOutputReturnVersion EORV on EEORVA.EeeOutputReturnVersionId = EORV.Id

	inner join	[PCS].DataReturnVersion DRV on EORV.Id = DRV.EeeOutputReturnVersionId

	inner JOIN [PCS].DataReturn DR
        on DRV.DataReturnId = DR.Id
     

    INNER JOIN [Producer].RegisteredProducer RP
        on EEOA.RegisteredProducerId = RP.Id
     
	 INNER JOIN [PCS].Scheme S
        on RP.SchemeId = S.Id	
    where 
        RP.ProducerRegistrationNumber =  @PRN and DRV.SubmittedDate is not null --filter out not submitted returns
	order by RowNumber, DR.ComplianceYear desc, DRV.SubmittedDate desc

SELECT EeeData.Id,EeeData.RowNumber,
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

INSERT INTO @ProducerEEEHistoryReturns
SELECT DISTINCT Producers.PRN, Producers.RowNumber, 
		case when Producers.SubmittedDate = 
		(select max(SubmittedDate) from #ProducerEEEData P2 where 	
				Producers.PRN= P2.PRN AND Producers.ApprovalNumber = P2.ApprovalNumber and Producers.RowNumber = P2.RowNumber
                and Producers.ComplianceYear = P2.ComplianceYear and Producers.Quarter = P2.Quarter ) then 'Yes' else 'No' end as 'LatestData',
	    
		Producers.ApprovalNumber, Producers.SchemeName, Producers.ComplianceYear, 		
                        Producers.Quarter, Producers.SubmittedDate,                           		 
                        B2C.Cat1B2C,B2C.Cat2B2C,B2C.Cat3B2C, B2C.Cat4B2C, B2C.Cat5B2C, B2C.Cat6B2C, B2C.Cat7B2C, B2C.Cat8B2C,B2C.Cat9B2C,
                        B2C.Cat10B2C,B2C.Cat11B2C, B2C.Cat12B2C, B2C.Cat13B2C,B2C.Cat14B2C, 
                        B2B.Cat1B2B,B2B.Cat2B2B,B2B.Cat3B2B, B2B.Cat4B2B, B2B.Cat5B2B, B2B.Cat6B2B, B2B.Cat7B2B, B2B.Cat8B2B,B2B.Cat9B2B,
						B2B.Cat10B2B,B2B.Cat11B2B, B2B.Cat12B2B, B2B.Cat13B2B,B2B.Cat14B2B
        FROM (
                    SELECT PE.Id, Pe.RowNumber, 						
                    Pe.PRN, PE.ApprovalNumber, PE.SchemeName, PE.ComplianceYear, PE.SubmittedDate, PE.Quarter
                    FROM  #ProducerEEEData PE		
                    where PE.RowNumber = 1		
					group by PE.Id, Pe.RowNumber,PE.PRN, PE.ApprovalNumber, PE.SchemeName, PE.ComplianceYear, PE.SubmittedDate, PE.Quarter
             ) AS Producers 


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
) as B2C on (Producers.Id = B2C.Id and 
Producers.PRN= B2C.PRN AND Producers.ApprovalNumber = B2C.ApprovalNumber 
                and Producers.ComplianceYear = B2C.ComplianceYear and Producers.Quarter = B2C.Quarter 
                and Producers.SubmittedDate = B2C.SubmittedDate
				)

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
) as B2B on (Producers.Id = B2B.Id and 
Producers.PRN= B2B.PRN AND Producers.ApprovalNumber = B2B.ApprovalNumber 
            and Producers.ComplianceYear = B2B.ComplianceYear and Producers.Quarter = B2B.Quarter  
			and Producers.SubmittedDate = B2B.SubmittedDate
			)

Order by ComplianceYear desc, SubmittedDate desc

DECLARE @ProducerNotInSchemeReturns TABLE
	(
		DataReturnVersionId UNIQUEIDENTIFIER NOT NULL,
		EeeOutputReturnVersionId UNIQUEIDENTIFIER NULL,
		[ApprovalNumber]	NVARCHAR(50),		
		[ComplianceYear] INT,
		[Quarter]		INT,
		[SubmittedDate] datetime
	)
INSERT INTO @ProducerNotInSchemeReturns
SELECT
 DRV.Id,
 DRV.EeeOutputReturnVersionId,
 S.ApprovalNumber,
 DR.ComplianceYear,
 DR.Quarter,
 DRV.SubmittedDate

FROM 
 PCS.DataReturnVersion DRV
 INNER JOIN PCS.DataReturn DR ON DRV.DataReturnId = DR.Id  
 INNER JOIN PCS.Scheme S on DR.SchemeId = S.Id
WHERE 

 DRV.SubmittedDate IS NOT NULL 
AND
 -- Only interested in returns belonging to schemes where the producer is registered
 DR.SchemeId IN (SELECT RP.SchemeId
                 FROM Producer.RegisteredProducer RP
                 WHERE 
                 RP.ProducerRegistrationNumber = @prn
                 AND RP.CurrentSubmissionId IS NOT NULL
				 AND DR.ComplianceYear = RP.ComplianceYear )
AND (DRV.EeeOutputReturnVersionId IS NULL 
		OR 
		(DRV.Id not in (
			select DRV.Id from PCS.DataReturnVersion DRV
			inner join PCS.EeeOutputReturnVersionAmount EORVA on DRV.EeeOutputReturnVersionId = EORVA.EeeOutputReturnVersionId
			inner join PCS.EeeOutputAmount EOA on EORVA.EeeOuputAmountId = EOA.Id
			inner join Producer.RegisteredProducer RP on EOA.RegisteredProducerId = RP.Id
			where RP.ProducerRegistrationNumber =@PRN and RP.SchemeId = DR.SchemeId and RP.ComplianceYear = DR.ComplianceYear)
		)
	)

--Result set 1 for EEE history for a producers
Select * from @ProducerEEEHistoryReturns P
order by ComplianceYear, SubmittedDate

--Result set 2 for EEE history where producer was not included in the data returns
Select * from @ProducerNotInSchemeReturns
END