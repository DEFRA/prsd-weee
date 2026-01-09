IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '[Lookup].[MessageBanner]')
BEGIN
    CREATE TABLE [Lookup].[MessageBanner] (
    [Id]            INT IDENTITY (1, 1) NOT NULL,
    [Title]         NVARCHAR(250),
    [StartTime]     DATETIME,
    [EndTime]       DATETIME,
    [Description]   NVARCHAR(1500),
     CONSTRAINT [PK_MessageBanner] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END

