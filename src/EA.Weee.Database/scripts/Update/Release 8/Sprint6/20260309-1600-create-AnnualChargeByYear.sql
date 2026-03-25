/*	=====================================================
	2027 Inflationary Charge Uplift - PCS Subsistence Fee
	===================================================== */

DECLARE @EAId UNIQUEIDENTIFIER = '184E1785-26B4-4AE4-80D3-AE319B103ACB';
DECLARE @SEPAId UNIQUEIDENTIFIER = '4209EE95-0882-42F2-9A5D-355B4D89EF30';
DECLARE @NIEAId UNIQUEIDENTIFIER = '7BFB8717-4226-40F3-BC51-B16FDF42550C';
DECLARE @NRWId UNIQUEIDENTIFIER = 'DB83F5AB-E745-49CF-B2CA-23FE391B67A8';

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