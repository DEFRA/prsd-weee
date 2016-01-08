ALTER TABLE [dbo].[SystemData]
ADD
	[UseFixedComplianceYearAndQuarter] [bit] NOT NULL DEFAULT ((0)),
	[FixedComplianceYear] [int] NOT NULL DEFAULT(2016),
	[FixedQuarter] [int] NOT NULL DEFAULT(1)
GO
