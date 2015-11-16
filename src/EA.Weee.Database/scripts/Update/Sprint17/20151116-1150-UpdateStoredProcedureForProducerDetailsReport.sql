/****** Object:  StoredProcedure [Producer].[spgCSVDataBySchemeComplianceYearAndAuthorisedAuthority]    Script Date: 16/11/2015 13:28:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Author:		Priety Mahajan
-- Create date: 03 Nov 2015
-- Description:	Returns data about all producers currently registered
--				with the specified scheme in the specified year and AA for Scheme.
--				This data is suitable for populating the CSV file
--				which may be downloaded by AA to see the members associated with scheme in compliance year
-- =============================================
ALTER PROCEDURE [Producer].[spgCSVDataBySchemeComplianceYearAndAuthorisedAuthority]
		@ComplianceYear INT,
		@SchemeId uniqueidentifier = null,
		@CompetentAuthorityId uniqueidentifier = null
AS
BEGIN

	SET NOCOUNT ON;

SELECT
	S.SchemeName,
	S.ApprovalNumber,
	P.TradingName,
    
	PBC.Name AS 'CompanyName',
	
	CASE 
	WHEN PBC.Name is null then 'Partnership'
	ELSE 'Registered company'
	END as 'ProducerType',

	CASE 
	when PBC.Name is null then PBP.Name
	else PBC.NAME
	end as 'ProducerName',

	 P.RegistrationNumber AS 'PRN',
	 
	 P_First.UpdatedDate AS 'DateRegistered',

 	 P.UpdatedDate AS 'DateAmended',

	 SICCODES.SICCode as 'SICCODES',

     P.VATRegistered,

     P.AnnualTurnover,

	  CASE P.AnnualTurnoverBandType
			WHEN 0 THEN 'Less than or equal to one million pounds'
			WHEN 1 THEN 'Greater than one million pounds'
			ELSE ''
		END AS 'AnnualTurnoverBandType',
      
	  CASE P.EEEPlacedOnMarketBandType
			WHEN 0 THEN 'More than or equal to 5T EEE placed on market'
			WHEN 1 THEN 'Less than 5T EEE placed on market'
			WHEN 2 THEN 'Both'
			ELSE ''
		END AS 'EEEPlacedOnMarketBandType',

	CASE P.ObligationType
			WHEN 1 THEN 'B2B'
			WHEN 2 THEN 'B2C'
			WHEN 3 THEN 'Both'
			ELSE ''
		END AS 'ObligationType',
  
  CASE CBA.ChargeBand
			WHEN 0 THEN 'A'
			WHEN 1 THEN 'B'
			WHEN 2 THEN 'C'
			WHEN 3 THEN 'D'
			WHEN 4 THEN 'E'
			ELSE ''
		END AS 'ChargeBandType',
		
	CASE P.SellingTechniqueType
			WHEN 0 THEN 'Direct Selling to End User'
			WHEN 1 THEN 'Indirect Selling to End User'
			WHEN 2 THEN 'Both'
			ELSE ''
		END AS 'SellingTechniqueType',

     P.CeaseToExist,	
	  
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
	  Partners.Partners,
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
      [Producer].[Producer] P
INNER JOIN
      [PCS].[MemberUpload] MU
            ON P.MemberUploadId = MU.Id
INNER JOIN
      [Pcs].[Scheme] S
            ON MU.SchemeId = S.Id
INNER JOIN
	[Lookup].[ChargeBandAmount] CBA
		ON P.[ChargeBandAmountId] = CBA.[Id]
LEFT JOIN
      [Producer].[AuthorisedRepresentative] AR
            ON P.AuthorisedRepresentativeId = AR.Id
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
            ON P.ProducerBusinessId = PB.Id
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
				P.RegistrationNumber,
				P.UpdatedDate,
				P.SchemeId,
				ROW_NUMBER() OVER
				(
					PARTITION BY
						MU.ComplianceYear,
						P.RegistrationNumber,
						P.SchemeId
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
			AND P_First.SchemeId = S.Id  


LEFT JOIN
	(
	select distinct P.Id, STUFF((SELECT distinct '; ' + SIC.Name
         from [Producer].[SICCode] SIC 
         where P.Id = SIC.ProducerId 
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,2,'') SICCode
		from [Producer].[Producer] P
	
	)SICCODES on P.Id = SICCODES.Id

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
      MU.ComplianceYear = @ComplianceYear
AND

	  (S.Id = @SchemeId or @SchemeId is null )
AND
	 (@CompetentAuthorityId is null or  S.CompetentAuthorityId = @CompetentAuthorityId)
AND
      MU.IsSubmitted = 1
AND
      P.IsCurrentForComplianceYear = 1
ORDER BY
	S.SchemeName,
     ProducerName
END
GO


