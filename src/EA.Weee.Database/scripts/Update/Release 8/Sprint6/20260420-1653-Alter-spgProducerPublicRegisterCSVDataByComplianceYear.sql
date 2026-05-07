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
		WHEN PBC.[Name] IS NULL THEN PBP.[Name]
		ELSE PBC.[Name]
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
		WHEN ORG.[Name] IS NULL THEN ORG.TradingName
		ELSE ORG.[Name]
	END AS 'SchemeOperator',

	-- Compliance Scheme Registered Office
	ORG_A.Address1 AS 'CSROAAddress1',
	ORG_A.Address2 AS 'CSROAAddress2',
	ORG_A.TownOrCity AS 'CSROATownOrCity',
	ORG_A.CountyOrRegion AS 'CSROACountyOrRegion',
	ORG_A.Postcode AS 'CSROAPostcode',
	ORG_A_C.[Name] AS 'CSROACountry',

	-- Overseas Producer Details
	AR.OverseasProducerName AS 'OPNAName',
	OC_A.PrimaryName AS 'OPNAPrimaryName',
	OC_A.SecondaryName AS 'OPNASecondaryName',
	OC_A.Street AS 'OPNAStreet',
	OC_A.Town AS 'OPNATown',
	OC_A.Locality AS 'OPNALocality',
	OC_A.AdministrativeArea AS 'OPNAAdministrativeArea', 
	OC_A_C.[Name] AS 'OPNACountry',
	OC_A.PostCode AS 'OPNAPostCode',

	MU.ComplianceYear,

	-- Principal Place of Business
	PPOB_A.PrimaryName AS 'PPOBPrimaryName',
	PPOB_A.SecondaryName AS 'PPOBSecondaryName',
	PPOB_A.Street AS 'PPOBStreet',
	PPOB_A.Town AS 'PPOBTown',
	PPOB_A.Locality AS 'PPOBLocality',
	PPOB_A.AdministrativeArea AS 'PPOBAdministrativeArea',
	PPOB_A_C.[Name] AS 'PPOBCountry',
	PPOB_A.PostCode AS 'PPOBPostcode',
	0 AS IsDirectProducer

FROM Producer.RegisteredProducer RP
INNER JOIN Producer.ProducerSubmission PS ON RP.CurrentSubmissionId = PS.Id
INNER JOIN PCS.MemberUpload MU ON PS.MemberUploadId = MU.Id
INNER JOIN Pcs.Scheme S ON MU.SchemeId = S.Id
INNER JOIN Organisation.Organisation ORG ON ORG.Id = S.OrganisationId
INNER JOIN [Organisation].[Address] ORG_A ON ORG.BusinessAddressId = ORG_A.Id
INNER JOIN [Lookup].[Country] ORG_A_C ON ORG_A.CountryId = ORG_A_C.Id

-- Authorized Representative
LEFT JOIN Producer.AuthorisedRepresentative AR ON PS.AuthorisedRepresentativeId = AR.Id
LEFT JOIN Producer.Contact OC ON AR.OverseasContactId = OC.Id
LEFT JOIN [Producer].[Address] OC_A ON OC.AddressId = OC_A.Id
LEFT JOIN [Lookup].[Country] OC_A_C ON OC_A.CountryId = OC_A_C.Id

-- Producer Business
LEFT JOIN Producer.Business PB ON PS.ProducerBusinessId = PB.Id
LEFT JOIN Producer.Contact CFNC ON PB.CorrespondentForNoticesContactId = CFNC.Id
LEFT JOIN Producer.[Address] CFNC_A ON CFNC.AddressId = CFNC_A.Id
LEFT JOIN [Lookup].[Country] CFNC_A_C ON CFNC_A.CountryId = CFNC_A_C.Id

-- Producer Company
LEFT JOIN Producer.Company PBC ON PB.CompanyId = PBC.Id
LEFT JOIN Producer.Contact ROC ON PBC.RegisteredOfficeContactId = ROC.Id
LEFT JOIN Producer.[Address] ROC_A ON ROC.AddressId = ROC_A.Id
LEFT JOIN [Lookup].[Country] ROC_A_C ON ROC_A.CountryId = ROC_A_C.Id

