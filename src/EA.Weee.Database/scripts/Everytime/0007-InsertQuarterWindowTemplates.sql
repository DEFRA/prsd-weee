GO
PRINT N'Altering [Lookup].[QuarterWindowTemplate]...';

DECLARE @tblTempQuarterWindowTemplate TABLE  (
	Id UNIQUEIDENTIFIER NOT NULL, 
	Quarter int NOT NULL,
	AddStartYears int NOT NULL,
	StartMonth int NOT NULL,
	StartDay int NOT NULL,
	AddEndYears int NOT NULL,
	EndMonth int NOT NULL,
	EndDay int NOT NULL
)

INSERT INTO @tblTempQuarterWindowTemplate (Id, Quarter, AddStartYears, StartMonth, StartDay, AddEndYears, EndMonth, EndDay)
VALUES ('457699FF-41BB-4DCB-B02C-37C7CBE00C51', 1, 0, 4, 1, 1, 3, 16),
('6402DDE4-0EEE-4299-AAA5-FA155870CF9A', 2, 0, 7, 1, 1, 3, 16),
('D388464A-ED9F-46E2-A123-8898E8D6766A', 3, 0, 10, 1, 1, 3, 16),
('5CDB440C-8204-45B3-938A-C87015D49DA5', 4, 1, 1, 1, 1, 3, 16)

INSERT INTO [Lookup].[QuarterWindowTemplate] (Id , Quarter, AddStartYears, StartMonth, StartDay, AddEndYears, EndMonth, EndDay) 
SELECT tmp.[Id], tmp.[Quarter], tmp.[AddStartYears], tmp.[StartMonth], tmp.[StartDay], tmp.[AddEndYears], tmp.[EndMonth], tmp.[EndDay]
FROM @tblTempQuarterWindowTemplate tmp
LEFT JOIN [Lookup].[QuarterWindowTemplate] tbl ON tbl.[Id] = tmp.[Id]
WHERE tbl.[Id] IS NULL

GO
PRINT N'Update complete.';

GO