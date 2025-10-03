GO
PRINT N'Altering [Lookup].[ChargeBandAmount] table to add missing columns for new charging system...';

-- Add CompetentAuthority column if it doesn't exist
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_SCHEMA = 'Lookup' 
               AND TABLE_NAME = 'ChargeBandAmount' 
               AND COLUMN_NAME = 'CompetentAuthority')
BEGIN
    ALTER TABLE [Lookup].[ChargeBandAmount]
    ADD [CompetentAuthority] INT NOT NULL DEFAULT(0);
    
    PRINT N'Added CompetentAuthority column to [Lookup].[ChargeBandAmount] table.';
END
ELSE
BEGIN
    PRINT N'CompetentAuthority column already exists in [Lookup].[ChargeBandAmount] table.';
END

-- Add VatRegistered column if it doesn't exist
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_SCHEMA = 'Lookup' 
               AND TABLE_NAME = 'ChargeBandAmount' 
               AND COLUMN_NAME = 'VatRegistered')
BEGIN
    ALTER TABLE [Lookup].[ChargeBandAmount]
    ADD [VatRegistered] BIT NOT NULL DEFAULT(0);
    
    PRINT N'Added VatRegistered column to [Lookup].[ChargeBandAmount] table.';
END
ELSE
BEGIN
    PRINT N'VatRegistered column already exists in [Lookup].[ChargeBandAmount] table.';
END

-- Add AnnualTurnoverBand column if it doesn't exist
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_SCHEMA = 'Lookup' 
               AND TABLE_NAME = 'ChargeBandAmount' 
               AND COLUMN_NAME = 'AnnualTurnoverBand')
BEGIN
    ALTER TABLE [Lookup].[ChargeBandAmount]
    ADD [AnnualTurnoverBand] INT NOT NULL DEFAULT(0);
    
    PRINT N'Added AnnualTurnoverBand column to [Lookup].[ChargeBandAmount] table.';
END
ELSE
BEGIN
    PRINT N'AnnualTurnoverBand column already exists in [Lookup].[ChargeBandAmount] table.';
END

-- Add EEEPlacedOnMarketBand column if it doesn't exist
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_SCHEMA = 'Lookup' 
               AND TABLE_NAME = 'ChargeBandAmount' 
               AND COLUMN_NAME = 'EEEPlacedOnMarketBand')
BEGIN
    ALTER TABLE [Lookup].[ChargeBandAmount]
    ADD [EEEPlacedOnMarketBand] INT NOT NULL DEFAULT(0);
    
    PRINT N'Added EEEPlacedOnMarketBand column to [Lookup].[ChargeBandAmount] table.';
END
ELSE
BEGIN
    PRINT N'EEEPlacedOnMarketBand column already exists in [Lookup].[ChargeBandAmount] table.';
END

-- Add ComplianceYear column if it doesn't exist
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_SCHEMA = 'Lookup' 
               AND TABLE_NAME = 'ChargeBandAmount' 
               AND COLUMN_NAME = 'ComplianceYear')
BEGIN
    ALTER TABLE [Lookup].[ChargeBandAmount]
    ADD [ComplianceYear] INT NOT NULL DEFAULT(2025);
    
    PRINT N'Added ComplianceYear column to [Lookup].[ChargeBandAmount] table.';
END
ELSE
BEGIN
    PRINT N'ComplianceYear column already exists in [Lookup].[ChargeBandAmount] table.';
END

-- Add EffectiveFrom column if it doesn't exist
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_SCHEMA = 'Lookup' 
               AND TABLE_NAME = 'ChargeBandAmount' 
               AND COLUMN_NAME = 'EffectiveFrom')
BEGIN
    ALTER TABLE [Lookup].[ChargeBandAmount]
    ADD [EffectiveFrom] DATETIME2(0) NOT NULL DEFAULT('2025-01-01');
    
    PRINT N'Added EffectiveFrom column to [Lookup].[ChargeBandAmount] table.';
END
ELSE
BEGIN
    PRINT N'EffectiveFrom column already exists in [Lookup].[ChargeBandAmount] table.';
END

PRINT N'Column structure updates completed. Continuing with indexes and data...';
GO

-- Separate batch to ensure column changes are committed before proceeding
PRINT N'Creating performance indexes on [Lookup].[ChargeBandAmount]...';

