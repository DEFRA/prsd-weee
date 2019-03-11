  INSERT INTO Lookup.ChargeBandAmount
  (Id, Amount, ChargeBand)
  VALUES('DC8545CD-0C38-40B8-87CF-30AFF1990742', 750, 5)
  GO
  INSERT INTO Lookup.ChargeBandAmount
  (Id, Amount, ChargeBand)
  VALUES('A8B0DB11-4158-4607-BED4-D476CCCCEB06', 100,6)
  GO
   INSERT INTO Lookup.ChargeBandAmount
  (Id, Amount, ChargeBand)
  VALUES('D8B5CD20-84AE-4495-A012-3994D8DB921C',100,7)
  GO
  INSERT INTO Lookup.ChargeBandAmount
  (Id, Amount, ChargeBand)
  VALUES('7D196466-4DE5-47B4-9D27-1CD3D09571C7',375,8)
  GO

  /****** Object:  StoredProcedure [Charging].[SpgInvoiceRunChargeBreakdown]    Script Date: 10/03/2019 17:14:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [Charging].[SpgInvoiceRunChargeBreakdown]
@InvoiceRunId UNIQUEIDENTIFIER
AS
BEGIN

SET NOCOUNT ON

SELECT
  S.[SchemeName],
  M.[ComplianceYear],
  M.[SubmittedDate] as 'SubmissionDate',
  CASE 
    WHEN PBC.Name IS NULL THEN PBP.Name
    ELSE PBC.NAME
  END AS 'ProducerName',
  RP.[ProducerRegistrationNumber] AS 'PRN',
  P.[ChargeThisUpdate] as 'ChargeValue',
  CASE C.ChargeBand
    WHEN 0 THEN 'A'
    WHEN 1 THEN 'B'
    WHEN 2 THEN 'C'
    WHEN 3 THEN 'D'
    WHEN 4 THEN 'E'
	WHEN 5 THEN 'A2'
	WHEN 6 THEN 'C2'
	WHEN 7 THEN 'D2'
	WHEN 8 THEN 'D3'
    ELSE ''
  END AS 'ChargeBandType'
FROM [Producer].[ProducerSubmission] P
  JOIN [PCS].[MemberUpload] M ON P.MemberUploadId = M.Id
  JOIN [Lookup].[ChargeBandAmount] C ON P.ChargeBandAmountId = C.Id
  JOIN [Producer].[Business] PB ON P.ProducerBusinessId = PB.Id
  LEFT JOIN [Producer].[Partnership] PBP ON PB.PartnershipId = PBP.Id
  LEFT JOIN [Producer].[Company] PBC ON PB.CompanyId = PBC.Id
  JOIN [Producer].[RegisteredProducer] RP ON P.RegisteredProducerId = RP.Id
  JOIN [PCS].[Scheme] S ON RP.SchemeId = S.Id
WHERE
    M.InvoiceRunId = @InvoiceRunId
  AND
    P.Invoiced = 1
  AND
    P.ChargeThisUpdate > 0
ORDER BY 
  S.[SchemeName] ASC,
  M.[ComplianceYear] DESC,
  M.[SubmittedDate] ASC

END
GO

/****** Object:  StoredProcedure [Producer].[spgCSVDataByOrganisationIdAndComplianceYear]    Script Date: 11/03/2019 10:25:56 ******/
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
			WHEN 5 THEN 'A2'
			WHEN 6 THEN 'C2'
			WHEN 7 THEN 'D2'
			WHEN 8 THEN 'D3'
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
GO

