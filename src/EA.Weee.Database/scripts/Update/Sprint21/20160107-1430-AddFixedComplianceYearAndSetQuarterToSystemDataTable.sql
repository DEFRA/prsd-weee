ALTER TABLE [dbo].[SystemData]
ADD
	[UseFixedComplianceYearAndQuarter] [bit] NOT NULL DEFAULT ((0)),
	[FixedComplianceYear] [int] NULL,
	[FixedQuarter] [int] NULL
GO