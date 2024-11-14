ALTER PROCEDURE [Producer].[spgCSVDataBySchemeComplianceYearAndAuthorisedAuthority]
    @ComplianceYear INT,
    @IncludeRemovedProducer BIT,
    @IncludeBrandNames BIT,
    @SchemeId UNIQUEIDENTIFIER = NULL,
    @CompetentAuthorityId UNIQUEIDENTIFIER = NULL,
    @FilterByDirectRegistrant BIT = 0,
    @FilterBySchemes BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Main query to retrieve producer data
    SELECT
        -- Scheme Information
        S.SchemeName,
        S.ApprovalNumber,
        
        -- Producer Information
        PS.TradingName,
        PBC.Name AS 'CompanyName',
        CASE 
            WHEN PBC.Name IS NULL THEN 'Partnership'
            ELSE 'Registered company'
        END AS 'ProducerType',
        CASE 
            WHEN PBC.Name IS NULL THEN PBP.Name
            ELSE PBC.Name
        END AS 'ProducerName',
        RP.ProducerRegistrationNumber AS 'PRN',
        P_First.SubmittedDate AS 'DateRegistered',
        MU.SubmittedDate AS 'DateAmended',
        
        -- Business Details
        SICCODES.SICCode as 'SICCODES',
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
        CASE PS.SellingTechniqueType
            WHEN 0 THEN 'Direct Selling to End User'
            WHEN 1 THEN 'Indirect Selling to End User'
            WHEN 2 THEN 'Both'
            ELSE ''
        END AS 'SellingTechniqueType',
        PS.CeaseToExist,
        
        -- Correspondence for Notices Contact Details
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
   
        -- Registered Office Details
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
   
        -- Principal Place of Business Details
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
      
        -- Overseas Contact Details
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
        
        -- Removal Status
        CASE RP.Removed
            WHEN 1 THEN 'Yes'
            WHEN 0 THEN 'No'
        END AS 'RemovedFromScheme',

        -- Brand Names (if requested)
        CASE @IncludeBrandNames
            WHEN 1 THEN 
                (SELECT STUFF((SELECT '; ' + BN.Name
                FROM [Producer].[BrandName] BN
                WHERE BN.ProducerSubmissionId = PS.Id
                FOR XML PATH(''), TYPE)
                .value('.', 'NVARCHAR(MAX)') 
                , 1, 2, ''))
            WHEN 0 THEN NULL
        END AS 'BrandNames',
		0 AS IsDirectProducer

    FROM
        [Producer].[RegisteredProducer] RP
    INNER JOIN
        [Producer].[ProducerSubmission] PS ON RP.[CurrentSubmissionId] = PS.[Id]
    INNER JOIN
        [PCS].[MemberUpload] MU ON PS.MemberUploadId = MU.Id
    INNER JOIN
        [Pcs].[Scheme] S ON RP.SchemeId = S.Id
    INNER JOIN
        [Lookup].[ChargeBandAmount] CBA ON PS.[ChargeBandAmountId] = CBA.[Id]
    LEFT JOIN
        [Producer].[AuthorisedRepresentative] AR ON PS.AuthorisedRepresentativeId = AR.Id
    LEFT JOIN
        [Producer].[Contact] OC ON AR.OverseasContactId = OC.Id
    LEFT JOIN
        [Producer].[Address] OC_A ON OC.AddressId = OC_A.Id
    LEFT JOIN
        [Lookup].[Country] OC_A_C ON OC_A.CountryId = OC_A_C.Id
    LEFT JOIN
        [Producer].[Business] PB ON PS.ProducerBusinessId = PB.Id
    LEFT JOIN
        [Producer].[Contact] CFNC ON PB.CorrespondentForNoticesContactId = CFNC.Id
    LEFT JOIN
        [Producer].[Address] CFNC_A ON CFNC.AddressId = CFNC_A.Id
    LEFT JOIN
        [Lookup].[Country] CFNC_A_C ON CFNC_A.CountryId = CFNC_A_C.Id
    LEFT JOIN
        [Producer].[Company] PBC ON PB.CompanyId = PBC.Id
    LEFT JOIN
        [Producer].[Contact] ROC ON PBC.RegisteredOfficeContactId = ROC.Id
    LEFT JOIN
        [Producer].[Address] ROC_A ON ROC.AddressId = ROC_A.Id
    LEFT JOIN
        [Lookup].[Country] ROC_A_C ON ROC_A.CountryId = ROC_A_C.Id
    LEFT JOIN
        [Producer].[Partnership] PBP ON PB.PartnershipId = PBP.Id
    LEFT JOIN
        [Producer].[Contact] PPOB ON PBP.PrincipalPlaceOfBusinessId = PPOB.Id
    LEFT JOIN
        [Producer].[Address] PPOB_A ON PPOB.AddressId = PPOB_A.Id
    LEFT JOIN
        [Lookup].[Country] PPOB_A_C ON PPOB_A.CountryId = PPOB_A_C.Id
    INNER JOIN
    (
        -- Subquery to get the first submission date for each producer
        SELECT
            MU.ComplianceYear,
            PS.RegisteredProducerId,
            MU.SubmittedDate,
            ROW_NUMBER() OVER (
                PARTITION BY PS.RegisteredProducerId
                ORDER BY PS.UpdatedDate
            ) AS RowNumber
        FROM
            Producer.ProducerSubmission PS
        INNER JOIN
            PCS.MemberUpload MU ON PS.MemberUploadId = MU.Id
        INNER JOIN 
            Producer.RegisteredProducer RP ON PS.RegisteredProducerId = RP.Id
        WHERE
            MU.IsSubmitted = 1
            AND (@IncludeRemovedProducer = 1 OR RP.Removed = 0)
    ) P_First ON PS.RegisteredProducerId = P_First.RegisteredProducerId AND P_First.RowNumber = 1
    LEFT JOIN
    (
        -- Subquery to concatenate SIC codes
        SELECT DISTINCT
            PS.Id,
            STUFF((SELECT distinct '; ' + SIC.Name
                FROM [Producer].[SICCode] SIC 
                WHERE PS.Id = SIC.ProducerSubmissionId 
                FOR XML PATH(''), TYPE
                ).value('.', 'NVARCHAR(MAX)') 
                ,1,2,'') SICCode
        FROM
            [Producer].[ProducerSubmission] PS
    ) SICCODES ON PS.Id = SICCODES.Id
    LEFT JOIN
    (
        -- Subquery to concatenate partner names
        SELECT DISTINCT
            P.Id,
            STUFF((SELECT distinct '; ' + PP.Name
                FROM [Producer].[Partner] PP 
                WHERE P.Id = PP.PartnershipId
                FOR XML PATH(''), TYPE
                ).value('.', 'NVARCHAR(MAX)') 
                ,1,2,'') Partners
        FROM
            [Producer].[Partnership] P
    ) Partners ON PBP.[Id] = Partners.[Id]
    WHERE
        RP.ComplianceYear = @ComplianceYear
        AND (@SchemeId IS NULL OR RP.SchemeId = @SchemeId)
        AND (@CompetentAuthorityId IS NULL OR S.CompetentAuthorityId = @CompetentAuthorityId)
        AND (@IncludeRemovedProducer = 1 OR RP.Removed = 0)
        AND (@FilterByDirectRegistrant = 0)

	UNION ALL


	-- Query for the direct registrant dataset
    SELECT
		-- Scheme Information
        'Direct registrant' AS SchemeName,
        NULL AS ApprovalNumber,
        -- Producer Information
        o.[TradingName] AS 'TradingName',
        o.[Name] AS 'CompanyName',
		CASE 
			WHEN o.OrganisationType = 1 THEN 'Registered company'
			WHEN o.OrganisationType IN (2,4) THEN 'Partnership'
			WHEN o.OrganisationType = 3 THEN 'Sole trader or individual'
		END AS 'ProducerType',
        o.[Name] AS 'ProducerName',
        rp.ProducerRegistrationNumber AS 'PRN',
        firstSubmitted.SubmittedDate AS 'DateRegistered',
        dpsh.SubmittedDate AS 'DateAmended',
        
        -- Business Details
        NULL AS 'SICCODES',
        NULL AS VATRegistered,
        NULL AS AnnualTurnover,
        NULL AS 'AnnualTurnoverBandType',
		'Less than 5T EEE placed on market' AS 'EEEPlacedOnMarketBandType',
         dbo.GetObligationType(eorv.Id) AS 'ObligationType',
        NULL 'ChargeBandType',
        CASE dpsh.SellingTechniqueType
            WHEN 0 THEN 'Direct Selling to End User'
            WHEN 1 THEN 'Indirect Selling to End User'
            WHEN 2 THEN 'Both'
            ELSE ''
        END AS 'SellingTechniqueType',
        NULL AS CeaseToExist,
        
        -- Correspondence for Notices Contact Details
        NULL as 'CNTitle',
        NULL as 'CNForename',
        NULL as 'CNSurname',
        serviceAddress.Telephone as 'CNTelephone',
        NULL as 'CNMobile',
        NULL as 'CNFax',
        serviceAddress.Email as 'CNEmail',
        serviceAddress.Address1 as 'CNPrimaryName',
        NULL as 'CNSecondaryName',
        serviceAddress.Address2 as 'CNStreet',
        serviceAddress.TownOrCity as 'CNTown',
        serviceAddress.CountyOrRegion as 'CNLocality',
        NULL as 'CNAdministrativeArea',
        serviceAddress.PostCode as 'CNPostcode',
        serviceAddressCountry.Name as 'CNCountry',
   
        -- Registered Office Details
        o.CompanyRegistrationNumber AS 'CompanyNumber',
        NULL as 'CompanyContactTitle',
        oc.FirstName as 'CompanyContactForename',
        oc.LastName as 'CompanyContactSurname',
        oa.Telephone as 'CompanyContactTelephone',
        NULL as 'CompanyContactMobile',
        NULL as 'CompanyContactFax',
        oa.Email as 'CompanyContactEmail',
        oa.Address1 as 'CompanyContactPrimaryName',
        NULL as 'CompanyContactSecondaryName',
        oa.Address2 as 'CompanyContactStreet',
        oa.TownOrCity as 'CompanyContactTown',
        oa.CountyOrRegion as 'CompanyContactLocality',
        NULL as 'CompanyContactAdministrativeArea',
        oa.PostCode as 'CompanyContactPostcode',
        loc.Name as 'CompanyContactCountry',
   
        -- Principal Place of Business Details
        additionalDetails.AllFullNames AS 'Partners',
        NULL as 'PPOBContactTitle',
        NULL as 'PPOBContactForename',
        NULL as 'PPOBContactSurname',
        NULL as 'PPOBContactTelephone',
        NULL as 'PPOBContactMobile',
        NULL as 'PPOBContactFax',
        NULL as 'PPOBContactEmail',
        NULL as 'PPOBContactPrimaryName',
        NULL as 'PPOBContactSecondaryName',
        NULL as 'PPOBContactStreet',
        NULL as 'PPOBContactTown',
        NULL as 'PPOBContactLocality',
        NULL as 'PPOBContactAdministrativeArea',
        NULL as 'PPOBContactPostcode',
        NULL as 'PPOBContactCountry',
      
        -- Overseas Contact Details / Auth rep
        ap.OverseasProducerName as 'OverseasProducerName',
        pc.Title as 'OverseasContactTitle',
        pc.Forename as 'OverseasContactForename',
        pc.Surname as 'OverseasContactSurname',
        pc.Telephone as 'OverseasContactTelephone',
        pc.Mobile as 'OverseasContactMobile',
        pc.Fax as 'OverseasContactFax',
        pc.Email as 'OverseasContactEmail',
        pa.PrimaryName as 'OverseasContactPrimaryName',
        pa.SecondaryName as 'OverseasContactSecondaryName',
        pa.Street as 'OverseasContactStreet',
        pa.Town as 'OverseasContactTown',
        pa.Locality as 'OverseasContactLocality',
        pa.AdministrativeArea as 'OverseasContactAdministrativeArea',
        pa.PostCode as 'OverseasContactPostcode',
        ac.Name as 'OverseasContactCountry',
        
        -- Removal Status
        CASE RP.Removed
            WHEN 1 THEN 'Yes'
            WHEN 0 THEN 'No'
        END AS 'RemovedFromScheme',

        -- Brand Names (if requested)
        CASE @IncludeBrandNames
            WHEN 1 THEN 
                ISNULL(NULLIF(
                    (SELECT STUFF((SELECT '; ' + BN.Name
                    FROM 
                    [Producer].[DirectRegistrant] drb
                    INNER JOIN [Producer].[BrandName] bn ON bn.Id = drb.BrandNameId
                    WHERE drb.Id = dr.Id
                    FOR XML PATH(''), TYPE)
                    .value('.', 'NVARCHAR(MAX)') 
                    , 1, 2, '')), ''), '')
            WHEN 0 THEN ''
        END AS 'BrandNames',
		1 AS IsDirectProducer

	FROM
        [Producer].[DirectProducerSubmission] dps
        INNER JOIN [Producer].[DirectRegistrant] dr ON dr.Id = dps.DirectRegistrantId
        INNER JOIN [Organisation].[Organisation] o ON o.Id = dr.OrganisationId
        INNER JOIN [Organisation].[Address] oa ON oa.Id = o.BusinessAddressId
		INNER JOIN [Organisation].[Contact] oc ON oc.Id = dr.ContactId
        INNER JOIN [Lookup].[Country] loc ON loc.Id = oa.CountryId
        INNER JOIN [Producer].[RegisteredProducer] rp ON dps.RegisteredProducerId = rp.Id AND dps.ComplianceYear = @ComplianceYear
        INNER JOIN (
                    SELECT 
                        dpsh.Id,
                        dpsh.DirectProducerSubmissionId,
                        ROW_NUMBER() OVER (PARTITION BY dpsh.DirectProducerSubmissionId ORDER BY dpsh.SubmittedDate DESC) as rn
                    FROM 
                        [Producer].[DirectProducerSubmissionHistory] dpsh
                        INNER JOIN [Producer].[DirectProducerSubmission] ps ON dpsh.DirectProducerSubmissionId = ps.Id
                        INNER JOIN [Producer].[RegisteredProducer] rp ON ps.RegisteredProducerId = rp.Id
                    WHERE 
                        dpsh.SubmittedDate IS NOT NULL
                        AND (@IncludeRemovedProducer = 1 OR RP.Removed = 0)
                    ) latest_submission ON latest_submission.DirectProducerSubmissionId = dps.Id AND latest_submission.rn = 1
		INNER JOIN [Producer].[DirectProducerSubmissionHistory] dpsh ON dpsh.Id = latest_submission.Id
		LEFT JOIN [Organisation].[Address] serviceAddress ON serviceAddress.Id = dpsh.ServiceOfNoticeAddressId
		LEFT JOIN [Lookup].[Country] serviceAddressCountry ON serviceAddressCountry.Id = serviceAddress.CountryId
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
                AND (@IncludeRemovedProducer = 1 OR RP.Removed = 0)
            GROUP BY
                ps.RegisteredProducerId
		) firstSubmitted ON dps.RegisteredProducerId = firstSubmitted.RegisteredProducerId
		LEFT JOIN 
		(
			 SELECT
                DirectRegistrantId,
                STUFF((
                    SELECT '; ' + FirstName + ' ' + LastName
                    FROM [Organisation].AdditionalCompanyDetails acd2
                    WHERE acd2.DirectRegistrantId = acd1.DirectRegistrantId
                    FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS AllFullNames
            FROM
                [Organisation].AdditionalCompanyDetails acd1
            GROUP BY
                DirectRegistrantId
		) additionalDetails ON dr.Id = additionalDetails.DirectRegistrantId
    WHERE
        (@IncludeRemovedProducer = 1 OR rp.Removed = 0) AND
        dps.ComplianceYear = @ComplianceYear AND
		@FilterBySchemes = 0
		
    ORDER BY
		IsDirectProducer,
        S.SchemeName,
        ProducerName;
END

