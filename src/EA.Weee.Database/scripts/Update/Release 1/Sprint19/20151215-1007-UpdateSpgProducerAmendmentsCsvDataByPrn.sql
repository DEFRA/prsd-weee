/****** Object:  StoredProcedure [Producer].[spgProducerAmendmentsCSVDataByPRN]    Script Date: 15/12/2015 10:06:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Updated date: 08 Dec 2015
-- Description:	Returns history of amendments for specific producer
-- =============================================
ALTER PROCEDURE [Producer].[spgProducerAmendmentsCSVDataByPRN]
		@PRN NVARCHAR(50)
AS
BEGIN

	SET NOCOUNT ON;

SELECT
	PBC.Name AS 'CompanyName',
	MU.ComplianceYear,
	P_First.UpdatedDate AS 'DateRegistered',
 	PS.UpdatedDate AS 'DateAmended',
	S.SchemeName AS 'PCSName',
	S.ApprovalNumber,
	COALESCE(PBC.Name, PBP.Name, '') AS 'ProducerName',

	CASE 
	WHEN PBC.Name is null then 'Partnership'
	ELSE 'Registered company'
	END as 'ProducerType',

	 RP.ProducerRegistrationNumber AS 'PRN',

	 Partners.Partners,

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
  
	SICCODES.SICCode as 'SICCODES',
		
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
     [Producer].[RegisteredProducer] RP
INNER JOIN
      [Producer].[ProducerSubmission] PS
			 ON PS.RegisteredProducerId = RP.Id
INNER JOIN
      [PCS].[MemberUpload] MU
            ON PS.MemberUploadId = MU.Id
INNER JOIN
      [Pcs].[Scheme] S
            ON MU.SchemeId = S.Id
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
			INNER JOIN
			(
			SELECT
			MU.ComplianceYear,
			PS.RegisteredProducerId,
			PS.UpdatedDate,
			ROW_NUMBER() OVER
			(
				PARTITION BY
					PS.RegisteredProducerId
				ORDER BY PS.UpdatedDate
			) AS RowNumber
		FROM
			Producer.ProducerSubmission PS
		INNER JOIN
			Producer.RegisteredProducer RP
				ON PS.RegisteredProducerId = RP.Id
		INNER JOIN
			PCS.MemberUpload MU
				ON PS.MemberUploadId = MU.Id
		WHERE
			MU.IsSubmitted = 1
		AND
			RP.IsAligned = 1
	) P_First
		ON PS.RegisteredProducerId = P_First.RegisteredProducerId
		AND P_First.RowNumber = 1

LEFT JOIN
	(
	SELECT DISTINCT
			PS.Id,
			STUFF((SELECT distinct '; ' + SIC.Name
				from [Producer].[SICCode] SIC 
				where PS.Id = SIC.ProducerSubmissionId 
				FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)') 
				,1,2,'') SICCode
		FROM
			[Producer].[ProducerSubmission] PS
		INNER JOIN Producer.RegisteredProducer RP
			ON PS.RegisteredProducerId = RP.Id
		WHERE 
			RP.IsAligned = 1
	
	) SICCODES
		ON PS.Id = SICCODES.Id

LEFT JOIN
	(
	select distinct P.Id, STUFF((SELECT distinct '; ' + PP.Name
         from [Producer].[Partner] PP 
         where P.Id = PP.PartnershipId
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,2,'') Partners
		from [Producer].[Partnership] P
	
	)Partners on PBP.Id = Partners.Id

WHERE
	RP.ProducerRegistrationNumber = @PRN
AND
	RP.IsAligned = 1
AND
    MU.IsSubmitted = 1
ORDER BY
MU.ComplianceYear desc,
PS.UpdatedDate desc
END
GO


