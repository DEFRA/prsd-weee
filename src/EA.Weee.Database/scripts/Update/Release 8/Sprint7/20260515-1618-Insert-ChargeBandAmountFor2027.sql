/* ===========================
   2027 CHARGEBANDAMOUNT INSERT
   =========================== */

SET NOCOUNT ON;
DECLARE @EffectiveFrom NVARCHAR(10) = '2027-01-01';
DECLARE @ComplianceYear INT = 2027;

INSERT INTO [Lookup].[ChargeBandAmount] (
    [Id], [Amount], [ChargeBand], [CompetentAuthority], [VatRegistered],
    [AnnualTurnoverBand], [EEEPlacedOnMarketBand], [ComplianceYear], [EffectiveFrom]
)
VALUES
    -- 2027 Data (Effective from 2027-04-01)
    -- Non-UK (0)
    ('4F296FF4-097D-40EE-BF4E-828325FFE664',    111.59, 7,  0,  0,  2,  0,  @ComplianceYear,    @EffectiveFrom), -- D2, VAT=FALSE, N/A turnover, ≥5T
    ('82C4E2BD-D691-4C61-BCD1-01AEF2422F67',    418.45, 8,  0,  1,  2,  0,  @ComplianceYear,    @EffectiveFrom), -- D3, VAT=TRUE,  N/A turnover, ≥5T
    ('83A52B20-B2EC-4FCA-A29A-3BB2B8A06804',    33.48,  4,  0,  1,  2,  1,  @ComplianceYear,    @EffectiveFrom), -- E, VAT=TRUE,  N/A turnover, <5T
    ('8F53E8A4-79E8-4638-B965-B8BA95C0D5A9',    33.48,  4,  0,  0,  2,  1,  @ComplianceYear,    @EffectiveFrom), -- E, VAT=FALSE, N/A turnover, <5T

    -- England (1)
    ('816D10DB-7573-4FB1-BED8-375D99889E33',    836.89, 5,  1,  1,  2,  0,  @ComplianceYear,    @EffectiveFrom), -- A2, VAT=TRUE, N/A turnover, ≥5T
    ('1857B7CA-2C0A-4630-97BB-96215DE4201A',    111.59, 6,  1,  0,  2,  0,  @ComplianceYear,    @EffectiveFrom), -- C2, VAT=FALSE, N/A turnover, ≥5T
    ('157C3CB3-5182-47CA-BC4A-FE9FDBE082E6',    33.48,  4,  1,  1,  2,  1,  @ComplianceYear,    @EffectiveFrom), -- E, VAT=TRUE,  N/A turnover, <5T
    ('7A635397-6F6C-48E6-BF52-9B6B2B2DF35B',    33.48,  4,  1,  0,  2,  1,  @ComplianceYear,    @EffectiveFrom), -- E, VAT=FALSE, N/A turnover, <5T

    -- Wales (2)
    ('2413D96E-92F8-4E74-93B9-89ADA47BC9DB',    445.00, 0,  2,  1,  1,  0,  @ComplianceYear,    @EffectiveFrom), -- A, VAT=TRUE, >£1m, ≥5T
    ('654489DA-674E-4A18-BF47-9E7FB278DE8B',    210.00, 1,  2,  1,  0,  0,  @ComplianceYear,    @EffectiveFrom), -- B, VAT=TRUE, <=£1m, ≥5T
    ('0B55182A-EAAD-4031-BA0B-46A960CD4932',    30.00,  2,  2,  0,  0,  0,  @ComplianceYear,    @EffectiveFrom), -- C, VAT=FALSE, <=£1m, ≥5T
    ('7041CCCC-6ACF-420E-AF47-465AE15AA202',    30.00,  3,  2,  0,  1,  0,  @ComplianceYear,    @EffectiveFrom), -- D, VAT=FALSE, >£1m, ≥5T
    ('985BB992-0492-402B-9A40-4ADE010F8F49',    30.00,  4,  2,  1,  1,  1,  @ComplianceYear,    @EffectiveFrom), -- E, VAT=TRUE, >£1m, <5T
    ('33EDE48A-75C2-4F72-B48D-792CCCAA73F0',    30.00,  4,  2,  1,  0,  1,  @ComplianceYear,    @EffectiveFrom), -- E, VAT=TRUE, <=£1m, <5T
    ('69136DF6-1CBD-4FC6-989D-FBC2D2F29E1E',    30.00,  4,  2,  0,  1,  1,  @ComplianceYear,    @EffectiveFrom), -- E, VAT=FALSE, >£1m, <5T
    ('B1647B05-5FB7-4D79-A83D-995DD97DD801',    30.00,  4,  2,  0,  0,  1,  @ComplianceYear,    @EffectiveFrom), -- E, VAT=FALSE, <=£1m, <5T

    -- Scotland (3)
    ('44C2C14D-E757-4859-A12D-A33D186E5796',    445.00, 0,  3,  1,  1,  0,  @ComplianceYear,    @EffectiveFrom), -- A, VAT=TRUE, >£1m, ≥5T
    ('28AF1FD1-2094-4A36-9808-CD549AFDA9FD',    210.00, 1,  3,  1,  0,  0,  @ComplianceYear,    @EffectiveFrom), -- B, VAT=TRUE, <=£1m, ≥5T
    ('C0AC7101-0664-4B67-AF35-1271E99B0400',    30.00,  2,  3,  0,  0,  0,  @ComplianceYear,    @EffectiveFrom), -- C, VAT=FALSE, <=£1m, ≥5T
    ('8939AA34-C735-433E-935D-BDA712A3704F',    30.00,  3,  3,  0,  1,  0,  @ComplianceYear,    @EffectiveFrom), -- D, VAT=FALSE, >£1m, ≥5T
    ('259D2BF4-0942-4602-B605-8206B729C4DE',    30.00,  4,  3,  1,  1,  1,  @ComplianceYear,    @EffectiveFrom), -- E, VAT=TRUE, >£1m, <5T
    ('72628AF5-BC4C-41C8-9181-251C19F72FF8',    30.00,  4,  3,  1,  0,  1,  @ComplianceYear,    @EffectiveFrom), -- E, VAT=TRUE, <=£1m, <5T
    ('75249CA3-B283-4C8B-A56F-23024705D847',    30.00,  4,  3,  0,  1,  1,  @ComplianceYear,    @EffectiveFrom), -- E, VAT=FALSE, >£1m, <5T
    ('3416725B-7AF0-47D1-9C45-35EA843F0232',    30.00,  4,  3,  0,  0,  1,  @ComplianceYear,    @EffectiveFrom), -- E, VAT=FALSE, <=£1m, <5T

    -- Northern Ireland (4)
    ('B8D9788C-7AD4-410C-B3D0-F5867765A387',    445.00, 0,  4,  1,  1,  0,  @ComplianceYear,    @EffectiveFrom), -- A, VAT=TRUE, >£1m, ≥5T
    ('5ED64242-57CF-4AEA-AC2D-7DB79D822A0E',    210.00, 1,  4,  1,  0,  0,  @ComplianceYear,    @EffectiveFrom), -- B, VAT=TRUE, <=£1m, ≥5T
    ('F65B748F-4FAF-4E29-962E-0AD953EF9060',    30.00,  2,  4,  0,  0,  0,  @ComplianceYear,    @EffectiveFrom), -- C, VAT=FALSE, <=£1m, ≥5T
    ('892BE1AF-572A-4113-B6EE-AED90A7E4FFE',    30.00,  3,  4,  0,  1,  0,  @ComplianceYear,    @EffectiveFrom), -- D, VAT=FALSE, >£1m, ≥5T
    ('FE30C6E4-0595-45D4-9639-CACD8B111764',    30.00,  4,  4,  1,  1,  1,  @ComplianceYear,    @EffectiveFrom), -- E, VAT=TRUE, >£1m, <5T
    ('903AF67D-EB65-416B-AD51-BC61A3ECDA77',    30.00,  4,  4,  1,  0,  1,  @ComplianceYear,    @EffectiveFrom), -- E, VAT=TRUE, <=£1m, <5T
    ('41416A6B-12DA-4181-9A19-26A9F1D5F201',    30.00,  4,  4,  0,  1,  1,  @ComplianceYear,    @EffectiveFrom), -- E, VAT=FALSE, >£1m, <5T
    ('DEE88927-06C4-4D27-85FF-35D90C241BBC',    30.00,  4,  4,  0,  0,  1,  @ComplianceYear,    @EffectiveFrom); -- E, VAT=FALSE, <=£1m, <5T

GO