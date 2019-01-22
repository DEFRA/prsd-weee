 /* 
  * This script moves data from the producer table into the new registered producer and
  * producer submission tables.
  *
  * There is a 1-to-1 mapping from producer to producer submission. All rows will
  * retain their existing IDs.
  *
  * The registered producer table will be populated by grouping producers by registration
  * number, compliance year and producer compliance scheme.
  *
  * Once the two tables are populated, the current submission ID will be populated
  * for each complete registration, i.e. where the producer was marked as current.
  *
  * Note: Some developer environments have producers with an obligation type of 0.
  * This does not map to the current values defined for the obligation type enumeration
  * so this script maps it to an arbitrary value of 'B2B.
  */

-- Group the existing producers to populate the registered producer table.
INSERT INTO [Producer].[RegisteredProducer]
(
	[Id],
	[ProducerRegistrationNumber],
	[SchemeId],
	[ComplianceYear],
	[CurrentSubmissionId]
)
SELECT
	NEWID(),
	P.[RegistrationNumber],
	MU.[SchemeId],
	MU.[ComplianceYear],
	NULL -- CurrentSubmissionId will be populated once the [ProducerSubmission] table has been populated (see below).
FROM
	[Producer].[Producer] P
INNER JOIN
	[PCS].[MemberUpload] MU
		ON P.[MemberUploadId] = MU.[Id]
GROUP BY
	P.[RegistrationNumber],
	MU.[SchemeId],
	MU.[ComplianceYear]

-- Copy the existing producers to the producer submission table, linking to the relevant registered producer.
INSERT INTO [Producer].[ProducerSubmission]
(
	[Id],
	[UpdatedDate],
	[RegisteredProducerId],
	[ObligationType],
	[TradingName],
	[VATRegistered],
	[EEEPlacedOnMarketBandType],
	[SellingTechniqueType],
	[MemberUploadId],
	[AuthorisedRepresentativeId],
	[ProducerBusinessId],
	[AnnualTurnover],
	[AnnualTurnoverBandType],
	[ChargeBandAmountId],
	[ChargeThisUpdate],
	[CeaseToExist]
)
SELECT
	P.[Id],
	P.[UpdatedDate],
	RP.[Id],
	CASE P.[ObligationType]
		WHEN 1 THEN 'B2B'
		WHEN 2 THEN 'B2C'
		WHEN 3 THEN 'Both'
		ELSE 'B2B'
	END,
	P.[TradingName],
	P.[VATRegistered],
	P.[EEEPlacedOnMarketBandType],
	P.[SellingTechniqueType],
	P.[MemberUploadId],
	P.[AuthorisedRepresentativeId],
	P.[ProducerBusinessId],
	P.[AnnualTurnover],
	P.[AnnualTurnoverBandType],
	P.[ChargeBandAmountId],
	P.[ChargeThisUpdate],
	P.[CeaseToExist]
FROM
	[Producer].[Producer] P
INNER JOIN
	[PCS].[MemberUpload] MU
		ON P.[MemberUploadId] = MU.[Id]
INNER JOIN
	[Producer].[RegisteredProducer] RP
		ON P.[RegistrationNumber] = RP.[ProducerRegistrationNumber]
		AND MU.[SchemeId] = RP.[SchemeId]
		AND MU.[ComplianceYear] = RP.[ComplianceYear]

-- Set the current producer submission for each registered producer with a submitted submission.
UPDATE
	[Producer].[RegisteredProducer]
SET
	[CurrentSubmissionId] = PS.[Id]
FROM
	[Producer].[RegisteredProducer] RP
INNER JOIN
	[Producer].[ProducerSubmission] PS
		INNER JOIN
			[Producer].[Producer] P
				ON PS.[Id] = P.[Id]
		ON RP.[Id] = PS.[RegisteredProducerId]
		AND P.[IsCurrentForComplianceYear] = 1

GO