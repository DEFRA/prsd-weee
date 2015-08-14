GO
PRINT N'Creating [Admin]...';


GO
CREATE SCHEMA [Admin]
    AUTHORIZATION [dbo];

GO
PRINT N'Creating [Admin].[CompetentAuthorityUser]...';
GO
CREATE TABLE [Admin].[CompetentAuthorityUser](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[CompetentAuthorityId] [uniqueidentifier] NOT NULL,
	[CompetentAuthorityUserStatus] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_CompetentAuthorityUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Admin].[CompetentAuthorityUser]  WITH CHECK ADD  CONSTRAINT [FK_CompetentAuthorityUser_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [Identity].[AspNetUsers] ([Id])
GO

ALTER TABLE [Admin].[CompetentAuthorityUser] CHECK CONSTRAINT [FK_CompetentAuthorityUser_AspNetUsers]
GO

ALTER TABLE [Admin].[CompetentAuthorityUser]  WITH CHECK ADD  CONSTRAINT [FK_CompetentAuthorityUser_CompetentAuthority] FOREIGN KEY([CompetentAuthorityId])
REFERENCES [Lookup].[CompetentAuthority] ([Id])
GO

ALTER TABLE [Admin].[CompetentAuthorityUser] CHECK CONSTRAINT [FK_CompetentAuthorityUser_CompetentAuthority]

GO
PRINT N'Update complete.';
GO
