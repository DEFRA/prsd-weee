/* ===========================
   CHARGEBANDAMOUNT MIGRATION
   =========================== */

SET NOCOUNT ON;

-- ===========================
--   BATCH 1: Add columns only
-- ===========================
PRINT N'=== ChargeBandAmount Migration: Adding columns ===';

IF COL_LENGTH('Lookup.ChargeBandAmount','CompetentAuthority') IS NULL
BEGIN
    ALTER TABLE [Lookup].[ChargeBandAmount] ADD [CompetentAuthority] INT NULL;
    PRINT N'Added CompetentAuthority column';
END

IF COL_LENGTH('Lookup.ChargeBandAmount','VatRegistered') IS NULL
BEGIN
    ALTER TABLE [Lookup].[ChargeBandAmount] ADD [VatRegistered] BIT NULL;
    PRINT N'Added VatRegistered column';
END

IF COL_LENGTH('Lookup.ChargeBandAmount','AnnualTurnoverBand') IS NULL
BEGIN
    ALTER TABLE [Lookup].[ChargeBandAmount] ADD [AnnualTurnoverBand] INT NULL;
    PRINT N'Added AnnualTurnoverBand column';
END

IF COL_LENGTH('Lookup.ChargeBandAmount','EEEPlacedOnMarketBand') IS NULL
BEGIN
    ALTER TABLE [Lookup].[ChargeBandAmount] ADD [EEEPlacedOnMarketBand] INT NULL;
    PRINT N'Added EEEPlacedOnMarketBand column';
END

IF COL_LENGTH('Lookup.ChargeBandAmount','ComplianceYear') IS NULL
BEGIN
    ALTER TABLE [Lookup].[ChargeBandAmount] ADD [ComplianceYear] INT NULL;
    PRINT N'Added ComplianceYear column';
END

IF COL_LENGTH('Lookup.ChargeBandAmount','EffectiveFrom') IS NULL
BEGIN
    ALTER TABLE [Lookup].[ChargeBandAmount] ADD [EffectiveFrom] DATETIME2(0) NULL;
    PRINT N'Added EffectiveFrom column';
END

GO

-- =============================
--   BATCH 2: Insert new data
-- =============================
PRINT N'=== Inserting new charge band data ===';

