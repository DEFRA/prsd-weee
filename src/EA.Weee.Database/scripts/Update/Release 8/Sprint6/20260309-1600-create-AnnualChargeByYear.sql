/*	=====================================================
	2027 Inflationary Charge Uplift - PCS Subsistence Fee
	===================================================== */

DECLARE @EAId UNIQUEIDENTIFIER = '3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8';
DECLARE @SEPAId UNIQUEIDENTIFIER = '78F37814-364B-4FAE-BEB5-DB0439CBF177';
DECLARE @NIEAId UNIQUEIDENTIFIER = '4EEE5942-01B2-4A4D-855A-34DEE1BBBF26';
DECLARE @NRWId UNIQUEIDENTIFIER = '44C2F368-AA66-48F0-BBC9-A0ED34AD0951';

DECLARE @Id1 UNIQUEIDENTIFIER = '8E5E9C81-6219-B50C-82CB-FF866969BC24';
DECLARE @Id2 UNIQUEIDENTIFIER = '2A527D51-EFF7-4789-B985-CD43C64F03B3';
DECLARE @Id3 UNIQUEIDENTIFIER = 'BC807E29-8B36-4825-A90F-8C0D29F5C44C';
DECLARE @Id4 UNIQUEIDENTIFIER = '3FC1A9A2-D977-4D81-981E-9FA5A564C945';

DECLARE @EffectiveFrom DATETIME = '2026-04-01';
DECLARE @ComplianceYear INT = 2027;

--EA
IF NOT EXISTS (SELECT 1 FROM [Lookup].[AnnualChargeByYear] WHERE [ComplianceYear] = @ComplianceYear AND [EffectiveFrom] = @EffectiveFrom AND [CompetentAuthorityId] = @EAId)
	BEGIN
		INSERT INTO [Lookup].[AnnualChargeByYear]
			([Id],[CompetentAuthorityId],[ComplianceYear],[AnnualChargeAmount],[EffectiveFrom])
		VALUES
			(@Id1, @EAId, @ComplianceYear, 13948.13, @EffectiveFrom)
	END

--SEPA
IF NOT EXISTS (SELECT 1 FROM [Lookup].[AnnualChargeByYear] WHERE [ComplianceYear] = @ComplianceYear AND [EffectiveFrom] = @EffectiveFrom AND [CompetentAuthorityId] = @SEPAId)
	BEGIN
		INSERT INTO [Lookup].[AnnualChargeByYear]
			([Id],[CompetentAuthorityId],[ComplianceYear],[AnnualChargeAmount],[EffectiveFrom])
		VALUES
			(@Id2, @SEPAId, @ComplianceYear, 0.00, @EffectiveFrom)
	END

--NIEA
IF NOT EXISTS (SELECT 1 FROM [Lookup].[AnnualChargeByYear] WHERE [ComplianceYear] = @ComplianceYear AND [EffectiveFrom] = @EffectiveFrom AND [CompetentAuthorityId] = @NIEAId)
	BEGIN
		INSERT INTO [Lookup].[AnnualChargeByYear]
			([Id],[CompetentAuthorityId],[ComplianceYear],[AnnualChargeAmount],[EffectiveFrom])
		VALUES
			(@Id3, @NIEAId, @ComplianceYear, 0.00, @EffectiveFrom)
	END

--NRW
IF NOT EXISTS (SELECT 1 FROM [Lookup].[AnnualChargeByYear] WHERE [ComplianceYear] = @ComplianceYear AND [EffectiveFrom] = @EffectiveFrom AND [CompetentAuthorityId] = @NRWId)
	BEGIN
		INSERT INTO [Lookup].[AnnualChargeByYear]
			([Id],[CompetentAuthorityId],[ComplianceYear],[AnnualChargeAmount],[EffectiveFrom])
		VALUES
			(@Id4, @NRWId, @ComplianceYear, 0.00, @EffectiveFrom)
	END
GO