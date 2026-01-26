-- =====================================================================
-- Purpose: Implements the 2026 EA annual subsistence fee increase from
--          £12,500 to £13,438 (7.5% increase)
-- =====================================================================

SET NOCOUNT ON;

PRINT N'=== Creating AnnualChargeByYear Lookup Table ===';

-- Create new AnnualChargeByYear lookup table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Lookup].[AnnualChargeByYear]') AND type in (N'U'))
BEGIN
    PRINT N'Creating [Lookup].[AnnualChargeByYear] table...';
    
    CREATE TABLE [Lookup].[AnnualChargeByYear](
        [Id] [uniqueidentifier] NOT NULL,
        [RowVersion] [timestamp] NOT NULL,
        [CompetentAuthorityId] [uniqueidentifier] NOT NULL,
        [ComplianceYear] [int] NOT NULL,
        [AnnualChargeAmount] [decimal](18, 2) NOT NULL,
        [EffectiveFrom] [datetime2](0) NULL,
        CONSTRAINT [PK_AnnualChargeByYear] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_AnnualChargeByYear_CompetentAuthority] FOREIGN KEY([CompetentAuthorityId])
            REFERENCES [Lookup].[CompetentAuthority] ([Id]),
        CONSTRAINT [UQ_AnnualChargeByYear_CompetentAuthority_Year] UNIQUE ([CompetentAuthorityId], [ComplianceYear])
    );
    
    PRINT N'[Lookup].[AnnualChargeByYear] table created successfully.';
END
ELSE
BEGIN
    PRINT N'[Lookup].[AnnualChargeByYear] table already exists.';
END
GO

-- Create indexes for performance
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AnnualChargeByYear_Lookup' AND object_id = OBJECT_ID('[Lookup].[AnnualChargeByYear]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_AnnualChargeByYear_Lookup
    ON [Lookup].[AnnualChargeByYear] ([CompetentAuthorityId], [ComplianceYear], [EffectiveFrom] DESC)
    INCLUDE ([AnnualChargeAmount]);
    PRINT N'Created lookup index on [Lookup].[AnnualChargeByYear] table.';
END
GO

-- Use hardcoded GUIDs from 0004-InsertCompetentAuthorityData.sql
DECLARE @EAId UNIQUEIDENTIFIER = 'A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8';
DECLARE @SEPAId UNIQUEIDENTIFIER = '78F37814-364B-4FAE-BEB5-DB0439CBF177';
DECLARE @NIEAId UNIQUEIDENTIFIER = '4EEE5942-01B2-4A4D-855A-34DEE1BBBF26';
DECLARE @NRWId UNIQUEIDENTIFIER = '44C2F368-AA66-48F0-BBC9-A0ED34AD0951';

-- Check if CompetentAuthority records exist before inserting
IF EXISTS (SELECT 1 FROM [Lookup].[CompetentAuthority] WHERE Id = @EAId)
BEGIN
    PRINT N'Inserting annual charge data for EA (Environment Agency)...';

    -- Insert data for compliance years 2019-2026 for EA
    INSERT INTO [Lookup].[AnnualChargeByYear] ([Id], [CompetentAuthorityId], [ComplianceYear], [AnnualChargeAmount], [EffectiveFrom])
    SELECT * FROM (VALUES
        (NEWID(), @EAId, 2019, 12500.00, '2019-01-01'),
        (NEWID(), @EAId, 2020, 12500.00, '2020-01-01'),
        (NEWID(), @EAId, 2021, 12500.00, '2021-01-01'),
        (NEWID(), @EAId, 2022, 12500.00, '2022-01-01'),
        (NEWID(), @EAId, 2023, 12500.00, '2023-01-01'),
        (NEWID(), @EAId, 2024, 12500.00, '2024-01-01'),
        (NEWID(), @EAId, 2025, 12500.00, '2025-01-01'),
        -- New 2026 uplifted charge (7.5% increase = £938)
        (NEWID(), @EAId, 2026, 13438.00, '2026-01-01')
    ) AS NewData ([Id], [CompetentAuthorityId], [ComplianceYear], [AnnualChargeAmount], [EffectiveFrom])
    WHERE NOT EXISTS (
        SELECT 1 FROM [Lookup].[AnnualChargeByYear] 
        WHERE [CompetentAuthorityId] = @EAId 
        AND [ComplianceYear] = NewData.[ComplianceYear]
    );

    DECLARE @RowsInserted INT = @@ROWCOUNT;
    PRINT N'Inserted ' + CAST(@RowsInserted AS NVARCHAR(10)) + ' annual charge records for EA.';
    PRINT N'  2019-2025: £12,500.00';
    PRINT N'  2026: £13,438.00 (7.5% increase)';
END
ELSE
BEGIN
    PRINT N'WARNING: CompetentAuthority record for EA not found. Skipping EA annual charge data insertion.';
    PRINT N'         This data will be inserted by the Everytime script 0006-AnnualChargeByYearData.sql';
