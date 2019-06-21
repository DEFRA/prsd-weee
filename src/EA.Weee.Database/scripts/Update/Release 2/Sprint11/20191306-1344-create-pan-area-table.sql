CREATE TABLE [Lookup].[PanArea] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
    [Name]						  NVARCHAR (200) NOT NULL,
	[CompetentAuthorityId]		  UNIQUEIDENTIFIER NOT NULL

    CONSTRAINT [PK_PanArea_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

ALTER TABLE [Lookup].[PanArea] WITH NOCHECK
    ADD CONSTRAINT [FK_PanArea_CompetentAuthority] FOREIGN KEY ([CompetentAuthorityId]) REFERENCES [Lookup].[CompetentAuthority] ([Id]);