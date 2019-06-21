GO
PRINT N'Altering [Lookup].[PanArea]...';

DECLARE @tblPanArea TABLE  (
	Id UNIQUEIDENTIFIER NOT NULL, 
	[NAME] NVARCHAR(200) NOT NULL,
	CompetentAuthorityId UNIQUEIDENTIFIER NOT NULL
)

INSERT INTO @tblPanArea (Id, [Name], CompetentAuthorityId)
VALUES ('13D97D30-B94D-491A-BA39-4ABB891917DF', 'North', 'A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8'),
('C0246EE7-BF99-4489-AAF3-B150D5E05D25', 'Midlands', 'A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8'),
('F5767376-6E4A-4C88-AB25-7EC075081BC4', 'South East', 'A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8'),
('4AE1BF04-9E70-4092-815D-B8EE33FE228A', 'South West', 'A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8')

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