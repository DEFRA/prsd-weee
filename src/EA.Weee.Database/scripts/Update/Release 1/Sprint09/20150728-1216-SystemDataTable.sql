

GO
PRINT N'Creating [dbo].[SystemData]...';


GO
CREATE TABLE [dbo].[SystemData] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [LatestPRNSeed] BIGINT           NOT NULL,
    [RowVersion]    ROWVERSION       NOT NULL,
    CONSTRAINT [PK_SystemData] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Update complete.';


GO
