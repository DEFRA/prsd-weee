GO
PRINT N'Altering [AATF].[ReportOnQuestion]...';

DECLARE @tblReportOnQuestion TABLE  (
	Id INT NOT NULL, 
	[Question] NVARCHAR(300) NOT NULL,
	[Description] NVARCHAR(1000) NOT NULL,
	[ParentId] INT NULL,
	[AlternativeDescription] NVARCHAR(1000) NULL,
	Title NVARCHAR(1000) NOT NULL
)

INSERT INTO @tblReportOnQuestion (Id, [Question], [Description], [ParentId], [AlternativeDescription], [Title])
VALUES ('1', 'PCS', 'This is WEEE from designated collection facilities (DCFs), distributors and final holders which a PCS has arranged for you to treat', NULL, NULL, 'Obligated WEEE received on behalf of producer compliance schemes (PCSs)'),
('2', 'SentToAnotherATF', 'This is WEEE that you received at your AATF(s) which you have not treated and was sent on for treatment elsewhere', NULL, NULL, 'Obligated WEEE sent to another AATF or ATF for treatment'),
('3', 'WEEEReused', 'This is WEEE that has been refurbished or repaired to the point it is no longer waste', NULL, NULL, 'Obligated WEEE reused as a whole appliance'),
('4', 'NonObligated', 'This is WEEE that you received at your AATF(s) other than from or on behalf of a PCS. It should not include WEEE that has already been reported by another AATF(s).', NULL, NULL, 'Non-obligated WEEE'),
('5', 'NonObligatedDCF', 'This is WEEE that a local authority who is a DCF operator (for example at a civic amenity site / household waste collection site) has been allowed to keep / retain under regulation 53 and has sent to you for treatment', 4, 'Non-obligated WEEE received on behalf of a DCF', 'Was any of this non-obligated WEEE received on behalf of a DCF?')

INSERT INTO [AATF].ReportOnQuestion (Id, [Question], [Description], [ParentId], [AlternativeDescription], [Title])
SELECT tmp.[Id], tmp.[Question], tmp.[Description], tmp.[ParentId], tmp.[AlternativeDescription], tmp.[Title]
FROM @tblReportOnQuestion tmp
LEFT JOIN [AATF].[ReportOnQuestion] tbl ON tbl.[Id] = tmp.[Id]
WHERE tbl.[Id] IS NULL

UPDATE LiveTable SET
LiveTable.[Id] = tmp.[Id],
LiveTable.[Question] = tmp.[Question],
LiveTable.[Description] = tmp.[Description],
LiveTable.[ParentId] = tmp.[ParentId],
LiveTable.[AlternativeDescription] = tmp.[AlternativeDescription],
LiveTable.[Title] = tmp.[Title]
FROM [AATF].[ReportOnQuestion] LiveTable 
INNER JOIN @tblReportOnQuestion tmp ON LiveTable.[Id] = tmp.[Id]

GO
PRINT N'Update complete.';

GO