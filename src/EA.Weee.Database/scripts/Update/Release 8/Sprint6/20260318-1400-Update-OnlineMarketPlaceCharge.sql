/* ================================================
   UPDATE/INSERT ONLINEMARKETPLACECHARGE RECORDS
   ================================================ */

SET NOCOUNT ON;

-- Update existing record
UPDATE [Lookup].[OnlineMarketplaceCharge]
SET [Amount] = 14148.98
WHERE [Id] = '8B849A06-7A32-4B3E-9608-CDD1A48376AB';

-- Insert new records if they don't exist
IF NOT EXISTS (SELECT 1 FROM [Lookup].[OnlineMarketplaceCharge] WHERE [Id] = 'ED6C220E-BC29-4DFC-8A29-681393EEB197')
BEGIN
    INSERT INTO [Lookup].[OnlineMarketplaceCharge] ([Id], [CompetentAuthority], [Amount], [EffectiveFrom])
    VALUES
        -- 2025 rate (effective from 1st January 2025) - Non-UK
        ('ED6C220E-BC29-4DFC-8A29-681393EEB197', 0, 13631.00, '2025-01-01');
END

IF NOT EXISTS (SELECT 1 FROM [Lookup].[OnlineMarketplaceCharge] WHERE [Id] = '9F1D4FD9-9FC7-4321-8EF7-B9A6FBE00053')
BEGIN
    INSERT INTO [Lookup].[OnlineMarketplaceCharge] ([Id], [CompetentAuthority], [Amount], [EffectiveFrom])
    VALUES
        -- 2026 rate (effective from 1st April 2026) - Non-UK
        ('9F1D4FD9-9FC7-4321-8EF7-B9A6FBE00053', 0, 14148.98, '2026-04-01');
END

GO