/* ===================================
   ONLINE MARKETPLACE CHARGE TABLE
   =================================== */

SET NOCOUNT ON;

-- Create table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Lookup].[OnlineMarketplaceCharge]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Lookup].[OnlineMarketplaceCharge] (
        [Id]            UNIQUEIDENTIFIER NOT NULL,
        [Amount]        DECIMAL(18, 2)   NOT NULL,
        [EffectiveFrom] DATETIME2(0)     NOT NULL,
        CONSTRAINT [PK_OnlineMarketplaceCharge] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END

GO

-- Create index
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_OnlineMarketplaceCharge_EffectiveFrom' AND object_id = OBJECT_ID('[Lookup].[OnlineMarketplaceCharge]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_OnlineMarketplaceCharge_EffectiveFrom
    ON [Lookup].[OnlineMarketplaceCharge] ([EffectiveFrom] DESC)
    INCLUDE ([Amount]);
END

GO

-- Insert seed data
INSERT INTO [Lookup].[OnlineMarketplaceCharge] ([Id], [Amount], [EffectiveFrom])
SELECT * FROM (VALUES
    -- Current charge
    ('12767FA4-0DF2-4CA9-8954-1FB582198D2B', 13631.00, '2016-01-01'),
    
    -- 2026 charge (effective from 2026-04-01)
    ('8B849A06-7A32-4B3E-9608-CDD1A48376AB', 14148.98, '2026-04-01')
) AS NewData ([Id], [Amount], [EffectiveFrom])
WHERE NOT EXISTS (
    SELECT 1 FROM [Lookup].[OnlineMarketplaceCharge] 
    WHERE [Lookup].[OnlineMarketplaceCharge].[Id] = NewData.[Id]
);

PRINT N'Records added: ' + CAST(@@ROWCOUNT AS NVARCHAR(10));
GO