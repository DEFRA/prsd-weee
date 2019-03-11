  INSERT INTO Lookup.ChargeBandAmount
  (Id, Amount, ChargeBand)
  VALUES('DC8545CD-0C38-40B8-87CF-30AFF1990742', 750, 5)
  GO
  INSERT INTO Lookup.ChargeBandAmount
  (Id, Amount, ChargeBand)
  VALUES('A8B0DB11-4158-4607-BED4-D476CCCCEB06', 100,6)
  GO
   INSERT INTO Lookup.ChargeBandAmount
  (Id, Amount, ChargeBand)
  VALUES('D8B5CD20-84AE-4495-A012-3994D8DB921C',100,7)
  GO
  INSERT INTO Lookup.ChargeBandAmount
  (Id, Amount, ChargeBand)
  VALUES('7D196466-4DE5-47B4-9D27-1CD3D09571C7',375,8)
  GO

  /****** Object:  StoredProcedure [Charging].[SpgInvoiceRunChargeBreakdown]    Script Date: 10/03/2019 17:14:49 ******/
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
  S.[SchemeName],
  M.[ComplianceYear],
  M.[SubmittedDate] as 'SubmissionDate',
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
	WHEN 5 THEN 'A2'
	WHEN 6 THEN 'C2'
	WHEN 7 THEN 'D2'
	WHEN 8 THEN 'D3'
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
