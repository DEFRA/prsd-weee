BEGIN TRANSACTION
IF NOT EXISTS (SELECT * FROM Lookup.ChargeBandAmount WHERE Id = 'DC8545CD-0C38-40B8-87CF-30AFF1990742') 
BEGIN
	INSERT INTO Lookup.ChargeBandAmount (Id, Amount, ChargeBand)
	VALUES('DC8545CD-0C38-40B8-87CF-30AFF1990742', 750, 5)
END
GO

IF NOT EXISTS (SELECT * FROM Lookup.ChargeBandAmount WHERE Id = 'A8B0DB11-4158-4607-BED4-D476CCCCEB06') 
BEGIN
	INSERT INTO Lookup.ChargeBandAmount (Id, Amount, ChargeBand)
	VALUES('A8B0DB11-4158-4607-BED4-D476CCCCEB06', 100,6)
END
GO

IF NOT EXISTS (SELECT * FROM Lookup.ChargeBandAmount WHERE Id = 'D8B5CD20-84AE-4495-A012-3994D8DB921C') 
BEGIN
	INSERT INTO Lookup.ChargeBandAmount (Id, Amount, ChargeBand)
	VALUES('D8B5CD20-84AE-4495-A012-3994D8DB921C',100,7)
END
GO

IF NOT EXISTS (SELECT * FROM Lookup.ChargeBandAmount WHERE Id = '7D196466-4DE5-47B4-9D27-1CD3D09571C7') 
BEGIN
	INSERT INTO Lookup.ChargeBandAmount (Id, Amount, ChargeBand)
	VALUES('7D196466-4DE5-47B4-9D27-1CD3D09571C7',375,8)
