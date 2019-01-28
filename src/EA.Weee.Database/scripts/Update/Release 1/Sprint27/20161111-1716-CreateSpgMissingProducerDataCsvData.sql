SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Creation date: 2016 Nov 11
-- Description:	This stored procedure is used to provide the data for the admin report of data
--				for all registered producers that have not yet submitted a data return within
--				the limits of the specified parameters.
--				The stored procedure actually returns all the data that HAS been submitted and
--				will be used by the application to construct those that have not.
-- =============================================
CREATE PROCEDURE [Producer].[spgMissingProducerDataCsvData]
	@ComplianceYear INT,
	@ObligationType NVARCHAR(4),
	@Quarter INT,
	@SchemeId uniqueidentifier
AS
BEGIN

SELECT 
	S.SchemeName,
	S.ApprovalNumber,
	COALESCE(PBC.Name, PBP.Name, '') AS 'ProducerName',
	RP.ProducerRegistrationNumber as 'PRN',
	PS.ObligationType,
	QuarterType.Quarter,
	P_First.SubmittedDate AS 'DateRegistered'
FROM [Producer].[RegisteredProducer] RP
	INNER JOIN [PCS].Scheme S ON RP.SchemeId = S.Id
	INNER JOIN [Producer].[ProducerSubmission] PS ON RP.[CurrentSubmissionId] = PS.[Id]
	INNER JOIN [Producer].[Business] PB ON PS.ProducerBusinessId = PB.Id
		LEFT OUTER JOIN [Producer].[Company] PBC ON PB.CompanyId = PBC.Id
		LEFT OUTER JOIN [Producer].[Partnership] PBP ON PB.PartnershipId = PBP.Id
	INNER JOIN
	(
		SELECT
			MU.ComplianceYear,
			PS.RegisteredProducerId,
			MU.SubmittedDate,
			ROW_NUMBER() OVER
			(
				PARTITION BY PS.RegisteredProducerId
				ORDER BY PS.UpdatedDate
			) AS RowNumber
		FROM [Producer].[ProducerSubmission] PS
			INNER JOIN [PCS].[MemberUpload] MU ON PS.MemberUploadId = MU.Id
			INNER JOIN [Producer].[RegisteredProducer] RP ON PS.RegisteredProducerId = RP.Id
		WHERE MU.IsSubmitted = 1
		    AND RP.Removed = 0
	) AS P_First 
		ON PS.RegisteredProducerId = P_First.RegisteredProducerId
		AND P_First.RowNumber = 1
	LEFT OUTER JOIN (
		-- Inner query returns all quarters that have submitted data for the obligation type
		SELECT
			RP.Id AS 'RegProdId',
			DR.[Quarter]
		FROM PCS.DataReturn DR
			INNER JOIN [PCS].DataReturnVersion DRV ON DR.CurrentDataReturnVersionId = DRV.Id
			INNER JOIN [PCS].EeeOutputReturnVersionAmount EORVA ON DRV.EeeOutputReturnVersionId = EORVA.EeeOutputReturnVersionId
			INNER JOIN [PCS].EeeOutputAmount EOA ON EORVA.EeeOuputAmountId = EOA.Id
			INNER JOIN [Producer].[RegisteredProducer] RP ON EOA.RegisteredProducerId = RP.Id
		WHERE
			DR.ComplianceYear = @ComplianceYear
			AND EOA.ObligationType = @ObligationType
			AND DR.[Quarter] = COALESCE(@Quarter, DR.[Quarter])
		GROUP BY RP.Id, DR.[Quarter]
	) AS QuarterType ON QuarterType.RegProdId = RP.Id
WHERE RP.ComplianceYear = @ComplianceYear
	AND	RP.Removed = 0
	AND S.Id = COALESCE(@SchemeId, S.Id)
	AND S.SchemeStatus <> 3
	AND (PS.ObligationType = 'Both' 
		OR PS.ObligationType = @ObligationType)
ORDER BY S.SchemeName, ProducerName, QuarterType.Quarter
END
GO