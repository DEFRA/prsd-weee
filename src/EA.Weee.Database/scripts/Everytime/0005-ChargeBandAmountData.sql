GO
PRINT N'Altering [Lookup].[ChargeBandAmount]...';

DECLARE @tblTempChargeBandAmountTable TABLE (
[Id]            UNIQUEIDENTIFIER NOT NULL,
[ChargeBand]    INT NOT NULL,
[Amount]		DECIMAL(18,2) NOT NULL
)

INSERT INTO @tblTempChargeBandAmountTable([Id], [ChargeBand], [Amount])
VALUES
('469D87C5-260D-4EC3-8487-9D6B846F1898', 0, '445.00'),
('98B57733-023E-4261-9BF7-7D3F98EBC61B', 1, '210.00'),
('CE576193-6222-41A1-A811-8505BE83196E', 2, '30.00'),
('1D8554E4-98A9-47AA-9654-B78BE2EC3C58', 3, '30.00'),
('C1EF70C5-017F-458B-920B-F8F32796C9A8', 4, '30.00')

INSERT INTO [Lookup].[ChargeBandAmount]([Id], [ChargeBand], [Amount])
SELECT tmp.[Id], tmp.[ChargeBand], tmp.[Amount]
FROM @tblTempChargeBandAmountTable tmp
LEFT JOIN [Lookup].[ChargeBandAmount] tbl ON tbl.[Id] = tmp.[Id]
WHERE tbl.[Id] IS NULL

UPDATE LiveTable SET
LiveTable.[ChargeBand] = tmp.[ChargeBand],
LiveTable.[Amount] = tmp.[Amount]
FROM [Lookup].[ChargeBandAmount] LiveTable 
INNER JOIN @tblTempChargeBandAmountTable tmp ON LiveTable.[Id] = tmp.[Id]

GO
PRINT N'Update complete.';

GO