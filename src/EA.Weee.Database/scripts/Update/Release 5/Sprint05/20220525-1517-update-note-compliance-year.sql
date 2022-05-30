
IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'ComplianceYear'
          AND Object_ID = Object_ID(N'[Evidence].[Note]'))
BEGIN
    ALTER TABLE [Evidence].[Note] ALTER COLUMN [ComplianceYear] INT NOT NULL;
END
GO
