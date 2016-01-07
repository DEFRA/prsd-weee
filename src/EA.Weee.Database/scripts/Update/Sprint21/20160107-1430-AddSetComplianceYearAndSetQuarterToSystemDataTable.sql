ALTER TABLE [dbo].[SystemData]
ADD
	[UseSetComplianceYearAndQuarter] [bit] NOT NULL DEFAULT ((0)),
	[SetComplianceYear] [int] NOT NULL DEFAULT ((2016)),
	[SetQuarter] [int] NOT NULL DEFAULT ((1))
GO