INSERT INTO [Lookup].[ChargeBandAmount] (
    [Id], [Amount], [ChargeBand], [CompetentAuthority], [VatRegistered],
    [AnnualTurnoverBand], [EEEPlacedOnMarketBand], [ComplianceYear], [EffectiveFrom]
)
SELECT * FROM (VALUES
    -- 2025 Data (Effective from 2025-01-01)
    -- Non-UK (0)
    ('0C9F3CF2-752E-415B-832B-C8FA033C0D02',100.00,7,0,0,2,0,2025,'2025-01-01'), -- D2, VAT=FALSE, N/A turnover, ≥5T
    ('C67F1E00-750E-4504-9417-FEADE4AC1275',375.00,8,0,1,2,0,2025,'2025-01-01'), -- D3, VAT=TRUE,  N/A turnover, ≥5T
    ('AAE9546D-CD9C-4648-A3F5-8D99971AC2CC', 30.00,4,0,0,2,1,2025,'2025-01-01'), -- E, VAT=FALSE, N/A turnover, <5T
    ('66F1B29A-76B3-4D92-8E0A-AA7196B7AD9D', 30.00,4,0,1,2,1,2025,'2025-01-01'),  -- E, VAT=TRUE,  N/A turnover, <5T

    -- England (1)
    ('795B786B-0F29-473B-B276-DF7F27B614B6',750.00,5,1,1,2,0,2025,'2025-01-01'), -- A2, VAT=TRUE, N/A turnover, ≥5T
    ('88007B3D-3D72-496A-93A2-297230638B85',100.00,6,1,0,2,0,2025,'2025-01-01'), -- C2, VAT=FALSE, N/A turnover, ≥5T
    ('88C2682F-B770-40F3-8D1D-6A5DDF82DC84', 30.00,4,1,1,2,1,2025,'2025-01-01'), -- E, VAT=TRUE, N/A turnover, <5T
    ('733E5FA4-473F-48D7-9EEC-AFFA6F91075E', 30.00,4,1,0,2,1,2025,'2025-01-01'), -- E, VAT=FALSE, N/A turnover, <5T

    -- Wales (2)
    ('BAF18F7B-494D-4032-B327-A4D2CBB84413',445.00,0,2,1,1,0,2025,'2025-01-01'), -- A, VAT=TRUE, >£1m, ≥5T
    ('0DA8BD53-A7CF-46E3-9604-11F816428810',210.00,1,2,1,0,0,2025,'2025-01-01'), -- B, VAT=TRUE, <=£1m, ≥5T
    ('C0F07BBB-8E8B-4918-B32A-6367B1077292', 30.00,2,2,0,0,0,2025,'2025-01-01'), -- C, VAT=FALSE, <=£1m, ≥5T
    ('8617278B-C8EE-4C5E-867F-EDF378E08866', 30.00,3,2,0,1,0,2025,'2025-01-01'), -- D, VAT=FALSE, >£1m, ≥5T
    ('61976C61-F261-424D-974A-0B66508E161A', 30.00,4,2,1,1,1,2025,'2025-01-01'), -- E, VAT=TRUE, >£1m, <5T
    ('85FF07BC-07B0-4BC2-8FEE-2BC44AE7EB5F', 30.00,4,2,1,0,1,2025,'2025-01-01'), -- E, VAT=TRUE, <=£1m, <5T
    ('4557276D-3C17-4B2E-99F0-AA3C19F03DB8', 30.00,4,2,0,1,1,2025,'2025-01-01'), -- E, VAT=FALSE, >£1m, <5T
    ('BC4CBBFA-CCAC-4F8C-8DE7-FF01B2F2B18C', 30.00,4,2,0,0,1,2025,'2025-01-01'), -- E, VAT=FALSE, <=£1m, <5T

    -- Scotland (3)
    ('37085BDA-A3D1-4CBD-A814-9C3925AE6924',445.00,0,3,1,1,0,2025,'2025-01-01'), -- A, VAT=TRUE, >£1m, ≥5T
    ('D8FB1097-83AF-4B34-A6F9-DF889EEF81C8',210.00,1,3,1,0,0,2025,'2025-01-01'), -- B, VAT=TRUE, <=£1m, ≥5T
    ('934D4997-8AA5-47C6-A0EE-EB8A4AA1F119', 30.00,2,3,0,0,0,2025,'2025-01-01'), -- C, VAT=FALSE, <=£1m, ≥5T
    ('8DA8B5D8-A830-4CB5-BB23-1FC5C85D1745', 30.00,3,3,0,1,0,2025,'2025-01-01'), -- D, VAT=FALSE, >£1m, ≥5T
    ('89A39D4A-52B1-4999-9909-0C9EE35A8C95', 30.00,4,3,1,1,1,2025,'2025-01-01'), -- E, VAT=TRUE, >£1m, <5T
    ('0A77D8A2-F5A4-4E63-ACAE-61C7FC82E0F6', 30.00,4,3,1,0,1,2025,'2025-01-01'), -- E, VAT=TRUE, <=£1m, <5T
    ('9CA574B0-1CD1-4514-B883-FC21C1D8B291', 30.00,4,3,0,1,1,2025,'2025-01-01'), -- E, VAT=FALSE, >£1m, <5T
    ('54BF28D3-32EE-4B1D-8C7E-E8EA33E32BDD', 30.00,4,3,0,0,1,2025,'2025-01-01'), -- E, VAT=FALSE, <=£1m, <5T

    -- Northern Ireland (4)
    ('0C644F39-D047-478E-806E-FE308CD461AB',445.00,0,4,1,1,0,2025,'2025-01-01'), -- A, VAT=TRUE, >£1m, ≥5T
    ('9A904291-FF1A-4440-9333-88771A1EB12F',210.00,1,4,1,0,0,2025,'2025-01-01'), -- B, VAT=TRUE, <=£1m, ≥5T
    ('83B429FF-6E50-4211-A499-42D3559EF389', 30.00,2,4,0,0,0,2025,'2025-01-01'), -- C, VAT=FALSE, <=£1m, ≥5T
    ('352E25B5-D706-4216-A2C4-CD23DB818AA8', 30.00,3,4,0,1,0,2025,'2025-01-01'), -- D, VAT=FALSE, >£1m, ≥5T
    ('14EA62CD-3982-4516-A985-4E97DE22A30C', 30.00,4,4,1,0,1,2025,'2025-01-01'), -- E, VAT=TRUE, <=£1m, <5T
    ('323B147C-ABF7-4BE4-9C2D-384AB9B1EB26', 30.00,4,4,0,0,1,2025,'2025-01-01'), -- E, VAT=FALSE, <=£1m, <5T
    ('E65B77B0-C4D4-4F2E-AFA2-9EB93EFB349D', 30.00,4,4,1,1,1,2025,'2025-01-01'), -- E, VAT=TRUE, >£1m, <5T
    ('65B6F803-0BA7-4428-86B2-C13736E22963', 30.00,4,4,0,1,1,2025,'2025-01-01'), -- E, VAT=FALSE, >£1m, <5T

    -- 2026 Data (Effective from 2025-10-01)
    -- Non-UK (0)
    ('80F9E7EA-5310-4AB4-8D12-FC084A3BD870',108.00,7,0,0,2,0,2026,'2025-10-01'), -- D2, VAT=FALSE, N/A turnover, ≥5T
    ('DD58DDE1-8A72-4331-AD2B-DF43C361CD7B',403.00,8,0,1,2,0,2026,'2025-10-01'), -- D3, VAT=TRUE,  N/A turnover, ≥5T
    ('9E2BCCC5-ECDB-4DA2-BCC8-FC75564F11BA', 32.00,4,0,1,2,1,2026,'2025-10-01'), -- E, VAT=TRUE,  N/A turnover, <5T
    ('12B5C4F7-73AC-4853-81BA-D7E71240D9F7', 32.00,4,0,0,2,1,2026,'2025-10-01'),  -- E, VAT=FALSE, N/A turnover, <5T

    -- England (1)
    ('A363CDD3-CDE7-49AC-BC37-7E8CE0BCB3FE',806.00,5,1,1,2,0,2026,'2025-10-01'), -- A2, VAT=TRUE, N/A turnover, ≥5T
    ('7009C2EA-526F-4B4E-A4AC-EAFFAF4F4ACD',108.00,6,1,0,2,0,2026,'2025-10-01'), -- C2, VAT=FALSE, N/A turnover, ≥5T
    ('8BFBF6E7-2EF1-47DB-855D-B89044289F7D', 32.00,4,1,1,2,1,2026,'2025-10-01'), -- E, VAT=TRUE,  N/A turnover, <5T
    ('941DDC9D-5C06-48F5-9763-13665893166B', 32.00,4,1,0,2,1,2026,'2025-10-01'), -- E, VAT=FALSE, N/A turnover, <5T

    -- Wales (2)
    ('76FC1EFA-BB25-44DC-BA43-5BBEAA51932D',445.00,0,2,1,1,0,2026,'2025-10-01'), -- A, VAT=TRUE, >£1m, ≥5T
    ('143CB714-0193-4105-BB99-0C9B7056CB0E',210.00,1,2,1,0,0,2026,'2025-10-01'), -- B, VAT=TRUE, <=£1m, ≥5T
    ('B8D82EA3-6982-4646-B03A-EF744C94FF56', 30.00,2,2,0,0,0,2026,'2025-10-01'), -- C, VAT=FALSE, <=£1m, ≥5T
    ('3DF542ED-CD58-43FC-B843-F0758B8C68AA', 30.00,3,2,0,1,0,2026,'2025-10-01'), -- D, VAT=FALSE, >£1m, ≥5T
    ('FB0CCF6F-F24D-4854-99E9-DCF7A21C689C', 30.00,4,2,1,1,1,2026,'2025-10-01'), -- E, VAT=TRUE, >£1m, <5T
    ('AB33CC27-D301-4CFE-B766-CFA37C24175B', 30.00,4,2,1,0,1,2026,'2025-10-01'), -- E, VAT=TRUE, <=£1m, <5T
    ('496EA60C-10BF-43FD-9A20-89DF7CBF7A82', 30.00,4,2,0,1,1,2026,'2025-10-01'), -- E, VAT=FALSE, >£1m, <5T
    ('AFE73E8C-A0F2-4048-B0E5-59A4A3525D4E', 30.00,4,2,0,0,1,2026,'2025-10-01'), -- E, VAT=FALSE, <=£1m, <5T

    -- Scotland (3)
    ('A6E42F8D-3992-4C6F-9194-8DDB6EC1CC39',445.00,0,3,1,1,0,2026,'2025-10-01'), -- A, VAT=TRUE, >£1m, ≥5T
    ('8FD0B2BF-D4DB-488B-9EC9-7C88451EB7B4',210.00,1,3,1,0,0,2026,'2025-10-01'), -- B, VAT=TRUE, <=£1m, ≥5T
    ('0C76E813-DBE7-4DD7-BDDC-C0A0CC95DA7F', 30.00,2,3,0,0,0,2026,'2025-10-01'), -- C, VAT=FALSE, <=£1m, ≥5T
    ('4D795ABA-7F75-49A7-B48D-0C58CAA999BD', 30.00,3,3,0,1,0,2026,'2025-10-01'), -- D, VAT=FALSE, >£1m, ≥5T
    ('0930A67B-2CA8-4F47-9B70-AA8DA4205D22', 30.00,4,3,1,1,1,2026,'2025-10-01'), -- E, VAT=TRUE, >£1m, <5T
    ('9F6CFF46-4533-4801-A624-81FF9F1C5BFF', 30.00,4,3,1,0,1,2026,'2025-10-01'), -- E, VAT=TRUE, <=£1m, <5T
    ('BBDB908B-8DFE-4E4B-B43F-B4DB9204811C', 30.00,4,3,0,1,1,2026,'2025-10-01'), -- E, VAT=FALSE, >£1m, <5T
    ('EBD6790C-0713-466F-975E-FD2FB7CF72D0', 30.00,4,3,0,0,1,2026,'2025-10-01'), -- E, VAT=FALSE, <=£1m, <5T

    -- Northern Ireland (4)
    ('B90C7F34-C020-495B-A5EB-3B69A9F36EDE',445.00,0,4,1,1,0,2026,'2025-10-01'), -- A, VAT=TRUE, >£1m, ≥5T
    ('DACF3C12-8D31-4A8A-85D7-F233F6ABA236',210.00,1,4,1,0,0,2026,'2025-10-01'), -- B, VAT=TRUE, <=£1m, ≥5T
    ('7F5AD805-7ABD-4429-861D-032665AB2744', 30.00,2,4,0,0,0,2026,'2025-10-01'), -- C, VAT=FALSE, <=£1m, ≥5T
    ('CA12B443-FC51-4F3A-BD37-B89D4178BB64', 30.00,3,4,0,1,0,2026,'2025-10-01'), -- D, VAT=FALSE, >£1m, ≥5T
    ('947A62F4-60D1-4A60-B8DC-C4536F5F3A93', 30.00,4,4,1,1,1,2026,'2025-10-01'), -- E, VAT=TRUE, >£1m, <5T
    ('B0E8880B-5A34-4015-908F-17EFA92533B3', 30.00,4,4,1,0,1,2026,'2025-10-01'), -- E, VAT=TRUE, <=£1m, <5T
    ('CFE202AD-2F73-4AE5-8B7C-BFBC18849B7B', 30.00,4,4,0,1,1,2026,'2025-10-01'), -- E, VAT=FALSE, >£1m, <5T
    ('D0B375BB-ADCA-487F-91FB-C38985C3B9B7', 30.00,4,4,0,0,1,2026,'2025-10-01') -- E, VAT=FALSE, <=£1m, <5T
) AS NewData ([Id],[Amount],[ChargeBand],[CompetentAuthority],[VatRegistered],[AnnualTurnoverBand],[EEEPlacedOnMarketBand],[ComplianceYear],[EffectiveFrom])
WHERE NOT EXISTS (
    SELECT 1 FROM [Lookup].[ChargeBandAmount] 
    WHERE [Lookup].[ChargeBandAmount].[Id] = NewData.[Id]
);

PRINT N'New records added: ' + CAST(@@ROWCOUNT AS NVARCHAR(10));

GO

-- =============================
--   BATCH 3: Create indexes
-- =============================
PRINT N'=== Creating performance indexes ===';

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ChargeBandAmount_Lookup' AND object_id = OBJECT_ID('[Lookup].[ChargeBandAmount]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ChargeBandAmount_Lookup
    ON [Lookup].[ChargeBandAmount] (
        [CompetentAuthority], [VatRegistered], [AnnualTurnoverBand], 
        [EEEPlacedOnMarketBand], [ComplianceYear], [EffectiveFrom] DESC
    )
    INCLUDE ([Id], [Amount], [ChargeBand]);
    PRINT N'Created main lookup index';
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ChargeBandAmount_ComplianceYear' AND object_id = OBJECT_ID('[Lookup].[ChargeBandAmount]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ChargeBandAmount_ComplianceYear
    ON [Lookup].[ChargeBandAmount] ([ComplianceYear], [EffectiveFrom] DESC);
    PRINT N'Created compliance year index';
END

PRINT N'=== Migration completed successfully ===';

GO

