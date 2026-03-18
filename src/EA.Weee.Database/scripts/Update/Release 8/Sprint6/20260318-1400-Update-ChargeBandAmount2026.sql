/* ===========================
   2026 CHARGEBANDAMOUNT UPDATE
   =========================== */

SET NOCOUNT ON;

-- Update existing 2026 ChargeBandAmount records with corrected values
UPDATE [Lookup].[ChargeBandAmount]
SET [Amount] = source.[Amount]
FROM [Lookup].[ChargeBandAmount] AS target
INNER JOIN (VALUES
    -- Non-UK (0) - INCREASED
    ('F1A2B3C4-D5E6-47F8-A9B0-1C2D3E4F5A6B', 111.59),-- D2, VAT=FALSE, N/A turnover, ≥5T
    ('A7B8C9D0-E1F2-43A4-B5C6-7D8E9F0A1B2C', 418.45),-- D3, VAT=TRUE,  N/A turnover, ≥5T
    ('D4E5F6A7-B8C9-4D0E-1F2A-3B4C5D6E7F8A', 33.48), -- E, VAT=TRUE,  N/A turnover, <5T
    ('E9F0A1B2-C3D4-4E5F-6A7B-8C9D0E1F2A3B', 33.48), -- E, VAT=FALSE, N/A turnover, <5T
    -- England (1) - INCREASED
    ('B2C3D4E5-F6A7-48B9-C0D1-E2F3A4B5C6D7', 836.89),-- A2, VAT=TRUE, N/A turnover, ≥5T
    ('C5D6E7F8-A9B0-4C1D-2E3F-4A5B6C7D8E9F', 111.59),-- C2, VAT=FALSE, N/A turnover, ≥5T
    ('F8A9B0C1-D2E3-4F4A-5B6C-7D8E9F0A1B2C', 33.48), -- E, VAT=TRUE,  N/A turnover, <5T
    ('A0B1C2D3-E4F5-4A6B-7C8D-9E0F1A2B3C4D', 33.48)  -- E, VAT=FALSE, N/A turnover, <5T
) AS source ([Id], [Amount])
ON target.[Id] = source.[Id];

GO