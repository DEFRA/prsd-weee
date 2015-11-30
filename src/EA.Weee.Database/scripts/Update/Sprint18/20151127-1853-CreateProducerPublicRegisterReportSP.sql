SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Dhaval Shah
-- Create date: 24 Nov 2015
-- Description:	Return producer public register report data.
-- =============================================

CREATE PROCEDURE [Producer].[spgProducerPublicRegisterCSVDataByComplianceYear]
        @ComplianceYear INT
AS
BEGIN
    
    SET NOCOUNT ON;

    SELECT
    
    PBC.Name as 'CompanyName',

    CASE 
    when PBC.Name is null then PBP.Name
    else PBC.NAME
    end as 'ProducerName',
    
    P.TradingName,

    CASE P.ObligationType
            WHEN 1 THEN 'B2B'
            WHEN 2 THEN 'B2C'
            WHEN 3 THEN 'Both'
            ELSE ''
        END AS 'ObligationType',
    
     
    
    --Registered Office Address
    ROC_A.PrimaryName AS 'ROAPrimaryName',
    ROC_A.SecondaryName AS 'ROASecondaryName',
    ROC_A.Street AS 'ROAStreet',
    ROC_A.Town AS 'ROATown',
    ROC_A.Locality AS 'ROALocality',
    ROC_A.AdministrativeArea AS 'ROAAdministrativeArea',
    ROC_A.PostCode AS 'ROAPostCode',
    ROC_A_C.Name AS 'ROACountry',

    --Registered Office Telephone Number
    ROC.Telephone as 'ROATelephone',

    --Registered Office Telephone Number
    ROC.Email as 'ROAEmail',

    --PRN
    P.RegistrationNumber AS 'PRN',
        
    --Compliance Scheme Name
    S.SchemeName,

      --Compliance Scheme Operator
    CASE 
    WHEN ORG.Name is null THEN ORG.TradingName
    ELSE ORG.NAME
    END AS 'SchemeOperator',

    --PCS registered office
    ORG_A.Address1 AS 'CSROAAddress1',
    ORG_A.Address2 AS 'CSROAAddress2',
    ORG_A.TownOrCity AS 'CSROATownOrCity',
    ORG_A.CountyOrRegion AS 'CSROACountyOrRegion',
    ORG_A.Postcode AS 'CSROAPostcode',
    ORG_A_C.Name AS 'CSROACountry',

    --Overseas Producer Name and Address
    AR.OverseasProducerName AS 'OPNAName',
    OC_A.PrimaryName AS 'OPNAPrimaryName',
    OC_A.SecondaryName AS 'OPNASecondaryName',
    OC_A.Street AS 'OPNAStreet',
    OC_A.Town AS 'OPNATown',
    OC_A.Locality AS 'OPNALocality',
    OC_A.AdministrativeArea AS 'OPNAAdministrativeArea', 
    OC_A_C.Name AS 'OPNACountry',
    OC_A.PostCode AS 'OPNAPostCode',
    
    MU.ComplianceYear,

    PPOB_A.PrimaryName as 'PPOBPrimaryName',
    PPOB_A.SecondaryName as 'PPOBSecondaryName',
    PPOB_A.Street as 'PPOBStreet',
    PPOB_A.Town as 'PPOBTown',
    PPOB_A.Locality as 'PPOBLocality',
    PPOB_A.AdministrativeArea as 'PPOBAdministrativeArea',
    PPOB_A_C.Name as 'PPOBCountry',
    PPOB_A.PostCode as 'PPOBPostcode'
       

FROM
      [Producer].[Producer] P
INNER JOIN
      [PCS].[MemberUpload] MU
            ON P.MemberUploadId = MU.Id
INNER JOIN
      [Pcs].[Scheme] S
            ON MU.SchemeId = S.Id
INNER JOIN
      [Organisation].[Organisation] ORG
            ON ORG.Id = S.OrganisationId

INNER JOIN 
      [Organisation].[Address] ORG_A
            INNER JOIN
                  [Lookup].[Country] ORG_A_C
                           ON ORG_A.CountryId = ORG_A_C.Id
            ON ORG.BusinessAddressId = ORG_A.Id

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
      MU.IsSubmitted = 1
AND
      P.IsCurrentForComplianceYear = 1
ORDER BY
    S.SchemeName,
     ProducerName
    
END
GO