-- Create indexes for efficient querying (only if they don't exist)
IF NOT EXISTS (SELECT * FROM sys.indexes 
               WHERE name = 'IX_ChargeBandAmount_CompetentAuthority_VatRegistered_AnnualTurnoverBand_EEEPlacedOnMarketBand_ComplianceYear_EffectiveFrom'
               AND object_id = OBJECT_ID('[Lookup].[ChargeBandAmount]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ChargeBandAmount_CompetentAuthority_VatRegistered_AnnualTurnoverBand_EEEPlacedOnMarketBand_ComplianceYear_EffectiveFrom]
    ON [Lookup].[ChargeBandAmount] ([CompetentAuthority], [VatRegistered], [AnnualTurnoverBand], [EEEPlacedOnMarketBand], [ComplianceYear], [EffectiveFrom] DESC)
    INCLUDE ([Id], [Amount], [ChargeBand]);
    
    PRINT N'Created main performance index for new charging system queries.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes 
               WHERE name = 'IX_ChargeBandAmount_ComplianceYear_EffectiveFrom'
               AND object_id = OBJECT_ID('[Lookup].[ChargeBandAmount]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ChargeBandAmount_ComplianceYear_EffectiveFrom]
    ON [Lookup].[ChargeBandAmount] ([ComplianceYear], [EffectiveFrom] DESC);
    
    PRINT N'Created temporal index for compliance year and effective date queries.';
END

-- Check existing records before migration
DECLARE @ExistingCount INT;
SELECT @ExistingCount = COUNT(*) FROM [Lookup].[ChargeBandAmount];
PRINT N'Existing records in table before migration: ' + CAST(@ExistingCount AS NVARCHAR(10));

PRINT N'Adding new charging system data while preserving existing records...';

-- Build INSERT statement dynamically based on available columns
DECLARE @InsertSQL NVARCHAR(MAX);
DECLARE @ColumnList NVARCHAR(MAX) = '';
DECLARE @ValuesList NVARCHAR(MAX) = '';

-- Check which columns exist and build the column list
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'Lookup' AND TABLE_NAME = 'ChargeBandAmount' AND COLUMN_NAME = 'Id')
    SET @ColumnList = @ColumnList + '[Id],';
    
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'Lookup' AND TABLE_NAME = 'ChargeBandAmount' AND COLUMN_NAME = 'Amount')
    SET @ColumnList = @ColumnList + '[Amount],';
    
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'Lookup' AND TABLE_NAME = 'ChargeBandAmount' AND COLUMN_NAME = 'ChargeBand')
    SET @ColumnList = @ColumnList + '[ChargeBand],';
    
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'Lookup' AND TABLE_NAME = 'ChargeBandAmount' AND COLUMN_NAME = 'CompetentAuthority')
    SET @ColumnList = @ColumnList + '[CompetentAuthority],';
    
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'Lookup' AND TABLE_NAME = 'ChargeBandAmount' AND COLUMN_NAME = 'VatRegistered')
    SET @ColumnList = @ColumnList + '[VatRegistered],';
    
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'Lookup' AND TABLE_NAME = 'ChargeBandAmount' AND COLUMN_NAME = 'AnnualTurnoverBand')
    SET @ColumnList = @ColumnList + '[AnnualTurnoverBand],';
    
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'Lookup' AND TABLE_NAME = 'ChargeBandAmount' AND COLUMN_NAME = 'EEEPlacedOnMarketBand')
    SET @ColumnList = @ColumnList + '[EEEPlacedOnMarketBand],';
    
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'Lookup' AND TABLE_NAME = 'ChargeBandAmount' AND COLUMN_NAME = 'ComplianceYear')
    SET @ColumnList = @ColumnList + '[ComplianceYear],';
    
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'Lookup' AND TABLE_NAME = 'ChargeBandAmount' AND COLUMN_NAME = 'EffectiveFrom')
    SET @ColumnList = @ColumnList + '[EffectiveFrom],';

-- Remove trailing comma
SET @ColumnList = LEFT(@ColumnList, LEN(@ColumnList) - 1);

PRINT N'Available columns for insert: ' + @ColumnList;

-- Create a temp table with all the new data
IF OBJECT_ID('tempdb..#NewChargeBandData') IS NOT NULL DROP TABLE #NewChargeBandData;

