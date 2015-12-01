GO
PRINT N'Creating [Producer].[Producer].[IsCurrentForComplianceYear]...';

GO
ALTER TABLE [Producer].[Producer] ADD
	[IsCurrentForComplianceYear] BIT NOT NULL CONSTRAINT [DF_Producer_IsCurrentForComplianceYear] DEFAULT 0

GO    
PRINT N'Update complete.';

GO