--USE [EA.Weee]
--GO
--/****** Object:  StoredProcedure [Producer].[spgProducerPublicRegisterCSVDataByComplianceYear]    Script Date: 16/10/2024 10:37:31 ******/
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
---- Description:	Return producer public register report data.
---- =============================================

--ALTER PROCEDURE [Producer].[spgProducerPublicRegisterCSVDataByComplianceYear]
--        @ComplianceYear INT
--AS
--BEGIN
    
--    SET NOCOUNT ON;

--    SELECT
    
--    PBC.Name as 'CompanyName',

--    CASE 
--    when PBC.Name is null then PBP.Name
--    else PBC.NAME
--    end as 'ProducerName',
    
--    PS.TradingName,

--    PS.ObligationType,

--    --Registered Office Address
--    ROC_A.PrimaryName AS 'ROAPrimaryName',
--    ROC_A.SecondaryName AS 'ROASecondaryName',
--    ROC_A.Street AS 'ROAStreet',
--    ROC_A.Town AS 'ROATown',
--    ROC_A.Locality AS 'ROALocality',
--    ROC_A.AdministrativeArea AS 'ROAAdministrativeArea',
--    ROC_A.PostCode AS 'ROAPostCode',
--    ROC_A_C.Name AS 'ROACountry',

--    --Registered Office Telephone Number
--    ROC.Telephone as 'ROATelephone',

--    --Registered Office Telephone Number
--    ROC.Email as 'ROAEmail',

--    --PRN
--    RP.ProducerRegistrationNumber AS 'PRN',
        
--    --Compliance Scheme Name
--    S.SchemeName,

--      --Compliance Scheme Operator
--    CASE 
--    WHEN ORG.Name is null THEN ORG.TradingName
--    ELSE ORG.NAME
--    END AS 'SchemeOperator',

--    --PCS registered office
--    ORG_A.Address1 AS 'CSROAAddress1',
--    ORG_A.Address2 AS 'CSROAAddress2',
--    ORG_A.TownOrCity AS 'CSROATownOrCity',
--    ORG_A.CountyOrRegion AS 'CSROACountyOrRegion',
--    ORG_A.Postcode AS 'CSROAPostcode',
--    ORG_A_C.Name AS 'CSROACountry',

--    --Overseas Producer Name and Address
--    AR.OverseasProducerName AS 'OPNAName',
--    OC_A.PrimaryName AS 'OPNAPrimaryName',
--    OC_A.SecondaryName AS 'OPNASecondaryName',
--    OC_A.Street AS 'OPNAStreet',
--    OC_A.Town AS 'OPNATown',
--    OC_A.Locality AS 'OPNALocality',
--    OC_A.AdministrativeArea AS 'OPNAAdministrativeArea', 
--    OC_A_C.Name AS 'OPNACountry',
--    OC_A.PostCode AS 'OPNAPostCode',
    
--    MU.ComplianceYear,

--    PPOB_A.PrimaryName as 'PPOBPrimaryName',
--    PPOB_A.SecondaryName as 'PPOBSecondaryName',
--    PPOB_A.Street as 'PPOBStreet',
--    PPOB_A.Town as 'PPOBTown',
--    PPOB_A.Locality as 'PPOBLocality',
--    PPOB_A.AdministrativeArea as 'PPOBAdministrativeArea',
--    PPOB_A_C.Name as 'PPOBCountry',
--    PPOB_A.PostCode as 'PPOBPostcode'
       

--FROM
--      [Producer].[RegisteredProducer] RP
--INNER JOIN
--      [Producer].[ProducerSubmission] PS
--            ON RP.CurrentSubmissionId = PS.Id
--INNER JOIN
--      [PCS].[MemberUpload] MU
--            ON PS.MemberUploadId = MU.Id
--INNER JOIN
--      [Pcs].[Scheme] S
--            ON MU.SchemeId = S.Id
--INNER JOIN
--      [Organisation].[Organisation] ORG
--            ON ORG.Id = S.OrganisationId

--INNER JOIN 
--      [Organisation].[Address] ORG_A
--            INNER JOIN
--                  [Lookup].[Country] ORG_A_C
--                           ON ORG_A.CountryId = ORG_A_C.Id
--            ON ORG.BusinessAddressId = ORG_A.Id

