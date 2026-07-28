/*
 * Add Direct Registrant charges for April 2026 fee increase
 * Fee Changes (effective April 1, 2026):
 * - England + Non-UK: £32 → £33 (3.8% increase)
 * - Scotland/Wales/Northern Ireland: £30 (no change)
 * - In the future we just need to create new records for the new compliance year onwards
 */

-- England + Non-UK (IsNonUk = 1) - £33
IF NOT EXISTS (
    SELECT 1 FROM [Lookup].[DirectRegistrantCharge]
    WHERE [ComplianceYear] = 2026 
    AND [EffectiveFrom] = '2026-04-01' 
    AND [IsNonUk] = 1
)
BEGIN
    INSERT INTO [Lookup].[DirectRegistrantCharge]
        ([Id], [ComplianceYear], [EffectiveFrom], [ChargeAmount], [IsNonUk])
    VALUES
        ('D24429EB-50A0-43AB-8D50-FDBC6085AE92', 2026, '2026-04-01', 33.00, 1);
END

-- Scotland + Wales + Northern Ireland (IsNonUk = 0) - £30
IF NOT EXISTS (
    SELECT 1 FROM [Lookup].[DirectRegistrantCharge]
    WHERE [ComplianceYear] = 2026 
    AND [EffectiveFrom] = '2026-04-01' 
    AND [IsNonUk] = 0
)
BEGIN
    INSERT INTO [Lookup].[DirectRegistrantCharge]
        ([Id], [ComplianceYear], [EffectiveFrom], [ChargeAmount], [IsNonUk])
    VALUES
        ('1BEE68EC-D70D-4578-AEA8-27047A9A122E', 2026, '2026-04-01', 30.00, 0);
END

-- Add primary key constraint if it doesn't exist
IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE object_id = OBJECT_ID('[Lookup].[DirectRegistrantCharge]')
      AND is_primary_key = 1
)
BEGIN
    ALTER TABLE [Lookup].[DirectRegistrantCharge]
    ADD CONSTRAINT [PK_DirectRegistrantCharge] PRIMARY KEY CLUSTERED ([Id] ASC);
END

-- Create performance index if not exists
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE [name] = 'IX_DirectRegistrantCharge_ComplianceYear_IsNonUk_EffectiveFrom' 
    AND [object_id] = OBJECT_ID('[Lookup].[DirectRegistrantCharge]')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IX_DirectRegistrantCharge_ComplianceYear_IsNonUk_EffectiveFrom]
    ON [Lookup].[DirectRegistrantCharge] (
        [ComplianceYear] ASC, 
        [IsNonUk] ASC,
        [EffectiveFrom] DESC
    )
    INCLUDE ([ChargeAmount])
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, 
          IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, 
          ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);
END
GO