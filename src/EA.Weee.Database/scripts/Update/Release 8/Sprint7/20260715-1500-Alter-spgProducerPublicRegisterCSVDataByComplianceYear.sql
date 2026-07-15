GO
/****** Object:  StoredProcedure [Producer].[spgProducerPublicRegisterCSVDataByComplianceYear]    Script Date: 18/11/2025 17:29:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
    CASE
		WHEN ORG.OrganisationType = 1 THEN ROC.Telephone
		ELSE NULL
	END AS 'ROATelephone',
	CASE
		WHEN ORG.OrganisationType = 1 THEN ROC.Email
		ELSE NULL
	END AS 'ROAEmail',
	CASE
		WHEN ORG.OrganisationType = 1 THEN ROC.Fax
		ELSE NULL
	END AS 'ROAFax',

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

    -- Query for the direct registrant dataset - updated to use year-specific submission history
    SELECT
        -- Use submission history company name if available, otherwise fall back to organisation
        COALESCE(dpsh_latest.CompanyName, o.[Name], '') AS 'CompanyName',
        COALESCE(dpsh_latest.CompanyName, o.[Name], '') AS 'ProducerName',
        COALESCE(dpsh_latest.TradingName, o.[TradingName], '') AS 'TradingName',
        dbo.GetObligationType(eorv.Id) AS 'ObligationType',
        
        -- Registered Office Details - Use submission history business address if available
        COALESCE(ba.Address1, oa.Address1) as 'ROAPrimaryName',
        NULL as 'ROASecondaryName',
        COALESCE(ba.Address2, oa.Address2) as 'ROAStreet',
        COALESCE(ba.TownOrCity, oa.TownOrCity) as 'ROATown',
        COALESCE(ba.CountyOrRegion, oa.CountyOrRegion) as 'ROALocality',
        NULL as 'ROAAdministrativeArea',
        COALESCE(ba.Postcode, oa.Postcode) as 'ROAPostCode',
        COALESCE(ba_c.Name, loc.Name) as 'ROACountry',

        -- Registered Office Contact Details - year specific only do not fallback to root
        CASE
            WHEN o.OrganisationType = 1 THEN ba.Telephone
            ELSE NULL
        END AS 'ROATelephone',
        CASE
            WHEN o.OrganisationType = 1 THEN ba.Email
            ELSE NULL
        END AS 'ROAEmail',
        CASE
            WHEN o.OrganisationType = 1 THEN ba.Fax
            ELSE NULL
        END AS 'ROAFax',

        rp.ProducerRegistrationNumber AS 'PRN',
        'Direct registrant' AS SchemeName,
        NULL AS 'SchemeOperator',
        NULL AS 'CSROAAddress1',
        NULL AS 'CSROAAddress2',
        NULL AS 'CSROATownOrCity',
        NULL AS 'CSROACountyOrRegion',
        NULL AS 'CSROAPostcode',
        NULL AS 'CSROACountry',
        
        -- Overseas Producer Details - Use submission history authorised representative if available
        COALESCE(dpsh_ar.OverseasProducerName, ap.OverseasProducerName) as 'OPNAName',
        COALESCE(ar_pa.PrimaryName, pa.PrimaryName) as 'OPNAPrimaryName',
        COALESCE(ar_pa.SecondaryName, pa.SecondaryName) as 'OPNASecondaryName',
        COALESCE(ar_pa.Street, pa.Street) as 'OPNAStreet',
        COALESCE(ar_pa.Town, pa.Town) as 'OPNATown',
        COALESCE(ar_pa.Locality, pa.Locality) as 'OPNALocality',
        COALESCE(ar_pa.AdministrativeArea, pa.AdministrativeArea) as 'OPNAAdministrativeArea',
        COALESCE(ar_c.Name, ac.Name) as 'OPNACountry',
        COALESCE(ar_pa.PostCode, pa.PostCode) as 'OPNAPostCode',
        
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
        
        -- Organisation business address and contact (fallback for address only, not contact details)
        LEFT JOIN [Organisation].[Address] oa ON oa.Id = o.BusinessAddressId
        LEFT JOIN [Organisation].[Contact] oc ON oc.Id = dr.ContactId
        LEFT JOIN [Lookup].[Country] loc ON loc.Id = oa.CountryId
        
        INNER JOIN [Producer].[RegisteredProducer] rp ON dps.RegisteredProducerId = rp.Id AND dps.ComplianceYear = @ComplianceYear
        
        -- Get the latest submission history for THIS SPECIFIC compliance year
        INNER JOIN (
            SELECT 
                dpsh_inner.DirectProducerSubmissionId,
                dpsh_inner.EeeOutputReturnVersionId,
                dpsh_inner.Id,
                dpsh_inner.CompanyName,
                dpsh_inner.TradingName,
                dpsh_inner.BusinessAddressId,
                dpsh_inner.ContactId,
                dpsh_inner.AuthorisedRepresentativeId,
                ROW_NUMBER() OVER (PARTITION BY dpsh_inner.DirectProducerSubmissionId ORDER BY dpsh_inner.SubmittedDate DESC) AS RowNum
            FROM [Producer].[DirectProducerSubmissionHistory] dpsh_inner
            INNER JOIN [Producer].[DirectProducerSubmission] dps_inner ON dpsh_inner.DirectProducerSubmissionId = dps_inner.Id
            WHERE dpsh_inner.SubmittedDate IS NOT NULL
                AND dps_inner.ComplianceYear = @ComplianceYear  -- filter by compliance year
        ) DPSH ON DPSH.DirectProducerSubmissionId = DPS.Id AND DPSH.RowNum = 1
        
        -- Get year-specific submission data for this compliance year only
        OUTER APPLY (
            SELECT TOP 1
                ps.CompanyName,
                ps.TradingName,
                ps.BusinessAddressId,
                ps.ContactId,
                ps.AuthorisedRepresentativeId
            FROM [Producer].[DirectProducerSubmissionHistory] ps
            INNER JOIN [Producer].[DirectProducerSubmission] dps_apply ON ps.DirectProducerSubmissionId = dps_apply.Id
            WHERE ps.DirectProducerSubmissionId = DPS.Id
                AND ps.SubmittedDate IS NOT NULL
                AND dps_apply.ComplianceYear = @ComplianceYear  -- Filter by compliance year
            ORDER BY ps.SubmittedDate DESC
        ) dpsh_latest
        
        -- Year-specific business address from submission history
        LEFT JOIN [Organisation].[Address] ba ON ba.Id = dpsh_latest.BusinessAddressId
        LEFT JOIN [Lookup].[Country] ba_c ON ba_c.Id = ba.CountryId
        
        -- Year-specific authorised representative from submission history
        LEFT JOIN [Producer].[AuthorisedRepresentative] dpsh_ar ON dpsh_ar.Id = dpsh_latest.AuthorisedRepresentativeId
        LEFT JOIN [Producer].[Contact] ar_pc ON ar_pc.Id = dpsh_ar.OverseasContactId
        LEFT JOIN [Producer].[Address] ar_pa ON ar_pa.Id = ar_pc.AddressId
        LEFT JOIN [Lookup].[Country] ar_c ON ar_c.Id = ar_pa.CountryId
        
        -- EEE data
        LEFT JOIN [PCS].[EeeOutputReturnVersion] eorv ON eorv.Id = dpsh.EeeOutputReturnVersionId
        
        -- Root authorised representative (fallback)
        LEFT JOIN [Producer].[AuthorisedRepresentative] ap ON ap.Id = dr.AuthorisedRepresentativeId
        LEFT JOIN [Producer].[Contact] pc ON pc.Id = ap.OverseasContactId
        LEFT JOIN [Producer].[Address] pa ON pa.Id = pc.AddressId
        LEFT JOIN [Lookup].[Country] ac ON ac.Id = pa.CountryId
        
        -- Subquery to get the first submission date for each producer
        INNER JOIN
        (
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
    SchemeName,
    ProducerName

END