--LEFT JOIN
--      [Producer].[AuthorisedRepresentative] AR
--            ON PS.AuthorisedRepresentativeId = AR.Id
--      LEFT JOIN
--            [Producer].[Contact] OC
--            INNER JOIN
--                  [Producer].[Address] OC_A
--                  INNER JOIN
--                        [Lookup].[Country] OC_A_C
--                              ON OC_A.CountryId = OC_A_C.Id
--                        ON OC.AddressId = OC_A.Id
--                  ON AR.OverseasContactId = OC.Id
--LEFT JOIN
--      [Producer].[Business] PB
--            ON PS.ProducerBusinessId = PB.Id
--      LEFT JOIN
--            [Producer].[Contact] CFNC
--            INNER JOIN
--                  [Producer].[Address] CFNC_A
--                  INNER JOIN
--                        [Lookup].[Country] CFNC_A_C
--                              ON CFNC_A.CountryId = CFNC_A_C.Id   
--                        ON CFNC.AddressId = CFNC_A.Id
--                  ON PB.CorrespondentForNoticesContactId = CFNC.Id
--      LEFT JOIN
--            [Producer].[Company] PBC
--                  ON PB.CompanyId = PBC.Id
--            LEFT JOIN
--                  [Producer].[Contact] ROC
--                  INNER JOIN
--                        [Producer].[Address] ROC_A
--                        INNER JOIN
--                              [Lookup].[Country] ROC_A_C
--                                    ON ROC_A.CountryId = ROC_A_C.Id     
--                              ON ROC.AddressId = ROC_A.Id
--                        ON PBC.RegisteredOfficeContactId = ROC.Id
--      LEFT JOIN
--            [Producer].[Partnership] PBP
--                  ON PB.PartnershipId = PBP.Id
--            LEFT JOIN
--                  [Producer].[Contact] PPOB
--                  INNER JOIN
--                        [Producer].[Address] PPOB_A
--                        INNER JOIN
--                              [Lookup].[Country] PPOB_A_C
--                                    ON PPOB_A.CountryId = PPOB_A_C.Id   
--                              ON PPOB.AddressId = PPOB_A.Id
--                        ON PBP.PrincipalPlaceOfBusinessId = PPOB.Id
--            INNER JOIN
--            (
--            SELECT
--                PS.RegisteredProducerId,
--                ROW_NUMBER() OVER
--                (
--                    PARTITION BY
--                        PS.RegisteredProducerId
--                    ORDER BY PS.UpdatedDate
--                ) AS RowNumber
--            FROM
--                [Producer].[ProducerSubmission] PS
--            INNER JOIN
--                [PCS].[MemberUpload] MU
--                    ON PS.MemberUploadId = MU.Id
--			INNER JOIN
--				Producer.RegisteredProducer RP
--					ON PS.RegisteredProducerId = RP.Id
--            WHERE
--                MU.IsSubmitted = 1
--			AND
--				RP.Removed = 0
--        ) P_First
--            ON PS.RegisteredProducerId = P_First.RegisteredProducerId
--            AND P_First.RowNumber = 1
--LEFT JOIN
--    (
--    select distinct P.Id, STUFF((SELECT distinct '; ' + PP.Name
--         from [Producer].[Partner] PP 
--         where P.Id = PP.PartnershipId
--            FOR XML PATH(''), TYPE
--            ).value('.', 'NVARCHAR(MAX)') 
--        ,1,2,'') Partners
--        from [Producer].[Partnership] P
    
--    )Partners on PBP.Id = Partners.Id

--WHERE
--      MU.ComplianceYear = @ComplianceYear
--AND
--      MU.IsSubmitted = 1
--AND
--	  RP.Removed = 0
--ORDER BY
--    S.SchemeName,
--    ProducerName
    
--END
-- Set options

ALTER PROCEDURE [Producer].[spgProducerPublicRegisterCSVDataByComplianceYear]
        @ComplianceYear INT
AS
BEGIN
SET NOCOUNT ON;

-- Main query
SELECT
    -- Company and Producer details
    PBC.Name AS 'CompanyName',
    CASE 
        WHEN PBC.Name IS NULL THEN PBP.Name
        ELSE PBC.Name
    END AS 'ProducerName',
    PS.TradingName,
    PS.ObligationType,

    -- Registered Office Address
    ROC_A.PrimaryName AS 'ROAPrimaryName',
    ROC_A.SecondaryName AS 'ROASecondaryName',
    ROC_A.Street AS 'ROAStreet',
    ROC_A.Town AS 'ROATown',
    ROC_A.Locality AS 'ROALocality',
    ROC_A.AdministrativeArea AS 'ROAAdministrativeArea',
    ROC_A.PostCode AS 'ROAPostCode',
    ROC_A_C.Name AS 'ROACountry',

    -- Registered Office Contact Details
    ROC.Telephone AS 'ROATelephone',
    ROC.Email AS 'ROAEmail',

    -- Producer Registration Number
    RP.ProducerRegistrationNumber AS 'PRN',
    
    -- Compliance Scheme Details
    S.SchemeName,
    CASE 
        WHEN ORG.Name IS NULL THEN ORG.TradingName
        ELSE ORG.Name
    END AS 'SchemeOperator',

    -- Compliance Scheme Registered Office
    ORG_A.Address1 AS 'CSROAAddress1',
    ORG_A.Address2 AS 'CSROAAddress2',
    ORG_A.TownOrCity AS 'CSROATownOrCity',
    ORG_A.CountyOrRegion AS 'CSROACountyOrRegion',
    ORG_A.Postcode AS 'CSROAPostcode',
    ORG_A_C.Name AS 'CSROACountry',

    -- Overseas Producer Details
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

    -- Principal Place of Business
    PPOB_A.PrimaryName AS 'PPOBPrimaryName',
    PPOB_A.SecondaryName AS 'PPOBSecondaryName',
    PPOB_A.Street AS 'PPOBStreet',
    PPOB_A.Town AS 'PPOBTown',
    PPOB_A.Locality AS 'PPOBLocality',
    PPOB_A.AdministrativeArea AS 'PPOBAdministrativeArea',
    PPOB_A_C.Name AS 'PPOBCountry',
    PPOB_A.PostCode AS 'PPOBPostcode',
	0 AS IsDirectProducer

