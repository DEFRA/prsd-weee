GO
PRINT N'Altering [Lookup].[CompetentAuthority]...';

DECLARE @tblTempCompetentAuthorityTable TABLE (
[Id]            UNIQUEIDENTIFIER NOT NULL,
[Name]          NVARCHAR(1023) NOT NULL,
[Abbreviation]  NVARCHAR(65) NOT NULL,
[CountryId]     UNIQUEIDENTIFIER NOT NULL
)

INSERT INTO @tblTempCompetentAuthorityTable([Id], [Name], [Abbreviation], [CountryId])
VALUES
('A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8', 'Environment Agency', 'EA', '184E1785-26B4-4AE4-80D3-AE319B103ACB'),
('78F37814-364B-4FAE-BEB5-DB0439CBF177', 'Scottish Environment Protection Agency', 'SEPA', '4209EE95-0882-42F2-9A5D-355B4D89EF30'),
('4EEE5942-01B2-4A4D-855A-34DEE1BBBF26', 'Northern Ireland Environment Agency', 'NIEA', '7BFB8717-4226-40F3-BC51-B16FDF42550C'),
('44C2F368-AA66-48F0-BBC9-A0ED34AD0951', 'Natural Resources Wales', 'NRW', 'DB83F5AB-E745-49CF-B2CA-23FE391B67A8');

INSERT INTO [Lookup].[CompetentAuthority]([Id], [Name], [Abbreviation], [CountryId])
SELECT tmp.[Id], tmp.[Name], tmp.[Abbreviation], tmp.[CountryId]
FROM @tblTempCompetentAuthorityTable tmp
LEFT JOIN [Lookup].[CompetentAuthority] tbl ON tbl.[Id] = tmp.[Id]
WHERE tbl.[Id] IS NULL

UPDATE LiveTable SET
LiveTable.[Name] = tmp.[Name],
LiveTable.[Abbreviation] = tmp.[Abbreviation],
LiveTable.[CountryId] = tmp.[CountryId]
FROM [Lookup].[CompetentAuthority] LiveTable 
INNER JOIN @tblTempCompetentAuthorityTable tmp ON LiveTable.[Id] = tmp.[Id]

GO
PRINT N'Update complete.';

GO