GO
PRINT N'Altering [Lookup].[LocalArea]...';

DECLARE @tblLocalArea TABLE  (
	Id UNIQUEIDENTIFIER NOT NULL, 
	[NAME] NVARCHAR(1024) NOT NULL,
	CompetentAuthorityId GUID NOT NULL
)

INSERT INTO @tblLocalArea (Id, [Name], CompetentAuthorityId)
VALUES ('', '', '')

INSERT INTO [Lookup].LocalArea (Id , [Name], CompetentAuthorityId)
SELECT tmp.[Id], tmp.[NAME], tmp.CompetentAuthorityId
FROM @tblLocalArea tmp
LEFT JOIN [Lookup].[LocalArea] tbl ON tbl.[Id] = tmp.[Id]
WHERE tbl.[Id] IS NULL

UPDATE LiveTable SET
LiveTable.[Name] = tmp.[Name],
LiveTable.CompetentAuthorityId = tmp.CompetentAuthorityId
FROM [Lookup].[LocalArea] LiveTable 
INNER JOIN @tblLocalArea tmp ON LiveTable.[Id] = tmp.[Id]

GO
PRINT N'Update complete.';

GO