END

-- Check if other CompetentAuthority records exist before inserting
IF EXISTS (SELECT 1 FROM [Lookup].[CompetentAuthority] WHERE Id IN (@SEPAId, @NIEAId, @NRWId))
BEGIN
    PRINT N'Inserting annual charge data for SEPA, NRW, NIEA (£0 charge)...';

    -- Insert £0 charges for 2019-2026 for other authorities
    INSERT INTO [Lookup].[AnnualChargeByYear] ([Id], [CompetentAuthorityId], [ComplianceYear], [AnnualChargeAmount], [EffectiveFrom])
    SELECT * FROM (VALUES
        -- SEPA (Scotland)
        (NEWID(), @SEPAId, 2019, 0.00, '2019-01-01'),
        (NEWID(), @SEPAId, 2020, 0.00, '2020-01-01'),
        (NEWID(), @SEPAId, 2021, 0.00, '2021-01-01'),
        (NEWID(), @SEPAId, 2022, 0.00, '2022-01-01'),
        (NEWID(), @SEPAId, 2023, 0.00, '2023-01-01'),
        (NEWID(), @SEPAId, 2024, 0.00, '2024-01-01'),
        (NEWID(), @SEPAId, 2025, 0.00, '2025-01-01'),
        (NEWID(), @SEPAId, 2026, 0.00, '2026-01-01'),
        
        -- NRW (Wales)
        (NEWID(), @NRWId, 2019, 0.00, '2019-01-01'),
        (NEWID(), @NRWId, 2020, 0.00, '2020-01-01'),
        (NEWID(), @NRWId, 2021, 0.00, '2021-01-01'),
        (NEWID(), @NRWId, 2022, 0.00, '2022-01-01'),
        (NEWID(), @NRWId, 2023, 0.00, '2023-01-01'),
        (NEWID(), @NRWId, 2024, 0.00, '2024-01-01'),
        (NEWID(), @NRWId, 2025, 0.00, '2025-01-01'),
        (NEWID(), @NRWId, 2026, 0.00, '2026-01-01'),
        
        -- NIEA (Northern Ireland)
        (NEWID(), @NIEAId, 2019, 0.00, '2019-01-01'),
        (NEWID(), @NIEAId, 2020, 0.00, '2020-01-01'),
        (NEWID(), @NIEAId, 2021, 0.00, '2021-01-01'),
        (NEWID(), @NIEAId, 2022, 0.00, '2022-01-01'),
        (NEWID(), @NIEAId, 2023, 0.00, '2023-01-01'),
        (NEWID(), @NIEAId, 2024, 0.00, '2024-01-01'),
        (NEWID(), @NIEAId, 2025, 0.00, '2025-01-01'),
        (NEWID(), @NIEAId, 2026, 0.00, '2026-01-01')
    ) AS NewData ([Id], [CompetentAuthorityId], [ComplianceYear], [AnnualChargeAmount], [EffectiveFrom])
    WHERE NOT EXISTS (
        SELECT 1 FROM [Lookup].[AnnualChargeByYear] 
        WHERE [CompetentAuthorityId] = NewData.[CompetentAuthorityId]
        AND [ComplianceYear] = NewData.[ComplianceYear]
    )
    AND EXISTS (SELECT 1 FROM [Lookup].[CompetentAuthority] WHERE Id = NewData.[CompetentAuthorityId]);

    PRINT N'Inserted ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' annual charge records for SEPA, NRW, NIEA.';
END
ELSE
BEGIN
    PRINT N'WARNING: CompetentAuthority records for SEPA/NRW/NIEA not found. Skipping their annual charge data insertion.';
    PRINT N'         This data will be inserted by the Everytime script 0006-AnnualChargeByYearData.sql';
END

GO

PRINT N'=== AnnualChargeByYear Table Creation and Data Population Complete ===';
PRINT N'';
PRINT N'Summary:';
PRINT N'  - Table: [Lookup].[AnnualChargeByYear] created';
PRINT N'  - Data insertion depends on CompetentAuthority records existence';
PRINT N'  - If CompetentAuthority records don''t exist yet, data will be inserted by Everytime scripts';
PRINT N'';
PRINT N'Expected charges:';
PRINT N'  - EA (2019-2025): £12,500.00';
PRINT N'  - EA (2026): £13,438.00';
PRINT N'  - SEPA, NRW, NIEA (All years): £0.00';
PRINT N'';
PRINT N'Future years can be added with:';
PRINT N'  INSERT INTO [Lookup].[AnnualChargeByYear] ([Id], [CompetentAuthorityId], [ComplianceYear], [AnnualChargeAmount], [EffectiveFrom])';
PRINT N'  VALUES (NEWID(), ''A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8'', 2027, 14000.00, ''2027-01-01'');';

GO