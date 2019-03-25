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
           S.[SchemeName]
         , M.[ComplianceYear]
         , M.[SubmittedDate] as 'SubmissionDate'
         , CASE
               WHEN PBC.Name IS NULL
               THEN PBP.Name
           ELSE PBC.NAME
           END AS 'ProducerName'
         , RP.[ProducerRegistrationNumber] AS 'PRN'
         , P.[ChargeThisUpdate] AS 'ChargeValue'
         , CASE C.ChargeBand
              WHEN 0
                         THEN 'A'
              WHEN 1
                         THEN 'B'
              WHEN 2
                         THEN 'C'
              WHEN 3
                         THEN 'D'
              WHEN 4
                         THEN 'E'
              WHEN 5
                         THEN 'A2'
              WHEN 6
                         THEN 'C2'
              WHEN 7
                         THEN 'D2'
              WHEN 8
                         THEN 'D3'
			  WHEN 9
                         THEN 'NA'
                         ELSE ''
           END AS 'ChargeBandType',
          CASE 
             WHEN ROC_A_C.Name IS NOT NULL 
                        THEN ROC_A_C.Name 
             WHEN PPOB_A_C.Name IS NOT NULL 
                        THEN  PPOB_A_C.Name
			ELSE ''
         END RegOfficeOrPBoBCountry,
		 CASE
		 WHEN M.HasAnnualCharge = 1 
				THEN 'Yes'
		WHEN M.HasAnnualCharge = 0 
				THEN 'No'
		END  'HasAnnualCharge'
FROM
           [Producer].[ProducerSubmission] P
           JOIN
                      [PCS].[MemberUpload] M
           ON
                      P.MemberUploadId = M.Id
           JOIN
                      [Lookup].[ChargeBandAmount] C
           ON
                      P.ChargeBandAmountId = C.Id
           JOIN
                      [Producer].[Business] PB
           ON
                      P.ProducerBusinessId = PB.Id
           LEFT JOIN
                      [Producer].[Company] PBC
           ON
                      PB.CompanyId = PBC.Id
           LEFT JOIN
                      [Producer].[Contact] ROC
           LEFT JOIN
                      [Producer].[Address] ROC_A
           LEFT JOIN
                      [Lookup].[Country] ROC_A_C
           ON
                      ROC_A.CountryId = ROC_A_C.Id
           ON
                      ROC.AddressId = ROC_A.Id
           ON         PBC.RegisteredOfficeContactId = ROC.Id
           LEFT JOIN
                      [Producer].[Partnership] PBP
           ON
                      PB.PartnershipId = PBP.Id
          LEFT JOIN
                      [Producer].[Contact] PPOB
          LEFT JOIN
                      [Producer].[Address] PPOB_A
          LEFT JOIN
                      [Lookup].[Country] PPOB_A_C
          ON
                      PPOB_A.CountryId = PPOB_A_C.Id
          ON
                      PPOB.AddressId = PPOB_A.Id
          ON
                      PBP.PrincipalPlaceOfBusinessId = PPOB.Id
		LEFT JOIN 
					 [Lookup].[CompetentAuthority] LCA
		LEFT JOIN 
					 [Lookup].[Country] LC
		ON
					LCA.CountryId = LC.Id
		ON
					LCA.Id =  ROC_A_C.Id
        JOIN
                      [Producer].[RegisteredProducer] RP
         ON
                      P.RegisteredProducerId = RP.Id
         JOIN
                      [PCS].[Scheme] S
         ON
                      RP.SchemeId = S.Id
		LEFT JOIN 
					[Lookup].[CompetentAuthority] LCAS
		ON
					LCAS.Id = S.CompetentAuthorityId
        WHERE
                      M.InvoiceRunId = @InvoiceRunId
                      AND P.Invoiced = 1
                      AND P.ChargeThisUpdate > 0
       ORDER BY
                       S.[SchemeName] ASC
                     , M.[ComplianceYear] DESC
                     , M.[SubmittedDate] ASC

END
GO