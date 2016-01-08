UPDATE dbo.SystemData
SET
	[UseFixedComplianceYearAndQuarter] = @FixQuarter
    ,[FixedComplianceYear] = @ComplianceYear
    ,[FixedQuarter] = @Quarter