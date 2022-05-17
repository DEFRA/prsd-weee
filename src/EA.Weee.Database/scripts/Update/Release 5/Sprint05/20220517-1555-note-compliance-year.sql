
IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'ComplianceYear'
          AND Object_ID = Object_ID(N'[Evidence].[Note]'))
BEGIN
    ALTER TABLE [Evidence].[Note] ADD [ComplianceYear] [smallint] NULL;
END

UPDATE [Evidence].Note SET ComplianceYear = Year(StartDate)


ALTER TABLE [Evidence].[Note] ALTER COLUMN [ComplianceYear] [smallint] NOT NULL;