GO
PRINT N'Altering [Lookup].[PanArea]...';

DECLARE @tblPanArea TABLE  (
	Id UNIQUEIDENTIFIER NOT NULL, 
	[NAME] NVARCHAR(1024) NOT NULL,
	CompetentAuthorityId GUID NOT NULL
)

INSERT INTO @tblPanArea (Id, [Name], CompetentAuthorityId)
VALUES ('d24b9d56-5fca-497d-843a-c8ec81c388b6', 'North', 'A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8'),
VALUES ('14221944-5598-4ad9-84c0-1ca40f1049d4', 'Midlands', 'A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8'),
VALUES ('ca403bf3-d2f1-4e2f-9942-2e2c227f52f7', 'South east', 'A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8'),
VALUES ('99ac80c6-a26b-42ce-a516-a0006367f00c', 'South west', 'A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8')

INSERT INTO [Lookup].PanArea (Id , [Name], CompetentAuthorityId)
SELECT tmp.[Id], tmp.[NAME], tmp.CompetentAuthorityId
FROM @tblPanArea tmp
LEFT JOIN [Lookup].[PanArea] tbl ON tbl.[Id] = tmp.[Id]
WHERE tbl.[Id] IS NULL

UPDATE LiveTable SET
LiveTable.[Name] = tmp.[Name],
LiveTable.CompetentAuthorityId = tmp.CompetentAuthorityId
FROM [Lookup].[PanArea] LiveTable 
INNER JOIN @tblPanArea tmp ON LiveTable.[Id] = tmp.[Id]

GO
PRINT N'Update complete.';

GO