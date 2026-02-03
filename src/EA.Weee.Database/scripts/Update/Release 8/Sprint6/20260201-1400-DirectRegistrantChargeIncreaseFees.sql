/*
 * Add Direct Registrant charges for April 2026 fee increase
 * Fee Changes (effective April 1, 2026):
 * - England + Non-UK: £32 → £33 (3.8% increase)
 * - Scotland/Wales/Northern Ireland: £30 (no change)
 * - In the future we just need to create new records for the new compliance year onwards
 */

SET NOCOUNT ON;
SET XACT_ABORT ON;


BEGIN TRANSACTION;

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
        (NEWID(), 2026, '2026-04-01', 33.00, 1);
    PRINT N'Added England + Non-UK April 2026 fee: £33.00';
END
ELSE
BEGIN
    PRINT N'England + Non-UK April 2026 fee record already exists';
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
        (NEWID(), 2026, '2026-04-01', 30.00, 0);
    PRINT N'Added Scotland/Wales/Northern Ireland April 2026 fee: £30.00';
END
ELSE
BEGIN
    PRINT N'Scotland/Wales/Northern Ireland April 2026 fee record already exists';
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
    PRINT N'Added primary key constraint on DirectRegistrantCharge.Id';
END
ELSE
BEGIN
    PRINT N'Primary key constraint already exists';
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
    PRINT N'Created performance index: IX_DirectRegistrantCharge_ComplianceYear_IsNonUk_EffectiveFrom';
END
ELSE
BEGIN
    PRINT N'Performance index already exists';
END

COMMIT TRANSACTION;
GO

-- Verification query
PRINT N'';
PRINT N'=== Verification: All Direct Registrant charges for 2026 ===';
GO

SELECT 
    [Id],
    [ComplianceYear],
    CONVERT(VARCHAR(10), [EffectiveFrom], 120) AS [EffectiveFrom],
    [ChargeAmount],
    CASE [IsNonUk]
        WHEN 0 THEN 'Scotland/Wales/Northern Ireland'
        WHEN 1 THEN 'England + Non-UK'
        ELSE 'Unknown'
    END AS [AppliesTo],
    [IsNonUk]
FROM [Lookup].[DirectRegistrantCharge]
WHERE [ComplianceYear] = 2026
ORDER BY [EffectiveFrom] ASC, [IsNonUk] ASC;
GO

PRINT N'=== April 2026 Direct Registrant fee records added successfully ===';
GO