CREATE TABLE #NewChargeBandData (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [Amount] DECIMAL(18,2) NOT NULL,
    [ChargeBand] INT NOT NULL,
    [CompetentAuthority] INT NOT NULL,
    [VatRegistered] BIT NOT NULL,
    [AnnualTurnoverBand] INT NOT NULL,
    [EEEPlacedOnMarketBand] INT NOT NULL,
    [ComplianceYear] INT NOT NULL,
    [EffectiveFrom] DATETIME2(0) NOT NULL
);

-- Load new data for 2025 and 2026 compliance years
INSERT INTO #NewChargeBandData ([Id],[Amount],[ChargeBand],[CompetentAuthority],[VatRegistered],
                               [AnnualTurnoverBand],[EEEPlacedOnMarketBand],[ComplianceYear],[EffectiveFrom])
VALUES
-- 2025 Data (Effective from 2025-01-01)
('BAF18F7B-494D-4032-B327-A4D2CBB84413',445.00,0,2,1,1,0,2025,'2025-01-01'),
('37085BDA-A3D1-4CBD-A814-9C3925AE6924',445.00,0,3,1,1,0,2025,'2025-01-01'),
('0C644F39-D047-478E-806E-FE308CD461AB',445.00,0,4,1,1,0,2025,'2025-01-01'),
('D8FB1097-83AF-4B34-A6F9-DF889EEF81C8',210.00,1,3,1,0,0,2025,'2025-01-01'),
('9A904291-FF1A-4440-9333-88771A1EB12F',210.00,1,4,1,0,0,2025,'2025-01-01'),
('0DA8BD53-A7CF-46E3-9604-11F816428810',210.00,1,2,1,0,0,2025,'2025-01-01'),
('83B429FF-6E50-4211-A499-42D3559EF389', 30.00,2,4,0,0,0,2025,'2025-01-01'),
('C0F07BBB-8E8B-4918-B32A-6367B1077292', 30.00,2,2,0,0,0,2025,'2025-01-01'),
('934D4997-8AA5-47C6-A0EE-EB8A4AA1F119', 30.00,2,3,0,0,0,2025,'2025-01-01'),
('8617278B-C8EE-4C5E-867F-EDF378E08866', 30.00,3,2,0,1,0,2025,'2025-01-01'),
('352E25B5-D706-4216-A2C4-CD23DB818AA8', 30.00,3,4,0,1,0,2025,'2025-01-01'),
('8DA8B5D8-A830-4CB5-BB23-1FC5C85D1745', 30.00,3,3,0,1,0,2025,'2025-01-01'),
('61976C61-F261-424D-974A-0B66508E161A', 30.00,4,2,1,1,1,2025,'2025-01-01'),
('88C2682F-B770-40F3-8D1D-6A5DDF82DC84', 30.00,4,1,1,2,1,2025,'2025-01-01'),
('14EA62CD-3982-4516-A985-4E97DE22A30C', 30.00,4,4,1,0,1,2025,'2025-01-01'),
('85FF07BC-07B0-4BC2-8FEE-2BC44AE7EB5F', 30.00,4,2,1,0,1,2025,'2025-01-01'),
('323B147C-ABF7-4BE4-9C2D-384AB9B1EB26', 30.00,4,4,0,0,1,2025,'2025-01-01'),
('AAE9546D-CD9C-4648-A3F5-8D99971AC2CC', 30.00,4,0,0,2,1,2025,'2025-01-01'),
('89A39D4A-52B1-4999-9909-0C9EE35A8C95', 30.00,4,3,1,1,1,2025,'2025-01-01'),
('0A77D8A2-F5A4-4E63-ACAE-61C7FC82E0F6', 30.00,4,3,1,0,1,2025,'2025-01-01'),
('E65B77B0-C4D4-4F2E-AFA2-9EB93EFB349D', 30.00,4,4,1,1,1,2025,'2025-01-01'),
('733E5FA4-473F-48D7-9EEC-AFFA6F91075E', 30.00,4,1,0,2,1,2025,'2025-01-01'),
('4557276D-3C17-4B2E-99F0-AA3C19F03DB8', 30.00,4,2,0,1,1,2025,'2025-01-01'),
('66F1B29A-76B3-4D92-8E0A-AA7196B7AD9D', 30.00,4,0,1,2,1,2025,'2025-01-01'),
('65B6F803-0BA7-4428-86B2-C13736E22963', 30.00,4,4,0,1,1,2025,'2025-01-01'),
('54BF28D3-32EE-4B1D-8C7E-E8EA33E32BDD', 30.00,4,3,0,0,1,2025,'2025-01-01'),
('BC4CBBFA-CCAC-4F8C-8DE7-FF01B2F2B18C', 30.00,4,2,0,0,1,2025,'2025-01-01'),
('9CA574B0-1CD1-4514-B883-FC21C1D8B291', 30.00,4,3,0,1,1,2025,'2025-01-01'),
('795B786B-0F29-473B-B276-DF7F27B614B6',750.00,5,1,1,2,0,2025,'2025-01-01'),
('88007B3D-3D72-496A-93A2-297230638B85',100.00,6,1,0,2,0,2025,'2025-01-01'),
('0C9F3CF2-752E-415B-832B-C8FA033C0D02',100.00,7,0,0,2,0,2025,'2025-01-01'),
('C67F1E00-750E-4504-9417-FEADE4AC1275',375.00,8,0,1,2,0,2025,'2025-01-01'),

