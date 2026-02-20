-- =============================================
-- Create Online Marketplace Charge Lookup Table
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Lookup].[OnlineMarketplaceCharge]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Lookup].[OnlineMarketplaceCharge] (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [CompetentAuthority] INT NOT NULL,
        [Amount] DECIMAL(18, 2) NOT NULL,
        [EffectiveFrom] DATETIME2(0) NOT NULL,
        [RowVersion] ROWVERSION NOT NULL,
        CONSTRAINT [PK_OnlineMarketplaceCharge] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    -- Create index for efficient lookups
    CREATE NONCLUSTERED INDEX [IX_OnlineMarketplaceCharge_Lookup]
    ON [Lookup].[OnlineMarketplaceCharge] ([CompetentAuthority], [EffectiveFrom] DESC)
    INCLUDE ([Amount]);
END
GO

-- CompetentAuthorityType enum values:
-- 0 = NonUK
-- 1 = England
-- 2 = Wales
-- 3 = Scotland
-- 4 = NorthernIreland

-- Insert OMP charges for England
IF NOT EXISTS (SELECT 1 FROM [Lookup].[OnlineMarketplaceCharge] WHERE CompetentAuthority = 1)
BEGIN
    INSERT INTO [Lookup].[OnlineMarketplaceCharge] ([Id], [CompetentAuthority], [Amount], [EffectiveFrom])
    VALUES
        -- 2025 rate (effective from 1st January 2025) - England
        ('12767FA4-0DF2-4CA9-8954-1FB582198D2B', 1, 13631.00, '2025-01-01'),
        -- 2026 rate (effective from 1st April 2026) - England
        ('8B849A06-7A32-4B3E-9608-CDD1A48376AB', 1, 14653.00, '2026-04-01');
END
GO