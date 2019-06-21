/****** Object:  StoredProcedure [Producer].[spgSchemeObligationCsvData]    Script Date: 08/11/2016 15:55:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Creation date: 2016 Nov 03
-- Description:	This stored procedure is used to provide the data for the admin report of data
--				for all registered producers B2C EEE data for the previous compliance year
--				(where available).
-- =============================================
ALTER PROCEDURE [Producer].[spgSchemeObligationCsvData]
	@ComplianceYear INT
AS
BEGIN

SELECT 
	S.ApprovalNumber,
	S.SchemeName,
	RP.ProducerRegistrationNumber as 'PRN',
	COALESCE(PBC.Name, PBP.Name, '') AS 'ProducerName',
	PrevOT.ObligationType AS 'ObligationTypeForPreviousYear',
	PS.ObligationType AS 'ObligationTypeForSelectedYear',
	Cat1B2CTotal, Cat2B2CTotal, Cat3B2CTotal, Cat4B2CTotal, Cat5B2CTotal, Cat6B2CTotal, Cat7B2CTotal,
	Cat8B2CTotal, Cat9B2CTotal, Cat10B2CTotal, Cat11B2CTotal, Cat12B2CTotal, Cat13B2CTotal, Cat14B2CTotal
FROM [Producer].[RegisteredProducer] RP
	INNER JOIN [PCS].Scheme S ON RP.SchemeId = S.Id
	INNER JOIN [Producer].[ProducerSubmission] PS ON RP.[CurrentSubmissionId] = PS.[Id]
	INNER JOIN [Producer].[Business] PB ON PS.ProducerBusinessId = PB.Id
		LEFT OUTER JOIN [Producer].[Company] PBC ON PB.CompanyId = PBC.Id
		LEFT OUTER JOIN [Producer].[Partnership] PBP ON PB.PartnershipId = PBP.Id
	LEFT OUTER JOIN (
		--This query gets obligation types for producers in the previous year
		SELECT PrevRP.ProducerRegistrationNumber,
			PrevPS.ObligationType
		FROM [Producer].[RegisteredProducer] PrevRP
			INNER JOIN [Producer].[ProducerSubmission] PrevPS ON PrevRP.[CurrentSubmissionId] = PrevPS.[Id]
		WHERE PrevRP.ComplianceYear = @ComplianceYear - 1
			AND PrevRP.Removed = 0
	) AS PrevOT ON RP.ProducerRegistrationNumber = PrevOT.ProducerRegistrationNumber
	LEFT OUTER JOIN (
		--This query gets data from the nested query and pivots it by Category
		SELECT
			ProducerRegistrationNumber,
			[1] AS Cat1B2CTotal, [2] AS Cat2B2CTotal, [3] AS Cat3B2CTotal, [4] AS Cat4B2CTotal, [5] AS Cat5B2CTotal,
			[6] AS Cat6B2CTotal, [7] AS Cat7B2CTotal, [8] AS Cat8B2CTotal, [9] AS Cat9B2CTotal, [10] AS Cat10B2CTotal,
			[11] AS Cat11B2CTotal, [12] AS Cat12B2CTotal, [13] AS Cat13B2CTotal, [14] AS Cat14B2CTotal
		FROM
		(
			--This nested query returns all B2C EEE for the previous compliance year by producer
			SELECT
				PrevRP.ProducerRegistrationNumber,
				PrevEOA.WeeeCategory,
				SUM(PrevEOA.Tonnage) AS 'CategoryTonnageTotal'
			FROM [PCS].[DataReturn] PrevDR
				INNER JOIN [PCS].DataReturnVersion PrevDRV ON PrevDR.CurrentDataReturnVersionId = PrevDRV.Id
				INNER JOIN [PCS].EeeOutputReturnVersionAmount PrevEORVA ON PrevDRV.EeeOutputReturnVersionId = PrevEORVA.EeeOutputReturnVersionId
				INNER JOIN [PCS].EeeOutputAmount PrevEOA ON PrevEORVA.EeeOuputAmountId = PrevEOA.Id
				INNER JOIN [Producer].[RegisteredProducer] PrevRP ON PrevEOA.RegisteredProducerId = PrevRP.Id
			WHERE PrevDR.ComplianceYear = @ComplianceYear - 1
				AND PrevEOA.ObligationType = 'B2C'
				AND PrevRP.Removed = 0
			GROUP BY PrevEOA.RegisteredProducerId, PrevEOA.WeeeCategory, PrevRP.ProducerRegistrationNumber
		) AS PrevEee
		PIVOT
		(
			SUM(CategoryTonnageTotal)
			FOR PrevEee.WeeeCategory IN ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12],[13],[14]) 
		) AS PrevEeePivoted
	) AS PrevEeeByCat ON RP.ProducerRegistrationNumber = PrevEeeByCat.ProducerRegistrationNumber
WHERE RP.ComplianceYear = @ComplianceYear
	AND	RP.Removed = 0
	-- Do not include producers that were not or are not B2C
	AND NOT (PS.ObligationType = 'B2B' AND PrevOT.ObligationType = 'B2B')
	-- Do not include rows with previous year B2B obligation type that have B2C data included
	-- (Can only happen when producer was registered with 2 schemes in prev year, one B2B, one B2C.
	-- The Eee data will still appear in the report for the B2C scheme of that year)
	AND NOT (PrevOT.ObligationType = 'B2B' AND (
		Cat1B2CTotal IS NOT NULL OR 
		Cat2B2CTotal IS NOT NULL OR 
		Cat3B2CTotal IS NOT NULL OR 
		Cat4B2CTotal IS NOT NULL OR 
		Cat5B2CTotal IS NOT NULL OR
		Cat6B2CTotal IS NOT NULL OR
		Cat7B2CTotal IS NOT NULL OR
		Cat8B2CTotal IS NOT NULL OR 
		Cat9B2CTotal IS NOT NULL OR 
		Cat10B2CTotal IS NOT NULL OR 
		Cat11B2CTotal IS NOT NULL OR  
		Cat12B2CTotal IS NOT NULL OR
		Cat13B2CTotal IS NOT NULL OR 
		Cat14B2CTotal IS NOT NULL
	))
ORDER BY ProducerName

END
GO