GO
/****** Object:  StoredProcedure [Producer].[spgProducerPublicRegisterCSVDataByComplianceYear]    Script Date: 07/05/2026 ******/
SET ANSI_NULLS ON
GO

-- Modified Date: 07/05/2026 11:00:00 Ian Alvarado - Fix to use per-compliance-year DirectProducerSubmissionHistory for CompanyName, TradingName, 
--                                                   BusinessAddress and AuthorisedRepresentative details in the direct registrant dataset, 
--                                                   rather than the shared root entity fields on Organisation and DirectRegistrant which are no longer
--                                                   reliably synced on submission completion for migrated/legacy records. No changes to the scheme producer dataset.
-- =============================================================================================================
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [Producer].[spgProducerPublicRegisterCSVDataByComplianceYear]
        @ComplianceYear INT
AS
BEGIN
SET NOCOUNT ON;

-- Scheme producer query (unchanged)
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

    -- Direct registrant dataset
    -- FIX: Use per-compliance-year DirectProducerSubmissionHistory for CompanyName, TradingName,
    SELECT
        -- CompanyName: prefer the value recorded at submission time; fall back to root org name
        COALESCE(dpsh.CompanyName, o.[Name]) AS 'CompanyName',
        COALESCE(dpsh.CompanyName, o.[Name]) AS 'ProducerName',

        -- TradingName: prefer the value recorded at submission time; fall back to root org trading name
        COALESCE(dpsh.TradingName, o.[TradingName]) AS 'TradingName',

        dbo.GetObligationType(eorv.Id) AS 'ObligationType',

        -- Business address: prefer address stored in submission history for the compliance year;
        -- fall back to root organisation address for migrated/legacy records without a history address
        COALESCE(hist_ba.Address1,      oa.Address1)      AS 'ROAPrimaryName',
        NULL                                               AS 'ROASecondaryName',
        COALESCE(hist_ba.Address2,      oa.Address2)      AS 'ROAStreet',
        COALESCE(hist_ba.TownOrCity,    oa.TownOrCity)    AS 'ROATown',
        COALESCE(hist_ba.CountyOrRegion,oa.CountyOrRegion)AS 'ROALocality',
        NULL                                               AS 'ROAAdministrativeArea',
        COALESCE(hist_ba.PostCode,      oa.PostCode)      AS 'ROAPostCode',
        COALESCE(hist_loc.Name,         loc.Name)         AS 'ROACountry',

        -- Registered Office Contact Details (telephone/email/fax come from the business address record)
        CASE
            WHEN o.OrganisationType = 1 THEN COALESCE(hist_ba.Telephone, oa.Telephone)
            ELSE NULL
        END AS 'ROATelephone',
        CASE
            WHEN o.OrganisationType = 1 THEN COALESCE(hist_ba.Email, oa.Email)
            ELSE NULL
        END AS 'ROAEmail',
        CASE
            WHEN o.OrganisationType = 1 THEN COALESCE(hist_ba.Fax, oa.Fax)
            ELSE NULL
        END AS 'ROAFax',

        rp.ProducerRegistrationNumber AS 'PRN',
        'Direct registrant'           AS 'SchemeName',
        NULL AS 'SchemeOperator',
        NULL AS 'CSROAAddress1',
        NULL AS 'CSROAAddress2',
        NULL AS 'CSROATownOrCity',
        NULL AS 'CSROACountyOrRegion',
        NULL AS 'CSROAPostcode',
        NULL AS 'CSROACountry',

        -- Overseas producer (authorised representative): prefer history-level auth rep
        ap.OverseasProducerName AS 'OPNAName',
        pa.PrimaryName          AS 'OPNAPrimaryName',
        pa.SecondaryName        AS 'OPNASecondaryName',
        pa.Street               AS 'OPNAStreet',
        pa.Town                 AS 'OPNATown',
        pa.Locality             AS 'OPNALocality',
        pa.AdministrativeArea   AS 'OPNAAdministrativeArea',
        ac.Name                 AS 'OPNACountry',
        pa.PostCode             AS 'OPNAPostCode',

        dps.ComplianceYear,
        NULL AS 'PPOBPrimaryName',
        NULL AS 'PPOBSecondaryName',
        NULL AS 'PPOBStreet',
        NULL AS 'PPOBTown',
        NULL AS 'PPOBLocality',
        NULL AS 'PPOBAdministrativeArea',
        NULL AS 'PPOBCountry',
        NULL AS 'PPOBPostcode',
        1    AS IsDirectProducer

    FROM [Producer].[DirectProducerSubmission] dps
        INNER JOIN [Producer].[DirectRegistrant]    dr  ON dr.Id  = dps.DirectRegistrantId
        INNER JOIN [Organisation].[Organisation]     o   ON o.Id   = dr.OrganisationId
        -- Root org address kept as a fall-back for legacy/migrated records
        INNER JOIN [Organisation].[Address]          oa  ON oa.Id  = o.BusinessAddressId
        INNER JOIN [Lookup].[Country]                loc ON loc.Id = oa.CountryId
        INNER JOIN [Producer].[RegisteredProducer]   rp  ON rp.Id  = dps.RegisteredProducerId
                                                         AND dps.ComplianceYear = @ComplianceYear

        -- Latest submitted history row for this compliance-year submission
        INNER JOIN (
            SELECT
                DirectProducerSubmissionId,
                EeeOutputReturnVersionId,
                Id,
                -- Per-year fields that must NOT fall back to root entity
                CompanyName,
                TradingName,
                BusinessAddressId,
                AuthorisedRepresentativeId,
                ROW_NUMBER() OVER (
                    PARTITION BY DirectProducerSubmissionId
                    ORDER BY SubmittedDate DESC
                ) AS RowNum
            FROM [Producer].[DirectProducerSubmissionHistory]
            WHERE SubmittedDate IS NOT NULL
        ) dpsh ON dpsh.DirectProducerSubmissionId = dps.Id AND dpsh.RowNum = 1

        LEFT JOIN [PCS].[EeeOutputReturnVersion] eorv ON eorv.Id = dpsh.EeeOutputReturnVersionId

        -- Business address stored in the submission history for this compliance year
        -- Replaces the direct join to root Organisation.BusinessAddressId
        LEFT JOIN [Organisation].[Address] hist_ba  ON hist_ba.Id  = dpsh.BusinessAddressId
        LEFT JOIN [Lookup].[Country]       hist_loc ON hist_loc.Id = hist_ba.CountryId

        -- Authorised representative: prefer the one recorded in the submission history;
        -- fall back to the root DirectRegistrant authorised representative for migrated records
        LEFT JOIN [Producer].[AuthorisedRepresentative] ap ON ap.Id = COALESCE(dpsh.AuthorisedRepresentativeId, dr.AuthorisedRepresentativeId)
        LEFT JOIN [Producer].[Contact] pc ON pc.Id = ap.OverseasContactId
        LEFT JOIN [Producer].[Address] pa ON pa.Id = pc.AddressId
        LEFT JOIN [Lookup].[Country]   ac ON ac.Id = pa.CountryId

        -- First submission date (used for ordering/filtering, unchanged)
        INNER JOIN (
            SELECT
                ps.RegisteredProducerId,
                MIN(dpsh_first.SubmittedDate) AS SubmittedDate
            FROM [Producer].[DirectProducerSubmission] ps
                INNER JOIN [Producer].[DirectProducerSubmissionHistory] dpsh_first
                    ON dpsh_first.DirectProducerSubmissionId = ps.Id
                INNER JOIN [Producer].[RegisteredProducer] rp_first
                    ON rp_first.Id = ps.RegisteredProducerId
            WHERE dpsh_first.SubmittedDate IS NOT NULL
              AND rp_first.Removed = 0
            GROUP BY ps.RegisteredProducerId
        ) firstSubmitted ON firstSubmitted.RegisteredProducerId = dps.RegisteredProducerId

    WHERE dps.ComplianceYear = @ComplianceYear
      AND rp.Removed = 0

ORDER BY
    IsDirectProducer,
    S.SchemeName,
    ProducerName

END
GO