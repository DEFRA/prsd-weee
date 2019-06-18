GO
PRINT N'Creating [Lookup].[LocalArea]...';
GO
CREATE TABLE [Lookup].[LocalArea] (
    [Id]         UNIQUEIDENTIFIER NOT NULL,
    [Name]       NVARCHAR (1024)  NOT NULL,
	[CompetentAuthorityId] UNIQUEIDENTIFIER NOT NULL CONSTRAINT FK_LocalArea_CompetentAuthorityId FOREIGN KEY REFERENCES [Lookup].CompetentAuthority(Id)
    CONSTRAINT [PK_LocalArea_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
PRINT N'Complete...';