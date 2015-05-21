CREATE SCHEMA [Identity]
GO


GO
PRINT N'Creating [Identity].[AspNetRoles]...';


GO
CREATE TABLE [Identity].[AspNetRoles] (
    [Id]   NVARCHAR (128) NOT NULL,
    [Name] NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_User.AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [Identity].[AspNetRoles].[RoleNameIndex]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex]
    ON [Identity].[AspNetRoles]([Name] ASC);


GO
PRINT N'Creating [Identity].[AspNetUserClaims]...';


GO
CREATE TABLE [Identity].[AspNetUserClaims] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [UserId]     NVARCHAR (128) NOT NULL,
    [ClaimType]  NVARCHAR (MAX) NULL,
    [ClaimValue] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_User.AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [Identity].[AspNetUserClaims].[IX_UserId]...';


GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [Identity].[AspNetUserClaims]([UserId] ASC);


GO
PRINT N'Creating [Identity].[AspNetUserLogins]...';


GO
CREATE TABLE [Identity].[AspNetUserLogins] (
    [LoginProvider] NVARCHAR (128) NOT NULL,
    [ProviderKey]   NVARCHAR (128) NOT NULL,
    [UserId]        NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_User.AspNetUserLogins] PRIMARY KEY CLUSTERED ([LoginProvider] ASC, [ProviderKey] ASC, [UserId] ASC)
);


GO
PRINT N'Creating [Identity].[AspNetUserLogins].[IX_UserId]...';


GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [Identity].[AspNetUserLogins]([UserId] ASC);


GO
PRINT N'Creating [Identity].[AspNetUserRoles]...';


GO
CREATE TABLE [Identity].[AspNetUserRoles] (
    [UserId] NVARCHAR (128) NOT NULL,
    [RoleId] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_User.AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC)
);


GO
PRINT N'Creating [Identity].[AspNetUserRoles].[IX_RoleId]...';


GO
CREATE NONCLUSTERED INDEX [IX_RoleId]
    ON [Identity].[AspNetUserRoles]([RoleId] ASC);


GO
PRINT N'Creating [Identity].[AspNetUserRoles].[IX_UserId]...';


GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [Identity].[AspNetUserRoles]([UserId] ASC);


GO
PRINT N'Creating [Identity].[AspNetUsers]...';


GO
CREATE TABLE [Identity].[AspNetUsers] (
    [Id]                   NVARCHAR (128) NOT NULL,
    [Email]                NVARCHAR (256) NULL,
    [EmailConfirmed]       BIT            NOT NULL,
    [PasswordHash]         NVARCHAR (MAX) NULL,
    [SecurityStamp]        NVARCHAR (MAX) NULL,
    [PhoneNumber]          NVARCHAR (MAX) NULL,
    [PhoneNumberConfirmed] BIT            NOT NULL,
    [TwoFactorEnabled]     BIT            NOT NULL,
    [LockoutEndDateUtc]    DATETIME       NULL,
    [LockoutEnabled]       BIT            NOT NULL,
    [AccessFailedCount]    INT            NOT NULL,
    [UserName]             NVARCHAR (256) NOT NULL
    CONSTRAINT [PK_User.AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [Identity].[AspNetUsers].[UserNameIndex]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
    ON [Identity].[AspNetUsers]([UserName] ASC);


GO
PRINT N'Creating FK_User.AspNetUserClaims_User.AspNetUsers_UserId...';


GO
ALTER TABLE [Identity].[AspNetUserClaims] WITH NOCHECK
    ADD CONSTRAINT [FK_User.AspNetUserClaims_User.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[AspNetUsers] ([Id]) ON DELETE CASCADE;


GO
PRINT N'Creating FK_User.AspNetUserLogins_User.AspNetUsers_UserId...';


GO
ALTER TABLE [Identity].[AspNetUserLogins] WITH NOCHECK
    ADD CONSTRAINT [FK_User.AspNetUserLogins_User.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[AspNetUsers] ([Id]) ON DELETE CASCADE;


GO
PRINT N'Creating FK_User.AspNetUserRoles_User.AspNetUsers_UserId...';


GO
ALTER TABLE [Identity].[AspNetUserRoles] WITH NOCHECK
    ADD CONSTRAINT [FK_User.AspNetUserRoles_User.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[AspNetUsers] ([Id]) ON DELETE CASCADE;


GO
PRINT N'Creating FK_User.AspNetUserRoles_User.AspNetRoles_RoleId...';


GO
ALTER TABLE [Identity].[AspNetUserRoles] WITH NOCHECK
    ADD CONSTRAINT [FK_User.AspNetUserRoles_User.AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Identity].[AspNetRoles] ([Id]) ON DELETE CASCADE;


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [Identity].[AspNetUserClaims] WITH CHECK CHECK CONSTRAINT [FK_User.AspNetUserClaims_User.AspNetUsers_UserId];

ALTER TABLE [Identity].[AspNetUserLogins] WITH CHECK CHECK CONSTRAINT [FK_User.AspNetUserLogins_User.AspNetUsers_UserId];

ALTER TABLE [Identity].[AspNetUserRoles] WITH CHECK CHECK CONSTRAINT [FK_User.AspNetUserRoles_User.AspNetUsers_UserId];

ALTER TABLE [Identity].[AspNetUserRoles] WITH CHECK CHECK CONSTRAINT [FK_User.AspNetUserRoles_User.AspNetRoles_RoleId];

GO
PRINT N'Creating [dbo].[__MigrationHistory]...';


GO
CREATE TABLE [dbo].[__MigrationHistory] (
    [MigrationId]    NVARCHAR (150)  NOT NULL,
    [ContextKey]     NVARCHAR (300)  NOT NULL,
    [Model]          VARBINARY (MAX) NOT NULL,
    [ProductVersion] NVARCHAR (32)   NOT NULL,
    CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED ([MigrationId] ASC, [ContextKey] ASC)
);

GO
PRINT N'Creating [Identity].[Consents]...';


GO
CREATE TABLE [Identity].[Consents] (
    [Subject]  NVARCHAR (200)  NOT NULL,
    [ClientId] NVARCHAR (200)  NOT NULL,
    [Scopes]   NVARCHAR (2000) NOT NULL,
    CONSTRAINT [PK_Identity.Consents] PRIMARY KEY CLUSTERED ([Subject] ASC, [ClientId] ASC)
);


GO
PRINT N'Creating [Identity].[Tokens]...';


GO
CREATE TABLE [Identity].[Tokens] (
    [Key]       NVARCHAR (128)     NOT NULL,
    [TokenType] SMALLINT           NOT NULL,
    [SubjectId] NVARCHAR (200)     NULL,
    [ClientId]  NVARCHAR (200)     NOT NULL,
    [JsonCode]  NVARCHAR (MAX)     NOT NULL,
    [Expiry]    DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Identity.Tokens] PRIMARY KEY CLUSTERED ([Key] ASC, [TokenType] ASC)
);


GO


PRINT N'Update complete.';
GO