GO
PRINT N'Altering [AATF].[ReportOnQuestion]...';

DECLARE @tblReportOnQuestion TABLE  (
	Id INT NOT NULL, 
	[Question] NVARCHAR(300) NOT NULL,
	[Description] NVARCHAR(1000) NOT NULL,
	[ParentId] INT NULL,
	[AlternativeDescription] NVARCHAR(1000) NULL
)

INSERT INTO @tblReportOnQuestion (Id, [Question], [Description], [ParentId], [AlternativeDescription])
VALUES ('1', 'PCS', 'Obligated WEEE received on behalf of producer compliance schemes (PCSs)', NULL, NULL),
('2', 'SentToAnotherATF', 'Obligated WEEE sent to another AATF or ATF for treatment', NULL, NULL),
('3', 'WEEEReused', 'Obligated WEEE reused as a whole appliance', NULL, NULL),
('4', 'NonObligated', 'Non-obligated WEEE', NULL, NULL),
('5', 'NonObligatedDCF', 'Was any of this non-obligated WEEE received on behalf of a Designated Collection Facility (DCF)', 4, 'Non-obligated WEEE received on behalf of a DCF')

INSERT INTO [AATF].ReportOnQuestion (Id, [Question], [Description], [ParentId], [AlternativeDescription])
SELECT tmp.[Id], tmp.[Question], tmp.[Description], tmp.[ParentId], tmp.[AlternativeDescription]
FROM @tblReportOnQuestion tmp
LEFT JOIN [AATF].[ReportOnQuestion] tbl ON tbl.[Id] = tmp.[Id]
WHERE tbl.[Id] IS NULL

UPDATE LiveTable SET
LiveTable.[Id] = tmp.[Id],
LiveTable.[Question] = tmp.[Question],
LiveTable.[Description] = tmp.[Description],
LiveTable.[ParentId] = tmp.[ParentId],
LiveTable.[AlternativeDescription] = tmp.[AlternativeDescription]
FROM [AATF].[ReportOnQuestion] LiveTable 
INNER JOIN @tblReportOnQuestion tmp ON LiveTable.[Id] = tmp.[Id]

GO
PRINT N'Update complete.';

GO