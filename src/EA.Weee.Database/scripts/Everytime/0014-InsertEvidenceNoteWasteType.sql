
DECLARE @tblEvidenceNoteWasteType TABLE  (
	Id INT NOT NULL, 
	[NAME] NVARCHAR(20) NOT NULL
)

INSERT INTO @tblEvidenceNoteWasteType (Id, [Name])
VALUES 
(1, 'Household'),
(2, 'Non-household')

INSERT INTO [Lookup].[EvidenceNoteWasteType] (Id , [Name])
SELECT tmp.[Id], tmp.[NAME]
FROM @tblEvidenceNoteWasteType tmp
LEFT JOIN [Lookup].[EvidenceNoteWasteType] tbl ON tbl.[Id] = tmp.[Id]
WHERE tbl.[Id] IS NULL

UPDATE LiveTable SET
LiveTable.[Name] = tmp.[Name]
FROM [Lookup].[EvidenceNoteWasteType] LiveTable 
INNER JOIN @tblEvidenceNoteWasteType tmp ON LiveTable.[Id] = tmp.[Id]

GO
PRINT N'EvidenceNoteWasteType Update complete.';

GO