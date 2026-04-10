-- Fixes for EA annual subsistence charge for compliance year 2027

SET NOCOUNT ON;

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