-- Producer Partnership
LEFT JOIN Producer.Partnership PBP ON PB.PartnershipId = PBP.Id
LEFT JOIN Producer.Contact PPOB ON PBP.PrincipalPlaceOfBusinessId = PPOB.Id
LEFT JOIN [Producer].[Address] PPOB_A ON PPOB.AddressId = PPOB_A.Id
LEFT JOIN [Lookup].[Country] PPOB_A_C ON PPOB_A.CountryId = PPOB_A_C.Id

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
		SELECT DISTINCT '; ' + PP.[Name]
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
		O.[Name] AS 'CompanyName',
		O.[Name] AS 'ProducerName',
		O.[TradingName] AS 'TradingName',
		dbo.GetObligationType(eorv.Id) AS 'ObligationType',

		-- Registered Office Details
		OA.Address1 AS 'ROAPrimaryName',
		NULL AS 'ROASecondaryName',
		OA.Address2 AS 'ROAStreet',
		OA.TownOrCity AS 'ROATown',
		OA.CountyOrRegion AS 'ROALocality',
		NULL AS 'ROAAdministrativeArea',
		OA.PostCode AS 'ROAPostCode',
		LOC.[Name] AS 'ROACountry',

		-- Registered Office Contact Details
		CASE
			WHEN O.OrganisationType = 1 THEN OA.Telephone
			ELSE NULL
		END AS 'ROATelephone',
		CASE
			WHEN O.OrganisationType = 1 THEN OA.Email
			ELSE NULL
		END AS 'ROAEmail',
		CASE
			WHEN O.OrganisationType = 1 THEN OA.Fax
			ELSE NULL
		END AS 'ROAFax',

		RP.ProducerRegistrationNumber AS 'PRN',
		'Direct registrant' AS SchemeName,
		NULL AS 'SchemeOperator',
		NULL AS 'CSROAAddress1',
		NULL AS 'CSROAAddress2',
		NULL AS 'CSROATownOrCity',
		NULL AS 'CSROACountyOrRegion',
		NULL AS 'CSROAPostcode',
		NULL AS 'CSROACountry',
		AP.OverseasProducerName AS 'OPNAName',
		PA.PrimaryName AS 'OPNAPrimaryName',
		PA.SecondaryName AS 'OPNASecondaryName',
		PA.Street AS 'OPNAStreet',
		PA.Town AS 'OPNATown',
		PA.Locality AS 'OPNALocality',
		PA.AdministrativeArea AS 'OPNAAdministrativeArea',
		AC.[Name] AS 'OPNACountry',
		PA.PostCode AS 'OPNAPostCode',
		DPS.ComplianceYear,
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
		[Producer].[DirectProducerSubmission] DPS
		INNER JOIN [Producer].[DirectRegistrant] DR ON DR.Id = DPS.DirectRegistrantId
		INNER JOIN [Organisation].[Organisation] O ON O.Id = DR.OrganisationId
		INNER JOIN [Organisation].[Address] OA ON OA.Id = O.BusinessAddressId
		INNER JOIN [Organisation].[Contact] OC ON OC.Id = DR.ContactId
		INNER JOIN [Lookup].[Country] LOC ON LOC.Id = OA.CountryId
		INNER JOIN [Producer].[RegisteredProducer] RP ON DPS.RegisteredProducerId = RP.Id AND DPS.ComplianceYear = @ComplianceYear
		INNER JOIN (
			SELECT 
				DirectProducerSubmissionId,
				EeeOutputReturnVersionId,
				Id,
				ROW_NUMBER() OVER (PARTITION BY DirectProducerSubmissionId ORDER BY SubmittedDate DESC) AS RowNum
			FROM [Producer].[DirectProducerSubmissionHistory]
			WHERE SubmittedDate IS NOT NULL
		) DPSH ON DPSH.DirectProducerSubmissionId = DPS.Id AND DPSH.RowNum = 1
		LEFT JOIN [PCS].[EeeOutputReturnVersion] EORV ON EORV.Id = DPSH.EeeOutputReturnVersionId
		LEFT JOIN [Producer].[AuthorisedRepresentative] AP ON AP.Id = DR.AuthorisedRepresentativeId
		LEFT JOIN [Producer].[Contact] PC ON PC.Id = AP.OverseasContactId
		LEFT JOIN [Producer].[Address] PA ON PA.Id = PC.AddressId
		LEFT JOIN [Lookup].[Country] AC ON AC.Id = PA.CountryId
		INNER JOIN
		(
			-- Subquery to get the first submission date for each producer
			SELECT
				PS.RegisteredProducerId,
				MIN(DPSH.SubmittedDate) AS SubmittedDate
			FROM
				[Producer].[DirectProducerSubmission] PS
			INNER JOIN [Producer].[DirectProducerSubmissionHistory] DPSH ON DPSH.DirectProducerSubmissionId = PS.Id
			INNER JOIN [Producer].[RegisteredProducer] RP ON PS.RegisteredProducerId = RP.Id
			WHERE
				DPSH.SubmittedDate IS NOT NULL AND (RP.Removed = 0)
			GROUP BY
				PS.RegisteredProducerId
		) firstSubmitted ON DPS.RegisteredProducerId = firstSubmitted.RegisteredProducerId
WHERE
	DPS.ComplianceYear = @ComplianceYear AND
	RP.Removed = 0 AND
	DPS.[Status] IN (2, 3) AND -- ('Submitted', 'Returned')
	O.OrganisationType = 1 -- Registered company

ORDER BY
	IsDirectProducer,
	S.SchemeName,
	ProducerName
END