END
GO

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

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [PCS].[spgSubmissionChangesCsvData]
   @MemberUploadId UNIQUEIDENTIFIER
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE 
    @MemberUploadSubmittedDate DATETIME,
    @SchemeId UNIQUEIDENTIFIER,
    @ComplianceYear INT;

    SELECT
      @MemberUploadSubmittedDate = MU.SubmittedDate,
      @SchemeId = MU.SchemeId,
      @ComplianceYear = MU.ComplianceYear
    FROM PCS.MemberUpload MU
    WHERE 
      MU.Id = @MemberUploadId
    AND
      MU.IsSubmitted = 1;

    -- Non removed producers associated with the scheme and registered
    -- prior to the current submission
    SELECT
       RP.ProducerRegistrationNumber
    INTO #NonRemovedProducers
    FROM Producer.ProducerSubmission PS
    JOIN Producer.RegisteredProducer RP ON PS.RegisteredProducerId = RP.Id
    JOIN PCS.MemberUpload MU ON PS.MemberUploadId = MU.Id
    WHERE
      RP.SchemeId = @SchemeId
    AND
      RP.ComplianceYear = @ComplianceYear
    AND
      RP.Removed = 0
    AND
      MU.IsSubmitted = 1
    AND
      MU.SubmittedDate < @MemberUploadSubmittedDate;

    -- Producers registered prior to the current submission but removed
    -- after the submission date
    SELECT
       RP.ProducerRegistrationNumber
    INTO #RemovedProducers
    FROM Producer.ProducerSubmission PS
    JOIN Producer.RegisteredProducer RP ON PS.RegisteredProducerId = RP.Id
    JOIN PCS.MemberUpload MU ON PS.MemberUploadId = MU.Id
    WHERE
      RP.SchemeId = @SchemeId
    AND
      RP.ComplianceYear = @ComplianceYear
    AND
      RP.Removed = 1
    AND
      MU.IsSubmitted = 1
    AND
      MU.SubmittedDate < @MemberUploadSubmittedDate
    AND
      RP.RemovedDate > @MemberUploadSubmittedDate;

    -- Producers associated with the current submission and
    -- whether they are new or changed
    SELECT
       PS.Id 'ProducerSubmissionId',
       RP.ProducerRegistrationNumber,
       RP.ComplianceYear,
       @MemberUploadSubmittedDate 'SubmittedDate',
       CASE
          WHEN ExistingProducers.ProducerRegistrationNumber IS NULL THEN 'New'
          ELSE 'Amended' 
       END AS 'ChangeType'
    INTO #SubmissionDetails
    FROM Producer.ProducerSubmission PS
    JOIN Producer.RegisteredProducer RP ON PS.RegisteredProducerId = RP.Id
    LEFT OUTER JOIN 
       (SELECT
          RM.ProducerRegistrationNumber
        FROM #RemovedProducers RM
        UNION
        SELECT
          NRM.ProducerRegistrationNumber
        FROM #NonRemovedProducers NRM) AS ExistingProducers ON RP.ProducerRegistrationNumber = ExistingProducers.ProducerRegistrationNumber
    WHERE PS.MemberUploadId = @MemberUploadId;

    -- Previous details of the producers associated with the current submission
    SELECT
      PreviousSubmissions.ProducerSubmissionId,
      PreviousSubmissions.ProducerRegistrationNumber,
      PreviousSubmissions.ComplianceYear,
      PreviousSubmissions.SubmittedDate,
      '' AS 'ChangeType'
    INTO #PreviousSubmissionDetails
    FROM 
      (SELECT 
         ROW_NUMBER() OVER (PARTITION BY RP.ProducerRegistrationNumber ORDER BY MU.SubmittedDate DESC) AS RowNumber,
         PS.Id AS 'ProducerSubmissionId',
         RP.ProducerRegistrationNumber,
         RP.ComplianceYear,
         MU.SubmittedDate
       FROM Producer.ProducerSubmission PS
       JOIN Producer.RegisteredProducer RP ON PS.RegisteredProducerId = RP.Id
       JOIN PCS.MemberUpload MU ON PS.MemberUploadId = MU.Id
       JOIN #SubmissionDetails SD ON RP.ProducerRegistrationNumber = SD.ProducerRegistrationNumber
         AND SD.ChangeType = 'Amended'
       WHERE
          RP.SchemeId = @SchemeId
        AND
          RP.ComplianceYear = @ComplianceYear
        AND
          MU.IsSubmitted = 1
        AND
          MU.SubmittedDate < SD.SubmittedDate) AS PreviousSubmissions
    WHERE PreviousSubmissions.RowNumber = 1;

    SELECT
    AllProducers.ProducerRegistrationNumber,
    AllProducers.ChangeType,
    AllProducers.ComplianceYear,
    AllProducers.SubmittedDate,

    PBC.Name AS 'CompanyName',
    COALESCE(PBC.Name, PBP.Name, '') AS 'ProducerName',

    CASE 
       WHEN PBC.Name IS NULL THEN 'Partnership'
       ELSE 'Registered company'
    END AS 'ProducerType',

    (SELECT STUFF((SELECT DISTINCT '; ' + PP.Name
           FROM [Producer].[Partner] PP
           WHERE PBP.Id = PP.PartnershipId
           FOR XML PATH(''), TYPE)
          .value('.', 'NVARCHAR(MAX)'), 1, 2, '')) AS Partners,
    
    PS.TradingName,

    CASE CBA.ChargeBand
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
    END AS 'ChargeBandType',	
     
    PS.VATRegistered,
    PS.AnnualTurnover,

    CASE PS.AnnualTurnoverBandType
       WHEN 0 THEN 'Less than or equal to one million pounds'
            WHEN 1 THEN 'Greater than one million pounds'
            ELSE ''
        END AS 'AnnualTurnoverBandType',
      
    CASE PS.EEEPlacedOnMarketBandType
       WHEN 0 THEN 'More than or equal to 5T EEE placed on market'
            WHEN 1 THEN 'Less than 5T EEE placed on market'
            WHEN 2 THEN 'Both'
            ELSE ''
        END AS 'EEEPlacedOnMarketBandType',

    PS.ObligationType AS 'ObligationType',
    
    (SELECT STUFF((SELECT DISTINCT '; ' + SIC.Name
           FROM [Producer].[SICCode] SIC 
           WHERE PS.Id = SIC.ProducerSubmissionId 
           FOR XML PATH(''), TYPE)
           .value('.', 'NVARCHAR(MAX)'), 1, 2, '')) AS 'SICCODES',
    
    CASE PS.SellingTechniqueType
       WHEN 0 THEN 'Direct Selling to End User'
       WHEN 1 THEN 'Indirect Selling to End User'
       WHEN 2 THEN 'Both'
       ELSE ''
    END AS 'SellingTechniqueType',

    PS.CeaseToExist,	
      
    --Correspondence of Notices details
    CFNC.Title as 'CNTitle',
    CFNC.Forename as 'CNForename',
    CFNC.Surname as 'CNSurname',
    CFNC.Telephone as 'CNTelephone',
    CFNC.Mobile as 'CNMobile',
    CFNC.Fax as 'CNFax',
    CFNC.Email as 'CNEmail',
    CFNC_A.PrimaryName as 'CNPrimaryName',
    CFNC_A.SecondaryName as 'CNSecondaryName',
    CFNC_A.Street as 'CNStreet',
    CFNC_A.Town as 'CNTown',
    CFNC_A.Locality as 'CNLocality',
    CFNC_A.AdministrativeArea as 'CNAdministrativeArea',
    CFNC_A.PostCode as 'CNPostcode',
    CFNC_A_C.Name as 'CNCountry',
   
    --Registered Office details
    PBC.CompanyNumber,
    ROC.Title as 'CompanyContactTitle',
    ROC.Forename as 'CompanyContactForename',
    ROC.Surname as 'CompanyContactSurname',
    ROC.Telephone as 'CompanyContactTelephone',
    ROC.Mobile as 'CompanyContactMobile',
    ROC.Fax as 'CompanyContactFax',
    ROC.Email as 'CompanyContactEmail',
    ROC_A.PrimaryName as 'CompanyContactPrimaryName',
    ROC_A.SecondaryName as 'CompanyContactSecondaryName',
    ROC_A.Street as 'CompanyContactStreet',
    ROC_A.Town as 'CompanyContactTown',
    ROC_A.Locality as 'CompanyContactLocality',
    ROC_A.AdministrativeArea as 'CompanyContactAdministrativeArea',
    ROC_A.PostCode as 'CompanyContactPostcode',
    ROC_A_C.Name as 'CompanyContactCountry',
   
    --Principal place of business details
    PPOB.Title as 'PPOBContactTitle',
    PPOB.Forename as 'PPOBContactForename',
    PPOB.Surname as 'PPOBContactSurname',
    PPOB.Telephone as 'PPOBContactTelephone',
    PPOB.Mobile as 'PPOBContactMobile',
    PPOB.Fax as 'PPOBContactFax',
    PPOB.Email as 'PPOBContactEmail',
    PPOB_A.PrimaryName as 'PPOBContactPrimaryName',
    PPOB_A.SecondaryName as 'PPOBContactSecondaryName',
    PPOB_A.Street as 'PPOBContactStreet',
    PPOB_A.Town as 'PPOBContactTown',
    PPOB_A.Locality as 'PPOBContactLocality',
    PPOB_A.AdministrativeArea as 'PPOBContactAdministrativeArea',
    PPOB_A.PostCode as 'PPOBContactPostcode',
    PPOB_A_C.Name as 'PPOBContactCountry',
      
    --Overseas contact details
    AR.OverseasProducerName as 'OverseasProducerName',
    OC.Title as 'OverseasContactTitle',
    OC.Forename as 'OverseasContactForename',
    OC.Surname as 'OverseasContactSurname',
    OC.Telephone as 'OverseasContactTelephone',
    OC.Mobile as 'OverseasContactMobile',
    OC.Fax as 'OverseasContactFax',
    OC.Email as 'OverseasContactEmail',
    OC_A.PrimaryName as 'OverseasContactPrimaryName',
    OC_A.SecondaryName as 'OverseasContactSecondaryName',
    OC_A.Street as 'OverseasContactStreet',
    OC_A.Town as 'OverseasContactTown',
    OC_A.Locality as 'OverseasContactLocality',
    OC_A.AdministrativeArea as 'OverseasContactAdministrativeArea',
    OC_A.PostCode as 'OverseasContactPostcode',
    OC_A_C.Name as 'OverseasContactCountry',

    (SELECT STUFF(
      (SELECT '; ' + BN.Name
          FROM [Producer].[BrandName] BN
          WHERE BN.ProducerSubmissionId = PS.Id
          FOR XML PATH(''), TYPE)
          .value('.', 'NVARCHAR(MAX)'), 1, 2, '')
    ) AS 'BrandNames'

    FROM 
    (SELECT * FROM #SubmissionDetails
       UNION
     SELECT * FROM #PreviousSubmissionDetails) AS AllProducers

    INNER JOIN [Producer].[ProducerSubmission] PS ON AllProducers.ProducerSubmissionId = PS.Id

    INNER JOIN
      [Lookup].[ChargeBandAmount] CBA
        ON PS.[ChargeBandAmountId] = CBA.[Id]

    LEFT JOIN
      [Producer].[AuthorisedRepresentative] AR
        ON PS.AuthorisedRepresentativeId = AR.Id
        LEFT JOIN
          [Producer].[Contact] OC
            INNER JOIN
              [Producer].[Address] OC_A
                INNER JOIN
                  [Lookup].[Country] OC_A_C
                    ON OC_A.CountryId = OC_A_C.Id
                      ON OC.AddressId = OC_A.Id
                ON AR.OverseasContactId = OC.Id
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

     ORDER BY AllProducers.ProducerRegistrationNumber, AllProducers.SubmittedDate DESC;
END
GO

COMMIT TRANSACTION