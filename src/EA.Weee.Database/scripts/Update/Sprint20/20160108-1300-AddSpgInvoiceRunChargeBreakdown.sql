SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Charging].[SpgInvoiceRunChargeBreakdown]
@InvoiceRunId UNIQUEIDENTIFIER
AS
BEGIN

SET NOCOUNT ON

SELECT
  S.[SchemeName],
  M.[ComplianceYear],
  M.[SubmittedDate],
  CASE 
    WHEN PBC.Name IS NULL THEN PBP.Name
    ELSE PBC.NAME
  END AS 'ProducerName',
  RP.[ProducerRegistrationNumber] AS 'PRN',
  P.[ChargeThisUpdate] as 'ChargeValue',
  CASE C.ChargeBand
    WHEN 0 THEN 'A'
    WHEN 1 THEN 'B'
    WHEN 2 THEN 'C'
    WHEN 3 THEN 'D'
    WHEN 4 THEN 'E'
    ELSE ''
  END AS 'ChargeBandType'
FROM [Producer].[ProducerSubmission] P
  JOIN [PCS].[MemberUpload] M ON P.MemberUploadId = M.Id
  JOIN [Lookup].[ChargeBandAmount] C ON P.ChargeBandAmountId = C.Id
  JOIN [Producer].[Business] PB ON P.ProducerBusinessId = PB.Id
  LEFT JOIN [Producer].[Partnership] PBP ON PB.PartnershipId = PBP.Id
  LEFT JOIN [Producer].[Company] PBC ON PB.CompanyId = PBC.Id
  JOIN [Producer].[RegisteredProducer] RP ON P.RegisteredProducerId = RP.Id
  JOIN [PCS].[Scheme] S ON RP.SchemeId = S.Id
WHERE
    M.InvoiceRunId = @InvoiceRunId
  AND
    P.Invoiced = 1
  AND
    P.ChargeThisUpdate > 0
ORDER BY 
  S.[SchemeName] ASC,
  M.[ComplianceYear] DESC,
  M.[SubmittedDate] ASC

END
GO