
DECLARE @tblEvidenceNoteStatus TABLE  (
	Id INT NOT NULL, 
	[NAME] NVARCHAR(20) NOT NULL
)

INSERT INTO @tblEvidenceNoteStatus (Id, [Name])
VALUES 
(1, 'Draft'),
(2, 'Submitted'),
(3, 'Approved'),
(4, 'Rejected'),
(5, 'Void'),
(6, 'Returned')

INSERT INTO [Lookup].[EvidenceNoteStatus] (Id , [Name])
SELECT tmp.[Id], tmp.[NAME]
FROM @tblEvidenceNoteStatus tmp
LEFT JOIN [Lookup].[EvidenceNoteStatus] tbl ON tbl.[Id] = tmp.[Id]
WHERE tbl.[Id] IS NULL

UPDATE LiveTable SET
LiveTable.[Name] = tmp.[Name]
FROM [Lookup].[EvidenceNoteStatus] LiveTable 
INNER JOIN @tblEvidenceNoteStatus tmp ON LiveTable.[Id] = tmp.[Id]

GO
PRINT N'EvidenceNoteStatus Update complete.';

GO