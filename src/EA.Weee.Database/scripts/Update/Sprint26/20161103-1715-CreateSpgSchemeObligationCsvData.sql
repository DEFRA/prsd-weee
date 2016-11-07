SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Creation date: 2016 Nov 03
-- Description:	This stored procedure is used to provide the data for the admin report of data
--				for all registered producers B2C EEE data for the previous compliance year
--				(where available).
-- =============================================
CREATE PROCEDURE [Producer].[spgSchemeObligationCsvData]
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
	AND PS.ObligationType NOT IN ('B2B')
ORDER BY ProducerName

END
GO