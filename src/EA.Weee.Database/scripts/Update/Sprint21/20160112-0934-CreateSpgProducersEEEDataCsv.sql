
/****** Object:  StoredProcedure [Producer].[spgProducerEEECSVDataByComplianceYearAndObligationType]    Script Date: 11/01/2016 16:27:24 ******/
GO
-- =============================================
-- Author:		Priety Mahajan
-- Create date: 2016 Jan 11
-- Description:	This stored procedure is used to provide the data for the admin report of EEE data
--				for Producer.
-- =============================================
CREATE PROCEDURE [Producer].[spgProducerEEECSVDataByComplianceYearAndObligationType]
	@ComplianceYear INT,
	@ObligationType NVARCHAR(4)
AS
BEGIN

SELECT 
		RP.ProducerRegistrationNumber as 'PRN',
		RP.SchemeId,
		S.SchemeName,
		COALESCE(PBC.Name, PBP.Name, '') AS 'ProducerName',
	    COALESCE(ROC_A_C.Name, PPOB_A_C.Name, '') AS 'ProducerCountry',
		DR.Quarter,
		EEOA.WeeeCategory,
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

	INNER JOIN [PCS].Scheme S
		on RP.SchemeId = S.Id
	INNER JOIN
			[Producer].[ProducerSubmission] PS
				ON RP.[CurrentSubmissionId] = PS.[Id]
	LEFT JOIN
			  [Producer].[Business] PB
					ON PS.ProducerBusinessId = PB.Id
			  LEFT JOIN
					[Producer].[Contact] CFNC
					INNER JOIN
						  [Producer].[Address] CFNC_A
						  INNER JOIN
								[Lookup].[Country] CFNC_A_C
									  ON CFNC_A.CountryId = CFNC_A_C.Id   
								ON CFNC.AddressId = CFNC_A.Id
						  ON PB.CorrespondentForNoticesContactId = CFNC.Id
			  LEFT JOIN
					[Producer].[Company] PBC
						  ON PB.CompanyId = PBC.Id
					LEFT JOIN
						  [Producer].[Contact] ROC
						  INNER JOIN
								[Producer].[Address] ROC_A
								INNER JOIN
									  [Lookup].[Country] ROC_A_C
											ON ROC_A.CountryId = ROC_A_C.Id     
									  ON ROC.AddressId = ROC_A.Id
								ON PBC.RegisteredOfficeContactId = ROC.Id
			  LEFT JOIN
					[Producer].[Partnership] PBP
						  ON PB.PartnershipId = PBP.Id
					LEFT JOIN
						  [Producer].[Contact] PPOB
						  INNER JOIN
								[Producer].[Address] PPOB_A
								INNER JOIN
									  [Lookup].[Country] PPOB_A_C
											ON PPOB_A.CountryId = PPOB_A_C.Id   
									  ON PPOB.AddressId = PPOB_A.Id
								ON PBP.PrincipalPlaceOfBusinessId = PPOB.Id
	where DR.ComplianceYear = @ComplianceYear
		AND
		RP.IsAligned = 1
		AND
		EEOA.ObligationType = @ObligationType


SELECT EeeData.PRN, EeeData.ProducerName, EeeData.ProducerCountry, EeeData.SchemeId, EeeData.SchemeName, EeeData.Quarter,
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


SELECT Producers.PRN, Producers.ProducerName, Producers.ProducerCountry, Producers.SchemeName, 
		COALESCE(Totals.TotalTonnage,0) as 'TotalTonnage',			 
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
	SELECT PE.PRN,  PE.SchemeId,PE.SchemeName,  PE.ProducerName,  PE.ProducerCountry
	FROM  #ProducerEEEData PE	
	GROUP BY  PE.PRN,  PE.SchemeId, PE.SchemeName,  PE.ProducerName,  PE.ProducerCountry
) AS Producers 

INNER JOIN (
SELECT E.PRN, E.SchemeId, SUM(COALESCE(E.Tonnage,0)) AS 'TotalTonnage'
	FROM #EEETable E
	GROUP BY E.PRN, 
			E.SchemeId
) AS Totals 
ON (Producers.PRN = Totals.PRN AND Producers.SchemeId = Totals.SchemeId)

