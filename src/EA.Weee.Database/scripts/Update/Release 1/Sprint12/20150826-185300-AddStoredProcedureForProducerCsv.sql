SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Graham Alexaner-Thomson
-- Create date: 26 AUG 2015
-- Description:	Returns data about all producers currently registered
--				with the specified organisation in the specified year.
--				This data is suitable for populating the CSV file
--				which may be downloaded by users associated with the
--				organisation.
--
--				Note: The @OrganisationId parameter is actually expecting
--				the ID of a scheme.
-- =============================================
CREATE PROCEDURE Producer.spgCSVDataByOrganisationIdAndComplianceYear
	@OrganisationId UNIQUEIDENTIFIER,
	@ComplianceYear INT
AS
BEGIN

	SET NOCOUNT ON;

	SELECT
		COALESCE(PBC.Name, PBP.Name, '') AS 'OrganisationName',
		P.TradingName AS 'TradingName',
		P.RegistrationNumber AS 'RegistrationNumber',
		COALESCE(PBC.CompanyNumber, '') AS 'CompanyNumber',
		CASE P.ChargeBandType
			WHEN 0 THEN 'A'
			WHEN 1 THEN 'B'
			WHEN 2 THEN 'C'
			WHEN 3 THEN 'D'
			WHEN 4 THEN 'E'
			ELSE ''
		END AS 'ChargeBand',
		P_First.UpdatedDate AS 'DateRegistered',
		P.UpdatedDate AS 'DateAmended',
		CASE WHEN P.AuthorisedRepresentativeId IS NULL
			THEN 'No'
			ELSE 'Yes'
		END AS 'AuthorisedRepresentative',
		COALESCE(PAR.OverseasProducerName, '') AS 'OverseasProducer'
	FROM
		Producer.Producer P
	LEFT JOIN
		Producer.Business PB
			ON P.ProducerBusinessId = PB.Id
	LEFT JOIN
		Producer.Company PBC
			ON PB.CompanyId = PBC.Id
	LEFT JOIN
		Producer.Partnership PBP
			ON PB.PartnershipId = PBP.Id
	LEFT JOIN
		Producer.AuthorisedRepresentative PAR
			ON P.AuthorisedRepresentativeId = PAR.Id
	INNER JOIN
		PCS.Scheme S
			ON P.SchemeId = S.Id
	INNER JOIN
		PCS.MemberUpload MU
			ON P.MemberUploadId = MU.Id
	INNER JOIN
		(
			SELECT
				MU.ComplianceYear,
				P.RegistrationNumber,
				P.UpdatedDate,
				ROW_NUMBER() OVER
				(
					PARTITION BY
						MU.ComplianceYear,
						P.RegistrationNumber
					ORDER BY P.UpdatedDate
				) AS RowNumber
			FROM
				Producer.Producer P
			INNER JOIN
				PCS.MemberUpload MU
					ON P.MemberUploadId = MU.Id
			WHERE
				MU.IsSubmitted = 1
		) P_First
			ON P.RegistrationNumber = P_First.RegistrationNumber
			AND MU.ComplianceYear = P_First.ComplianceYear
			AND P_First.RowNumber = 1
	WHERE
		S.OrganisationId = @OrganisationId
	AND
		MU.ComplianceYear = @ComplianceYear
	AND
		P.IsCurrentForComplianceYear = 1
	ORDER BY
		COALESCE(PBC.Name, PBP.Name, '')
END
GO
