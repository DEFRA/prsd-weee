-- Add Non-UK Online Marketplace charges
IF NOT EXISTS (SELECT 1 FROM [Lookup].[OnlineMarketplaceCharge] WHERE CompetentAuthority = 0 AND EffectiveFrom = '2025-01-01')
BEGIN
    INSERT INTO [Lookup].[OnlineMarketplaceCharge] (Id, CompetentAuthority, Amount, EffectiveFrom)
    VALUES ('ED6C220E-BC29-4DFC-8A29-681393EEB197', 0, 13631.00, '2025-01-01');  -- NonUK 2025
END

IF NOT EXISTS (SELECT 1 FROM [Lookup].[OnlineMarketplaceCharge] WHERE CompetentAuthority = 0 AND EffectiveFrom = '2026-04-01')
BEGIN
    INSERT INTO [Lookup].[OnlineMarketplaceCharge] (Id, CompetentAuthority, Amount, EffectiveFrom)
    VALUES ('9F1D4FD9-9FC7-4321-8EF7-B9A6FBE00053', 0, 14653.00, '2026-04-01');  -- NonUK 2026
END