--1st Quarter
LEFT JOIN (
select EEEData.PRN, EEEData.SchemeId, 
		EEEData.Cat1 as Cat1Q1, 
		EEEData.Cat2 as Cat2Q1, 		
		EEEData.Cat3 as Cat3Q1, 
		EEEData.Cat4 as Cat4Q1, 
		EEEData.Cat5 as Cat5Q1, 		
		EEEData.Cat6 as Cat6Q1, 
		EEEData.Cat7 as Cat7Q1, 
		EEEData.Cat8 as Cat8Q1, 
		EEEData.Cat9 as Cat9Q1, 
		EEEData.Cat10 as Cat10Q1, 
		EEEData.Cat11 as Cat11Q1, 
		EEEData.Cat12 as Cat12Q1,
		EEEData.cat13 as Cat13Q1, 
		EEEData.Cat14 as Cat14Q1
	 from #ProducerEEEData EEEData
	 where EEEData.Quarter = 1
) as Q1 on (Producers.PRN= Q1.PRN AND Producers.SchemeId = Q1.SchemeId)

--2nd Quarter
LEFT JOIN (
select EEEData.PRN, EEEData.SchemeId,		
		EEEData.Cat1 as Cat1Q2, 
		EEEData.Cat2 as Cat2Q2, 		
		EEEData.Cat3 as Cat3Q2, 
		EEEData.Cat4 as Cat4Q2, 
		EEEData.Cat5 as Cat5Q2, 		
		EEEData.Cat6 as Cat6Q2, 
		EEEData.Cat7 as Cat7Q2, 
		EEEData.Cat8 as Cat8Q2, 
		EEEData.Cat9 as Cat9Q2, 
		EEEData.Cat10 as Cat10Q2, 
		EEEData.Cat11 as Cat11Q2, 
		EEEData.Cat12 as Cat12Q2,
		EEEData.cat13 as Cat13Q2, 
		EEEData.Cat14 as Cat14Q2
	 from #ProducerEEEData EEEData
	 where EEEData.Quarter = 2
) as Q2 on (Producers.PRN= Q2.PRN AND Producers.SchemeId = Q2.SchemeId)

--3rd Quarter
LEFT JOIN (
select EEEData.PRN, EEEData.SchemeId, 
		EEEData.Cat1 as Cat1Q3, 
		EEEData.Cat2 as Cat2Q3, 		
		EEEData.Cat3 as Cat3Q3, 
		EEEData.Cat4 as Cat4Q3, 
		EEEData.Cat5 as Cat5Q3, 		
		EEEData.Cat6 as Cat6Q3, 
		EEEData.Cat7 as Cat7Q3, 
		EEEData.Cat8 as Cat8Q3, 
		EEEData.Cat9 as Cat9Q3, 
		EEEData.Cat10 as Cat10Q3, 
		EEEData.Cat11 as Cat11Q3, 
		EEEData.Cat12 as Cat12Q3,
		EEEData.cat13 as Cat13Q3, 
		EEEData.Cat14 as Cat14Q3
	 from #ProducerEEEData EEEData
	 where EEEData.Quarter = 3
) as Q3 on (Producers.PRN= Q3.PRN AND Producers.SchemeId = Q3.SchemeId)

--4th Quarter
LEFT JOIN (
select EEEData.PRN, EEEData.SchemeId, 
		EEEData.Cat1 as Cat1Q4, 
		EEEData.Cat2 as Cat2Q4, 		
		EEEData.Cat3 as Cat3Q4, 
		EEEData.Cat4 as Cat4Q4, 
		EEEData.Cat5 as Cat5Q4, 		
		EEEData.Cat6 as Cat6Q4, 
		EEEData.Cat7 as Cat7Q4, 
		EEEData.Cat8 as Cat8Q4, 
		EEEData.Cat9 as Cat9Q4, 
		EEEData.Cat10 as Cat10Q4, 
		EEEData.Cat11 as Cat11Q4, 
		EEEData.Cat12 as Cat12Q4,
		EEEData.cat13 as Cat13Q4, 
		EEEData.Cat14 as Cat14Q4
	 from #ProducerEEEData EEEData
	 where EEEData.Quarter = 4
) as Q4 on (Producers.PRN= Q4.PRN AND Producers.SchemeId = Q4.SchemeId)

END
GO