-- 2026 Data (Effective from 2025-10-01)
('B90C7F34-C020-495B-A5EB-3B69A9F36EDE',445.00,0,4,1,1,0,2026,'2025-10-01'),
('76FC1EFA-BB25-44DC-BA43-5BBEAA51932D',445.00,0,2,1,1,0,2026,'2025-10-01'),
('A6E42F8D-3992-4C6F-9194-8DDB6EC1CC39',445.00,0,3,1,1,0,2026,'2025-10-01'),
('8FD0B2BF-D4DB-488B-9EC9-7C88451EB7B4',210.00,1,3,1,0,0,2026,'2025-10-01'),
('143CB714-0193-4105-BB99-0C9B7056CB0E',210.00,1,2,1,0,0,2026,'2025-10-01'),
('DACF3C12-8D31-4A8A-85D7-F233F6ABA236',210.00,1,4,1,0,0,2026,'2025-10-01'),
('0C76E813-DBE7-4DD7-BDDC-C0A0CC95DA7F', 30.00,2,3,0,0,0,2026,'2025-10-01'),
('B8D82EA3-6982-4646-B03A-EF744C94FF56', 30.00,2,2,0,0,0,2026,'2025-10-01'),
('7F5AD805-7ABD-4429-861D-032665AB2744', 30.00,2,4,0,0,0,2026,'2025-10-01'),
('4D795ABA-7F75-49A7-B48D-0C58CAA999BD', 30.00,3,3,0,1,0,2026,'2025-10-01'),
('3DF542ED-CD58-43FC-B843-F0758B8C68AA', 30.00,3,2,0,1,0,2026,'2025-10-01'),
('CA12B443-FC51-4F3A-BD37-B89D4178BB64', 30.00,3,4,0,1,0,2026,'2025-10-01'),
('CFE202AD-2F73-4AE5-8B7C-BFBC18849B7B', 30.00,4,4,0,1,1,2026,'2025-10-01'),
('D0B375BB-ADCA-487F-91FB-C38985C3B9B7', 30.00,4,4,0,0,1,2026,'2025-10-01'),
('947A62F4-60D1-4A60-B8DC-C4536F5F3A93', 30.00,4,4,1,1,1,2026,'2025-10-01'),
('AB33CC27-D301-4CFE-B766-CFA37C24175B', 30.00,4,2,1,0,1,2026,'2025-10-01'),
('12B5C4F7-73AC-4853-81BA-D7E71240D9F7', 32.00,4,0,0,2,1,2026,'2025-10-01'),
('FB0CCF6F-F24D-4854-99E9-DCF7A21C689C', 30.00,4,2,1,1,1,2026,'2025-10-01'),
('9E2BCCC5-ECDB-4DA2-BCC8-FC75564F11BA', 32.00,4,0,1,2,1,2026,'2025-10-01'),
('EBD6790C-0713-466F-975E-FD2FB7CF72D0', 30.00,4,3,0,0,1,2026,'2025-10-01'),
('941DDC9D-5C06-48F5-9763-13665893166B', 32.00,4,1,0,2,1,2026,'2025-10-01'),
('B0E8880B-5A34-4015-908F-17EFA92533B3', 30.00,4,4,1,0,1,2026,'2025-10-01'),
('AFE73E8C-A0F2-4048-B0E5-59A4A3525D4E', 30.00,4,2,0,0,1,2026,'2025-10-01'),
('496EA60C-10BF-43FD-9A20-89DF7CBF7A82', 30.00,4,2,0,1,1,2026,'2025-10-01'),
('9F6CFF46-4533-4801-A624-81FF9F1C5BFF', 30.00,4,3,1,0,1,2026,'2025-10-01'),
('0930A67B-2CA8-4F47-9B70-AA8DA4205D22', 30.00,4,3,1,1,1,2026,'2025-10-01'),
('BBDB908B-8DFE-4E4B-B43F-B4DB9204811C', 30.00,4,3,0,1,1,2026,'2025-10-01'),
('8BFBF6E7-2EF1-47DB-855D-B89044289F7D', 32.00,4,1,1,2,1,2026,'2025-10-01'),
('A363CDD3-CDE7-49AC-BC37-7E8CE0BCB3FE',806.00,5,1,1,2,0,2026,'2025-10-01'),
('7009C2EA-526F-4B4E-A4AC-EAFFAF4F4ACD',108.00,6,1,0,2,0,2026,'2025-10-01'),
('80F9E7EA-5310-4AB4-8D12-FC084A3BD870',108.00,7,0,0,2,0,2026,'2025-10-01'),
('DD58DDE1-8A72-4331-AD2B-DF43C361CD7B',403.00,8,0,1,2,0,2026,'2025-10-01');

