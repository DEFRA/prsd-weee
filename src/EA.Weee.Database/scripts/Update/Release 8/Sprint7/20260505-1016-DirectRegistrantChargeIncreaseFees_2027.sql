-- =========================================================================
-- Author:		Sreedhar Bangarugari
-- Create date:	05-05-2026
-- Description:	Add Direct Registrant charges for January 2027 fee increase
--				Fee Changes (effective January 1, 2027):
--				England + Non-UK: £33.48 (no change)
--				Scotland/Wales/Northern Ireland: £30.00 (no change)
--				In the future we just need to create new records for the new compliance year onwards
-- =========================================================================

DECLARE @complianceYear INT = 2027
DECLARE @effectiveFrom NVARCHAR(10) = '2027-01-01'
DECLARE @UKPrice DECIMAL(5, 2) = 33.48
DECLARE @NonUKPrice DECIMAL(5, 2) = 30.00

-- England + Non-UK - £33.48
IF NOT EXISTS (SELECT 1 FROM [Lookup].[DirectRegistrantCharge] WHERE [ComplianceYear] = @complianceYear AND [EffectiveFrom] = @effectiveFrom AND [IsNonUk] = 1)
BEGIN
	INSERT INTO [Lookup].[DirectRegistrantCharge]
		([Id], [ComplianceYear], [EffectiveFrom], [ChargeAmount], [IsNonUk])
	VALUES
		('33391FC6-DDB0-49D8-A75E-13537EBE0069', @complianceYear, @effectiveFrom, @UKPrice, 1);
END

-- Scotland + Wales + Northern Ireland - £30.00
IF NOT EXISTS (SELECT 1 FROM [Lookup].[DirectRegistrantCharge] WHERE [ComplianceYear] = @complianceYear AND [EffectiveFrom] = @effectiveFrom AND [IsNonUk] = 0)
BEGIN
	INSERT INTO [Lookup].[DirectRegistrantCharge]
		([Id], [ComplianceYear], [EffectiveFrom], [ChargeAmount], [IsNonUk])
	VALUES
		('A52F5C35-67D6-4DFC-93B9-F7B0653853CF', @complianceYear, @effectiveFrom, @NonUKPrice, 0);
END