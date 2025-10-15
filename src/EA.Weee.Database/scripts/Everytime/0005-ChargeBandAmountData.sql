GO
PRINT N'Altering [Lookup].[ChargeBandAmount]...';

DECLARE @tblTempChargeBandAmountTable TABLE (
[Id]                        UNIQUEIDENTIFIER NOT NULL,
[ChargeBand]                INT NOT NULL,
[Amount]                    DECIMAL(18,2) NOT NULL,
[CompetentAuthority]        INT NOT NULL,
[VatRegistered]             BIT NOT NULL,
[AnnualTurnoverBand]        INT NOT NULL,
[EEEPlacedOnMarketBand]     INT NOT NULL,
[ComplianceYear]            INT NOT NULL,
[EffectiveFrom]             DATETIME2(0) NOT NULL
)

INSERT INTO @tblTempChargeBandAmountTable([Id], [ChargeBand], [Amount], [CompetentAuthority], [VatRegistered], [AnnualTurnoverBand], [EEEPlacedOnMarketBand], [ComplianceYear], [EffectiveFrom])
VALUES
-- Legacy charge band amounts (ComplianceYear = 2000 for backward compatibility)
('469D87C5-260D-4EC3-8487-9D6B846F1898', 0, '445.00', 0, 0, 2, 0, 2000, '2000-01-01'),
('98B57733-023E-4261-9BF7-7D3F98EBC61B', 1, '210.00', 0, 0, 2, 0, 2000, '2000-01-01'),
('CE576193-6222-41A1-A811-8505BE83196E', 2, '30.00', 0, 0, 2, 0, 2000, '2000-01-01'),
('1D8554E4-98A9-47AA-9654-B78BE2EC3C58', 3, '30.00', 0, 0, 2, 0, 2000, '2000-01-01'),
('C1EF70C5-017F-458B-920B-F8F32796C9A8', 4, '30.00', 0, 0, 2, 1, 2000, '2000-01-01'),
('DC8545CD-0C38-40B8-87CF-30AFF1990742', 5, '750.00', 0, 0, 2, 0, 2000, '2000-01-01'),
('A8B0DB11-4158-4607-BED4-D476CCCCEB06', 6, '100.00', 0, 0, 2, 0, 2000, '2000-01-01'),
('D8B5CD20-84AE-4495-A012-3994D8DB921C', 7, '100.00', 0, 0, 2, 0, 2000, '2000-01-01'),
('7D196466-4DE5-47B4-9D27-1CD3D09571C7', 8, '375.00', 0, 0, 2, 0, 2000, '2000-01-01'),
('78441DEE-DB5F-4061-B6B8-EB6E48BF23B6', 9, '0', 0, 0, 2, 0, 2000, '2000-01-01')

INSERT INTO [Lookup].[ChargeBandAmount]([Id], [ChargeBand], [Amount], [CompetentAuthority], [VatRegistered], [AnnualTurnoverBand], [EEEPlacedOnMarketBand], [ComplianceYear], [EffectiveFrom])
SELECT tmp.[Id], tmp.[ChargeBand], tmp.[Amount], tmp.[CompetentAuthority], tmp.[VatRegistered], tmp.[AnnualTurnoverBand], tmp.[EEEPlacedOnMarketBand], tmp.[ComplianceYear], tmp.[EffectiveFrom]
FROM @tblTempChargeBandAmountTable tmp
LEFT JOIN [Lookup].[ChargeBandAmount] tbl ON tbl.[Id] = tmp.[Id]
WHERE tbl.[Id] IS NULL

UPDATE LiveTable SET
LiveTable.[ChargeBand] = tmp.[ChargeBand],
LiveTable.[Amount] = tmp.[Amount],
LiveTable.[CompetentAuthority] = tmp.[CompetentAuthority],
LiveTable.[VatRegistered] = tmp.[VatRegistered],
LiveTable.[AnnualTurnoverBand] = tmp.[AnnualTurnoverBand],
LiveTable.[EEEPlacedOnMarketBand] = tmp.[EEEPlacedOnMarketBand],
LiveTable.[ComplianceYear] = tmp.[ComplianceYear],
LiveTable.[EffectiveFrom] = tmp.[EffectiveFrom]
FROM [Lookup].[ChargeBandAmount] LiveTable 
INNER JOIN @tblTempChargeBandAmountTable tmp ON LiveTable.[Id] = tmp.[Id]

GO
PRINT N'Update complete.';

GO