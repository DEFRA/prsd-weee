SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE PCS.spgSubmissionChangesCsvData
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
    SELECT TOP 1
      PS.Id AS 'ProducerSubmissionId',
      RP.ProducerRegistrationNumber,
      RP.ComplianceYear,
      MU.SubmittedDate,
      '' AS 'ChangeType'
    INTO #PreviousSubmissionDetails
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
      MU.SubmittedDate < SD.SubmittedDate
    ORDER BY MU.SubmittedDate DESC;


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
    OC_A_C.Name as 'OverseasContactCountry'

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