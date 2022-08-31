
DECLARE @tblEvidenceNoteProtocol TABLE  (
	Id INT NOT NULL, 
	[NAME] NVARCHAR(30) NOT NULL
)

INSERT INTO @tblEvidenceNoteProtocol (Id, [Name])
VALUES 
(1, 'Actual'),
(2, 'LDA protocol'),
(3, 'SMW protocol'),
(4, 'Site specific protocol'),
(5, 'Reuse network PWP'),
(6, 'Light iron protocol')

INSERT INTO [Lookup].[EvidenceNoteProtocol] (Id , [Name])
SELECT tmp.[Id], tmp.[NAME]
FROM @tblEvidenceNoteProtocol tmp
LEFT JOIN [Lookup].[EvidenceNoteProtocol] tbl ON tbl.[Id] = tmp.[Id]
WHERE tbl.[Id] IS NULL

UPDATE LiveTable SET
LiveTable.[Name] = tmp.[Name]
FROM [Lookup].[EvidenceNoteProtocol] LiveTable 
INNER JOIN @tblEvidenceNoteProtocol tmp ON LiveTable.[Id] = tmp.[Id]

GO
PRINT N'EvidenceNoteProtocol Update complete.';

GO