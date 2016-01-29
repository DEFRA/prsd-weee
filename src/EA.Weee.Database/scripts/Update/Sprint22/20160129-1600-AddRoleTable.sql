GO
CREATE SCHEMA [Security]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Security].[Role]
(
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](100) NOT NULL
	CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([Id] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT [Security].[Role]
VALUES
('1E23EF8A-5E91-47CD-889F-A556263A5F3F', 'InternalAdmin', 'Administrator'),
('B673CD78-0C47-4C1C-BD0E-BF366A344EC6', 'InternalUser', 'Standard user');

GO

ALTER TABLE [Admin].[CompetentAuthorityUser]
ADD [RoleId] [uniqueidentifier] NULL;

GO

DECLARE @standardUserId [uniqueidentifier];
SET @standardUserId = (SELECT Id FROM [Security].[Role] WHERE Name = 'InternalUser')

UPDATE CAU
SET CAU.RoleId = @standardUserId
FROM [Admin].[CompetentAuthorityUser] CAU

GO

ALTER TABLE [Admin].[CompetentAuthorityUser]
ALTER COLUMN [RoleId] [uniqueidentifier] NOT NULL;

GO

ALTER TABLE [Admin].[CompetentAuthorityUser] WITH CHECK ADD CONSTRAINT [FK_CompetentAuthorityUser_Role] FOREIGN KEY([RoleId])
REFERENCES [Security].[Role] ([Id])
GO

ALTER TABLE [Admin].[CompetentAuthorityUser] CHECK CONSTRAINT [FK_CompetentAuthorityUser_Role]
GO