PRINT N'Altering [Producer].[BrandName]...';
GO

ALTER TABLE [Producer].[BrandName] 
Alter Column [Name] nvarchar(255) NOT NULL;
GO

PRINT N'Altering [Producer].[Address]...';

GO
ALTER TABLE [Producer].[Address] 
Alter Column [Street] nvarchar(100) NOT NULL;

GO
ALTER TABLE [Producer].[Address] 
Alter Column  [Locality] nvarchar(35) NOT NULL;

GO
ALTER TABLE [Producer].[Address] 
Alter Column  [Town] nvarchar(30) NOT NULL;
GO

PRINT N'Altering [Producer].[SICCode]...';
GO

ALTER TABLE [Producer].[SICCode] 
Alter Column [Name] nvarchar(8) NOT NULL;
GO

PRINT N'Altering [Producer].[Contact]...';
GO

ALTER TABLE [Producer].[Contact] 
Alter Column [Email] nvarchar(255) NOT NULL;
GO

PRINT N'Altering [Producer].[Company]...';
GO

ALTER TABLE [Producer].[Company] 
Alter Column [Name] nvarchar(255) NOT NULL;
GO

PRINT N'Update complete.';