FROM Producer.RegisteredProducer RP
INNER JOIN Producer.ProducerSubmission PS ON RP.CurrentSubmissionId = PS.Id
INNER JOIN PCS.MemberUpload MU ON PS.MemberUploadId = MU.Id
INNER JOIN Pcs.Scheme S ON MU.SchemeId = S.Id
INNER JOIN Organisation.Organisation ORG ON ORG.Id = S.OrganisationId
INNER JOIN Organisation.Address ORG_A ON ORG.BusinessAddressId = ORG_A.Id
INNER JOIN Lookup.Country ORG_A_C ON ORG_A.CountryId = ORG_A_C.Id

-- Authorized Representative
LEFT JOIN Producer.AuthorisedRepresentative AR ON PS.AuthorisedRepresentativeId = AR.Id
LEFT JOIN Producer.Contact OC ON AR.OverseasContactId = OC.Id
LEFT JOIN Producer.Address OC_A ON OC.AddressId = OC_A.Id
LEFT JOIN Lookup.Country OC_A_C ON OC_A.CountryId = OC_A_C.Id

-- Producer Business
LEFT JOIN Producer.Business PB ON PS.ProducerBusinessId = PB.Id
LEFT JOIN Producer.Contact CFNC ON PB.CorrespondentForNoticesContactId = CFNC.Id
LEFT JOIN Producer.Address CFNC_A ON CFNC.AddressId = CFNC_A.Id
LEFT JOIN Lookup.Country CFNC_A_C ON CFNC_A.CountryId = CFNC_A_C.Id

-- Producer Company
LEFT JOIN Producer.Company PBC ON PB.CompanyId = PBC.Id
LEFT JOIN Producer.Contact ROC ON PBC.RegisteredOfficeContactId = ROC.Id
LEFT JOIN Producer.Address ROC_A ON ROC.AddressId = ROC_A.Id
LEFT JOIN Lookup.Country ROC_A_C ON ROC_A.CountryId = ROC_A_C.Id

-- Producer Partnership
LEFT JOIN Producer.Partnership PBP ON PB.PartnershipId = PBP.Id
LEFT JOIN Producer.Contact PPOB ON PBP.PrincipalPlaceOfBusinessId = PPOB.Id
LEFT JOIN Producer.Address PPOB_A ON PPOB.AddressId = PPOB_A.Id
LEFT JOIN Lookup.Country PPOB_A_C ON PPOB_A.CountryId = PPOB_A_C.Id

-- Subquery to get the first submission for each producer
INNER JOIN (
    SELECT
        PS.RegisteredProducerId,
        ROW_NUMBER() OVER (
            PARTITION BY PS.RegisteredProducerId
            ORDER BY PS.UpdatedDate
        ) AS RowNumber
    FROM Producer.ProducerSubmission PS
    INNER JOIN PCS.MemberUpload MU ON PS.MemberUploadId = MU.Id
    INNER JOIN Producer.RegisteredProducer RP ON PS.RegisteredProducerId = RP.Id
    WHERE MU.IsSubmitted = 1 AND RP.Removed = 0
) P_First ON PS.RegisteredProducerId = P_First.RegisteredProducerId AND P_First.RowNumber = 1

