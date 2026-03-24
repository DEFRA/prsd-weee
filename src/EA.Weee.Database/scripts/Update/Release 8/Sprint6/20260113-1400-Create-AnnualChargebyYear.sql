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
        ('5CE3679C-7B53-479D-AFB8-DD0DD921E5A5', @EAId, 2019, 12500.00, '2019-01-01'),
        ('7B993DD9-3DA1-4209-A53D-D367D5D5AE68', @EAId, 2020, 12500.00, '2020-01-01'),
        ('13C5F5A1-79AA-4207-B6BF-4BB5B40BBA4C', @EAId, 2021, 12500.00, '2021-01-01'),
        ('33EC6A2A-BC1B-4B6C-9CE8-47DFFC05B39F', @EAId, 2022, 12500.00, '2022-01-01'),
        ('A67EAC3B-8165-423E-8EF9-ABC2FF597B0C', @EAId, 2023, 12500.00, '2023-01-01'),
        ('6E8C4720-FDA1-4F23-8320-038D5000954E', @EAId, 2024, 12500.00, '2024-01-01'),
        ('26BC0040-4375-49F0-838B-1C9957DA1E6B', @EAId, 2025, 12500.00, '2025-01-01'),
        -- New 2026 uplifted charge (7.5% increase = £938)
        ('DEB42CB9-3250-4971-8C3D-E5475415AABB', @EAId, 2026, 13438.00, '2026-01-01')
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
        ('3070F540-24F5-4C3B-93F3-739CB997E768', @SEPAId, 2019, 0.00, '2019-01-01'),
        ('26FA5850-42F7-49D4-9347-B5CA77568991', @SEPAId, 2020, 0.00, '2020-01-01'),
        ('9C040A08-619B-4A93-BA4B-4D3E149AFF2C', @SEPAId, 2021, 0.00, '2021-01-01'),
        ('6C3ECF1A-6783-47B4-9992-A69712B25598', @SEPAId, 2022, 0.00, '2022-01-01'),
        ('B29BD0AA-FD16-4EBF-8F63-51A0FD9E7BE9', @SEPAId, 2023, 0.00, '2023-01-01'),
        ('5BB2F79D-E33E-42BB-B7C8-227BE740EDC9', @SEPAId, 2024, 0.00, '2024-01-01'),
        ('7E5E9C81-7219-4B0C-81FB-FF865469AC24', @SEPAId, 2025, 0.00, '2025-01-01'),
        ('6690C347-DE49-46F1-B4AD-9901D365BD9E', @SEPAId, 2026, 0.00, '2026-01-01'),
        
        -- NRW (Wales)
        ('803352A4-BAF2-4E3D-B620-50606BFD3A21', @NRWId, 2019, 0.00, '2019-01-01'),
        ('7BE15AD4-6D01-4E93-83F9-DB1271C7AF5A', @NRWId, 2020, 0.00, '2020-01-01'),
        ('09182CF6-3F6E-4D22-872C-CB68C9242A6D', @NRWId, 2021, 0.00, '2021-01-01'),
        ('3306CF13-2C4C-4543-859C-422D918B84EE', @NRWId, 2022, 0.00, '2022-01-01'),
        ('14E6A41D-8A4D-493D-B386-8E061059F0FC', @NRWId, 2023, 0.00, '2023-01-01'),
        ('9D36CD28-4B52-4660-8923-5297A024C116', @NRWId, 2024, 0.00, '2024-01-01'),
        ('A6F39098-A6C5-4BAF-B168-54B713D68E36', @NRWId, 2025, 0.00, '2025-01-01'),
        ('07C7DDE2-95B1-423D-A6C4-DB88AF85F3F9', @NRWId, 2026, 0.00, '2026-01-01'),
        
        -- NIEA (Northern Ireland)
        ('31F05828-A2B9-4747-AD82-48898BD88CA8', @NIEAId, 2019, 0.00, '2019-01-01'),
        ('AD7C6DA5-819E-48D4-A8D5-78AC834C9F06', @NIEAId, 2020, 0.00, '2020-01-01'),
        ('BBC5FA6E-72A4-4FF9-9EC9-F9965E3BD316', @NIEAId, 2021, 0.00, '2021-01-01'),
        ('BD878B7F-6BD9-425D-9990-EEFDDEFF5095', @NIEAId, 2022, 0.00, '2022-01-01'),
        ('B83B3978-B29A-446E-BA96-EB7E25096904', @NIEAId, 2023, 0.00, '2023-01-01'),
        ('FD38FBFF-CF00-4C99-B7E8-9E402F41945B', @NIEAId, 2024, 0.00, '2024-01-01'),
        ('2A9D9C68-4528-4C87-972A-65FFB5C4C29A', @NIEAId, 2025, 0.00, '2025-01-01'),
        ('71CC9860-E829-44AF-A0B8-7296F136EE12', @NIEAId, 2026, 0.00, '2026-01-01')
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