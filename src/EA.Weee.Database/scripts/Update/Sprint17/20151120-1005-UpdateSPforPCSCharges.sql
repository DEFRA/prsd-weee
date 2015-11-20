/****** Object:  StoredProcedure [Producer].[spgPCSChargesByComplianceYearAndAuthorisedAuthority]    Script Date: 16/11/2015 12:21:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Priety Mahajan
-- Create date: 16 Nov 2015
-- Modified date: 20 Nov 2015
-- Description:	Returns PCS charge breakdown currently registered
--				in the specified year and AA.
--				This data is suitable for populating the CSV file
--				which may be downloaded by AA to see the charges for a submission for all schemes in compliance year
-- =============================================
Alter PROCEDURE [Producer].[spgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority]
		@ComplianceYear INT,
		@CompetentAuthorityId uniqueidentifier = null
AS
BEGIN

	SET NOCOUNT ON;
	SELECT
	S.SchemeName,
	@ComplianceYear as 'ComplianceYear',

	CASE 
	when PBC.Name is null then PBP.Name
	else PBC.NAME
	end as 'ProducerName',

	 P.RegistrationNumber AS 'PRN',
	 
	 MU.Date AS 'SubmissionDate',

 	P.ChargeThisUpdate as 'ChargeValue',

     CASE CBA.ChargeBand
			WHEN 0 THEN 'A'
			WHEN 1 THEN 'B'
			WHEN 2 THEN 'C'
			WHEN 3 THEN 'D'
			WHEN 4 THEN 'E'
			ELSE ''
		END AS 'ChargeBandType'
		
	

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
	Producer.Business PB
		ON P.ProducerBusinessId = PB.Id
LEFT JOIN
	Producer.Company PBC
		ON PB.CompanyId = PBC.Id
LEFT JOIN
	Producer.Partnership PBP
		ON PB.PartnershipId = PBP.Id

WHERE
      MU.ComplianceYear = @ComplianceYear
AND
	 (@CompetentAuthorityId is null or  S.CompetentAuthorityId = @CompetentAuthorityId)
AND
      MU.IsSubmitted = 1
AND
	  P.ChargeThisUpdate > 0

ORDER BY
	S.SchemeName,
    MU.Date

END
