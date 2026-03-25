/*	=====================================================
	2027 Inflationary Charge Uplift - PCS Subsistence Fee
	===================================================== */

DECLARE @EAId UNIQUEIDENTIFIER = 'A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8';
DECLARE @Id UNIQUEIDENTIFIER = '8E5E9C81-6219-B50C-82CB-FF866969BC24';
DECLARE @EffectiveFrom DATETIME = '2026-04-01';
DECLARE @ComplianceYear INT = 2027;

IF NOT EXISTS (
	SELECT 1 FROM [Lookup].[AnnualChargeByYear]
	WHERE [ComplianceYear] = @ComplianceYear AND
		  [EffectiveFrom] = @EffectiveFrom AND
		  [CompetentAuthorityId] = @EAId
)

INSERT INTO [Lookup].[AnnualChargeByYear]
	([Id],[CompetentAuthorityId],[ComplianceYear],[AnnualChargeAmount],[EffectiveFrom])
VALUES
	(@Id, @EAId, @ComplianceYear, 13948.13, @EffectiveFrom)

GO
