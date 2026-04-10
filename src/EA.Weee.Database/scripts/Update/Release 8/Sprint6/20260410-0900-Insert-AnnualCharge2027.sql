-- EA annual subsistence charge uplift (3.8% inflation) effective from 1st April 2026.
-- New fee: £13,948.13 (up from £13,438.00)
-- Applies to compliance year 2026 for any new scheme registering after 1st April 2026,
-- and to compliance year 2027 onwards for all EA schemes.

SET NOCOUNT ON;

-- Insert the uplifted annual charge for EA for compliance year 2026 (effective from 1st April 2026)
-- This covers any new EA schemes approved for 2026 after the uplift date.
-- The existing 2026 row (£13,438, effective 2026-01-01) remains for schemes that already submitted before April.
IF NOT EXISTS (
    SELECT 1 FROM [Lookup].[AnnualChargeByYear]
    WHERE CompetentAuthorityId = 'A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8'
      AND ComplianceYear = 2026
      AND EffectiveFrom = '2026-04-01'
)
BEGIN
    INSERT INTO [Lookup].[AnnualChargeByYear] ([Id], [CompetentAuthorityId], [ComplianceYear], [AnnualChargeAmount], [EffectiveFrom])
    VALUES (
        'B7E3A1D2-8F4C-4E9A-B6D5-2C7F8A9E0B1D',
        'A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8',
        2026,
        13948.13,
        '2026-04-01'
    );
END

-- Insert the uplifted annual charge for EA for compliance year 2027
IF NOT EXISTS (
    SELECT 1 FROM [Lookup].[AnnualChargeByYear]
    WHERE CompetentAuthorityId = 'A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8'
      AND ComplianceYear = 2027
)
BEGIN
    INSERT INTO [Lookup].[AnnualChargeByYear] ([Id], [CompetentAuthorityId], [ComplianceYear], [AnnualChargeAmount], [EffectiveFrom])
    VALUES (
        '45F6DE40-5419-4104-8B87-98F6A870DAF3',
        'A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8',
        2027,
        13948.13,
        '2026-04-01'
    );
END

GO