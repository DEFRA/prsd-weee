PRINT N'Dropping [AATF].[AATFReturnScheme]...';
GO
IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'AATF' 
                 AND  TABLE_NAME = 'AATFReturnScheme'))
BEGIN
	ALTER TABLE [AATF].[AATFReturnScheme] DROP CONSTRAINT [FK_AATFReturnScheme_AATFReturn];
	DROP TABLE [AATF].[AATFReturnScheme];
END
GO

PRINT N'Creating [AATF].[ReturnScheme]...';
GO
CREATE TABLE [AATF].[ReturnScheme](
	[Id] [uniqueidentifier] NOT NULL,
	[SchemeId] [uniqueidentifier] NOT NULL,
	[ReturnId] [uniqueidentifier] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_ReturnScheme_Id] PRIMARY KEY CLUSTERED ([Id] ASC));
GO

PRINT N'Creating FK_ReturnScheme_Return...';
GO
ALTER TABLE [AATF].[ReturnScheme]  WITH NOCHECK ADD  CONSTRAINT [FK_ReturnScheme_Return] FOREIGN KEY([ReturnId])
REFERENCES [AATF].[Return] ([Id]);

PRINT N'Creating FK_ReturnScheme_Scheme...';
GO

ALTER TABLE [AATF].[ReturnScheme]  WITH NOCHECK ADD  CONSTRAINT [FK_ReturnScheme_Scheme] FOREIGN KEY([SchemeId])
REFERENCES [PCS].[Scheme] ([Id]);
