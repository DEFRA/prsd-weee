/*
 * This script updates the stored procedure 'spgCSVDataBySchemeComplianceYearAndAuthorisedAuthority' to reference
 * the new registered producer and producer submission tables.
 */


GO
/* 
 * Returns PCS charge breakdown currently registered
 * in the specified year and AA.
 * This data is suitable for populating the CSV file
 * which may be downloaded by AA to see the charges for a submission for all schemes in compliance year
 */
ALTER PROCEDURE [Producer].[spgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority]
		@ComplianceYear INT,
		@CompetentAuthorityId uniqueidentifier = null
AS
BEGIN

	SET NOCOUNT ON

	SELECT
		S.SchemeName,
		
		@ComplianceYear as 'ComplianceYear',

		CASE 
			WHEN PBC.Name IS NULL THEN PBP.Name
			ELSE PBC.NAME
		END AS 'ProducerName',

		RP.ProducerRegistrationNumber AS 'PRN',
	 
		MU.[Date] AS 'SubmissionDate',

		PS.ChargeThisUpdate as 'ChargeValue',

		CASE CBA.ChargeBand
			WHEN 0 THEN 'A'
			WHEN 1 THEN 'B'
			WHEN 2 THEN 'C'
			WHEN 3 THEN 'D'
			WHEN 4 THEN 'E'
			ELSE ''
		END AS 'ChargeBandType'

	FROM
		[Producer].[ProducerSubmission] PS
	INNER JOIN
		[Producer].[RegisteredProducer] RP
			ON PS.[RegisteredProducerId] = RP.[Id]
	INNER JOIN
		[PCS].[MemberUpload] MU
			ON PS.MemberUploadId = MU.Id
	INNER JOIN
		[PCS].[Scheme] S
			ON MU.SchemeId = S.Id
	INNER JOIN
		[Lookup].[ChargeBandAmount] CBA
			ON PS.[ChargeBandAmountId] = CBA.[Id]
	LEFT JOIN
		Producer.Business PB
			ON PS.ProducerBusinessId = PB.Id
	LEFT JOIN
		Producer.Company PBC
			ON PB.CompanyId = PBC.Id
	LEFT JOIN
		Producer.Partnership PBP
			ON PB.PartnershipId = PBP.Id
	WHERE
		MU.IsSubmitted = 1
	AND
		RP.ComplianceYear = @ComplianceYear
	AND
		(@CompetentAuthorityId IS NULL OR S.CompetentAuthorityId = @CompetentAuthorityId)
	AND
		PS.ChargeThisUpdate > 0
	ORDER BY
		S.SchemeName,
		MU.[Date]

END
GO