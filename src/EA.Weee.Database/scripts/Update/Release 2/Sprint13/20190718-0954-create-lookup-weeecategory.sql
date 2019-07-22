GO
PRINT N'Creating [Lookup].[WeeeCategory]...';
GO
CREATE TABLE [Lookup].[WeeeCategory](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](60) NOT NULL,
 CONSTRAINT [PK_WeeeCategory_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
));
GO
PRINT N'Complete...';