-- Insert only new records that don't already exist (preserves existing data)
SET @InsertSQL = N'
INSERT INTO [Lookup].[ChargeBandAmount] (' + @ColumnList + N')
SELECT ' + @ColumnList + N'
FROM #NewChargeBandData
WHERE NOT EXISTS (
    SELECT 1 FROM [Lookup].[ChargeBandAmount] 
    WHERE [Lookup].[ChargeBandAmount].[Id] = #NewChargeBandData.[Id]
);';

PRINT N'Executing insert with dynamic column list...';
EXEC sp_executesql @InsertSQL;

DECLARE @NewRecordsAdded INT;
SELECT @NewRecordsAdded = @@ROWCOUNT;
PRINT N'New records added: ' + CAST(@NewRecordsAdded AS NVARCHAR(10));

-- Clean up temp table
DROP TABLE #NewChargeBandData;

PRINT N'Successfully enhanced [Lookup].[ChargeBandAmount] table with new charging system structure and data.';

-- Verify the final state
DECLARE @FinalCount INT;
SELECT @FinalCount = COUNT(*) FROM [Lookup].[ChargeBandAmount];
PRINT N'Total records in table after enhancement: ' + CAST(@FinalCount AS NVARCHAR(10));

-- Verify all required columns exist
DECLARE @FinalColumnList NVARCHAR(MAX) = '';
SELECT @FinalColumnList = @FinalColumnList + COLUMN_NAME + ', '
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = 'Lookup' 
AND TABLE_NAME = 'ChargeBandAmount'
ORDER BY ORDINAL_POSITION;

PRINT N'Final table structure: ' + LEFT(@FinalColumnList, LEN(@FinalColumnList) - 1);

-- Verify we have data for both compliance years
DECLARE @Year2025Count INT, @Year2026Count INT;
SELECT @Year2025Count = COUNT(*) FROM [Lookup].[ChargeBandAmount] WHERE ComplianceYear = 2025;
SELECT @Year2026Count = COUNT(*) FROM [Lookup].[ChargeBandAmount] WHERE ComplianceYear = 2026;
PRINT N'Records for 2025: ' + CAST(@Year2025Count AS NVARCHAR(10));
PRINT N'Records for 2026: ' + CAST(@Year2026Count AS NVARCHAR(10));

-- Show preservation of existing records
DECLARE @LegacyCount INT;
SELECT @LegacyCount = COUNT(*) FROM [Lookup].[ChargeBandAmount] 
WHERE ComplianceYear NOT IN (2025, 2026) OR ComplianceYear IS NULL;
PRINT N'Legacy/existing records preserved: ' + CAST(@LegacyCount AS NVARCHAR(10));

PRINT N'ChargeBandAmount table enhancement completed successfully. New charging system is now enabled while preserving existing data.';

GO