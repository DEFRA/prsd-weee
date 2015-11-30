GO
PRINT N'Creating [Lookup].[Country]...';


GO
CREATE TABLE [Lookup].[Country] (
    [Id]                    UNIQUEIDENTIFIER NOT NULL,
    [Name]                  NVARCHAR (2048)  NOT NULL,
    CONSTRAINT [PK_Country_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
PRINT N'Update complete.';