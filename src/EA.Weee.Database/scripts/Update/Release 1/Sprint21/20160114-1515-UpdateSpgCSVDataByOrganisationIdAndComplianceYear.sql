/****** Object:  StoredProcedure [Producer].[spgCSVDataByOrganisationIdAndComplianceYear]    Script Date: 15/12/2015 09:39:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*
 * Returns data about all producers currently registered
 * with the specified organisation in the specified year.
 * This data is suitable for populating the CSV file
 * which may be downloaded by users associated with the
 * organisation.
 */
ALTER PROCEDURE [Producer].[spgCSVDataByOrganisationIdAndComplianceYear]
	@OrganisationId UNIQUEIDENTIFIER,
	@ComplianceYear INT
AS
BEGIN

	SET NOCOUNT ON;

	SELECT
		COALESCE(PBC.Name, PBP.Name, '') AS 'OrganisationName',
		
		PS.TradingName AS 'TradingName',
		
		RP.ProducerRegistrationNumber AS 'RegistrationNumber',
		
		COALESCE(PBC.CompanyNumber, '') AS 'CompanyNumber',
		
		CASE CBA.[ChargeBand]
			WHEN 0 THEN 'A'
			WHEN 1 THEN 'B'
			WHEN 2 THEN 'C'
			WHEN 3 THEN 'D'
			WHEN 4 THEN 'E'
			ELSE ''
		END AS 'ChargeBand',
		
		PS.[ObligationType],

		P_First.SubmittedDate AS 'DateRegistered',
		
		MU.SubmittedDate AS 'DateAmended',
		
		CASE WHEN PS.AuthorisedRepresentativeId IS NOT NULL AND PAR.OverseasContactId IS NOT NULL AND PC.Email IS NOT NULL AND PC.Email != ''
			THEN 'Yes'
			ELSE 'No'
		END AS 'AuthorisedRepresentative',
		
		COALESCE(PAR.OverseasProducerName, '') AS 'OverseasProducer'
	FROM
		Producer.RegisteredProducer RP
	INNER JOIN
		PCS.[Scheme] S
			ON RP.[SchemeId] = S.[Id]
	INNER JOIN
		Producer.ProducerSubmission PS
			ON RP.[CurrentSubmissionId] = PS.[Id]
	LEFT JOIN
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
	LEFT JOIN
		Producer.AuthorisedRepresentative PAR
			ON PS.AuthorisedRepresentativeId = PAR.Id
	LEFT JOIN Producer.Contact PC
			ON PAR.OverseasContactId = PC.Id
	INNER JOIN
		PCS.MemberUpload MU
			ON PS.MemberUploadId = MU.Id
	INNER JOIN
		(
			SELECT
				MU.ComplianceYear,
				PS.RegisteredProducerId,
				MU.SubmittedDate,
				ROW_NUMBER() OVER
				(
					PARTITION BY
						PS.RegisteredProducerId
					ORDER BY PS.UpdatedDate
				) AS RowNumber
			FROM
				Producer.ProducerSubmission PS
			INNER JOIN
				PCS.MemberUpload MU
					ON PS.MemberUploadId = MU.Id
			INNER JOIN 
				Producer.RegisteredProducer RP
					ON PS.RegisteredProducerId = RP.Id
			WHERE
				MU.IsSubmitted = 1
			AND
				MU.OrganisationId = @OrganisationId
			AND
				RP.Removed = 0
		) P_First
			ON PS.RegisteredProducerId = P_First.RegisteredProducerId
			AND P_First.RowNumber = 1
	WHERE
		S.[OrganisationId] = @OrganisationId
	AND
		RP.[ComplianceYear] = @ComplianceYear
	AND
		RP.Removed = 0
	ORDER BY
		COALESCE(PBC.Name, PBP.Name, '')
END