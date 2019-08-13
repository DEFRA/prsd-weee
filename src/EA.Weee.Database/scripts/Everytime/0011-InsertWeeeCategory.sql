GO
PRINT N'Altering [Lookup].[WeeeCategory]...';

DECLARE @tblLocalArea TABLE  (
	Id INT NOT NULL, 
	[NAME] NVARCHAR(1024) NOT NULL
)

INSERT INTO @tblLocalArea (Id, [Name])
VALUES 
(1, 'Large household appliances'),
(2, 'Small household appliances'),
(3, 'IT and telecommunications equipment'),
(4, 'Consumer equipment'),
(5, 'Lighting equipment'),
(6, 'Electrical and electronic tools'),
(7, 'Toys, leisure and sports equipment'),
(8, 'Medical devices'),
(9, 'Monitoring and control instruments'),
(10, 'Automatic dispensers'),
(11, 'Display equipment'),
(12, 'Appliances containing refrigerants'),
(13, 'Gas discharge lamps and LED light sources'),
(14, 'Photovoltaic panels')


INSERT INTO [Lookup].[WeeeCategory] (Id , [Name])
SELECT tmp.[Id], tmp.[NAME]
FROM @tblLocalArea tmp
LEFT JOIN [Lookup].[WeeeCategory] tbl ON tbl.[Id] = tmp.[Id]
WHERE tbl.[Id] IS NULL

UPDATE LiveTable SET
LiveTable.[Name] = tmp.[Name]
FROM [Lookup].[WeeeCategory] LiveTable 
INNER JOIN @tblLocalArea tmp ON LiveTable.[Id] = tmp.[Id]

GO
PRINT N'Update complete.';

GO