-- Partners subquery
LEFT JOIN (
    SELECT DISTINCT 
        P.Id, 
        STUFF((
            SELECT DISTINCT '; ' + PP.Name
            FROM Producer.Partner PP 
            WHERE P.Id = PP.PartnershipId
            FOR XML PATH(''), TYPE
        ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS Partners
    FROM Producer.Partnership P
) Partners ON PBP.Id = Partners.Id

WHERE
    MU.ComplianceYear = @ComplianceYear
    AND RP.Removed = 0

UNION ALL

	-- Query for the direct registrant dataset
    SELECT
		o.[Name] AS 'CompanyName',
		COALESCE(ap.OverseasProducerName, o.[Name], '') AS 'ProducerName',
		COALESCE(ap.OverseasProducerTradingName, o.[TradingName], '') AS 'TradingName',
		dbo.GetObligationType(eorv.Id) AS 'ObligationType',
		-- Registered Office Details
        oa.Address1 as 'ROAPrimaryName',
        NULL as 'ROASecondaryName',
        oa.Address2 as 'ROAStreet',
        oa.TownOrCity as 'ROATown',
        oa.CountyOrRegion as 'ROALocality',
        NULL as 'ROAAdministrativeArea',
        oa.PostCode as 'ROAPostCode',
        loc.Name as 'ROACountry',
		oa.Telephone as 'ROATelephone',
        oa.Email as 'ROAEmail',
		rp.ProducerRegistrationNumber AS 'PRN',
		'Direct registrant' AS SchemeName,
		NULL AS 'SchemeOperator',
		NULL AS 'CSROAAddress1',
		NULL AS 'CSROAAddress2',
		NULL AS 'CSROATownOrCity',
		NULL AS 'CSROACountyOrRegion',
		NULL AS 'CSROAPostcode',
		NULL AS 'CSROACountry',
		ap.OverseasProducerName as 'OPNAName',
		pa.PrimaryName as 'OPNAPrimaryName',
        pa.SecondaryName as 'OPNASecondaryName',
		pa.Street as 'OPNAStreet',
        pa.Town as 'OPNATown',
        pa.Locality as 'OPNALocality',
        pa.AdministrativeArea as 'OPNAAdministrativeArea',
		ac.Name as 'OPNACountry',
        pa.PostCode as 'OPNAPostCode',
		dps.ComplianceYear,
		NULL AS 'PPOBPrimaryName',
		NULL AS 'PPOBSecondaryName',
		NULL AS 'PPOBStreet',
		NULL AS 'PPOBTown',
		NULL AS 'PPOBLocality',
		NULL AS 'PPOBAdministrativeArea',
		NULL AS 'PPOBCountry',
		NULL AS 'PPOBPostcode',
		1 AS IsDirectProducer
	FROM
        [Producer].[DirectProducerSubmission] dps
        INNER JOIN [Producer].[DirectRegistrant] dr ON dr.Id = dps.DirectRegistrantId
        INNER JOIN [Organisation].[Organisation] o ON o.Id = dr.OrganisationId
        INNER JOIN [Organisation].[Address] oa ON oa.Id = o.BusinessAddressId
		INNER JOIN [Organisation].[Contact] oc ON oc.Id = dr.ContactId
        INNER JOIN [Lookup].[Country] loc ON loc.Id = oa.CountryId
        INNER JOIN [Producer].[RegisteredProducer] rp ON dps.RegisteredProducerId = rp.Id AND dps.ComplianceYear = @ComplianceYear
        INNER JOIN [Producer].[DirectProducerSubmissionHistory] dpsh ON dpsh.Id = dps.CurrentSubmissionId
        LEFT JOIN [PCS].[EeeOutputReturnVersion] eorv ON eorv.Id = dpsh.EeeOutputReturnVersionId
        LEFT JOIN [Producer].[AuthorisedRepresentative] ap ON ap.Id = dr.AuthorisedRepresentativeId
        LEFT JOIN [Producer].[Contact] pc ON pc.Id = ap.OverseasContactId
        LEFT JOIN [Producer].[Address] pa ON pa.Id = pc.AddressId
        LEFT JOIN [Lookup].[Country] ac ON ac.Id = pa.CountryId
        INNER JOIN
		(
			-- Subquery to get the first submission date for each producer
			SELECT
                ps.RegisteredProducerId,
                MIN(dpsh.SubmittedDate) AS SubmittedDate
            FROM
                [Producer].[DirectProducerSubmission] ps
                INNER JOIN [Producer].[DirectProducerSubmissionHistory] dpsh ON dpsh.DirectProducerSubmissionId = ps.Id
                INNER JOIN [Producer].[RegisteredProducer] rp ON ps.RegisteredProducerId = rp.Id
            WHERE
                dpsh.SubmittedDate IS NOT NULL
                AND (RP.Removed = 0)
            GROUP BY
                ps.RegisteredProducerId
		) firstSubmitted ON dps.RegisteredProducerId = firstSubmitted.RegisteredProducerId
WHERE
    dps.ComplianceYear = @ComplianceYear
    AND RP.Removed = 0

ORDER BY
	IsDirectProducer,
    S.SchemeName,
    ProducerName

END