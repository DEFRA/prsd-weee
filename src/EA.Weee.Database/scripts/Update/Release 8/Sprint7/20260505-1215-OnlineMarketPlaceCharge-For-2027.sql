-- =========================================================================
-- Author:		Sreedhar Bangarugari
-- Create date:	05-05-2026
-- Description:	Add OnlineMarketplace Charge for January 2027 fee
--				Fee Changes (effective January 1, 2027)
-- =========================================================================

DECLARE @amount DECIMAL(10,2) = '14148.98'
DECLARE @effectiveFrom NVARCHAR(10) = '2027-01-01'

-- CompetentAuthorityType = 0 means Non-UK
IF NOT EXISTS (SELECT 1 FROM [Lookup].[OnlineMarketplaceCharge] WHERE [EffectiveFrom] = @effectiveFrom AND [CompetentAuthority] = 0)
BEGIN
	INSERT INTO [Lookup].[OnlineMarketplaceCharge]
		([Id], [CompetentAuthority], [Amount], [EffectiveFrom])
	VALUES
		('AB196ED5-0AAF-4B1B-B234-EFF36E4CB1AD', 0, @amount, @effectiveFrom);
END

-- CompetentAuthorityType = 1 means England
IF NOT EXISTS (SELECT 1 FROM [Lookup].[OnlineMarketplaceCharge] WHERE [EffectiveFrom] = @effectiveFrom AND [CompetentAuthority] = 1)
BEGIN
	INSERT INTO [Lookup].[OnlineMarketplaceCharge]
		([Id], [CompetentAuthority], [Amount], [EffectiveFrom])
	VALUES
		('B1B3D282-0A52-49B7-8B38-E5127A599C85', 1, @amount, @effectiveFrom);
END