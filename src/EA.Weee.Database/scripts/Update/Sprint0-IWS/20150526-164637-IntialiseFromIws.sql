
PRINT N'Creating [Auditing]...';


GO
CREATE SCHEMA [Auditing]
    AUTHORIZATION [dbo];


GO
PRINT N'Creating [Organisation]...';


GO
CREATE SCHEMA [Organisation]
    AUTHORIZATION [dbo];


GO
PRINT N'Creating [Identity]...';


GO
CREATE SCHEMA [Identity]
    AUTHORIZATION [dbo];


GO
PRINT N'Creating [Lookup]...';


GO
CREATE SCHEMA [Lookup]
    AUTHORIZATION [dbo];

GO
PRINT N'Creating [Auditing].[AuditLog]...';


GO
CREATE TABLE [Auditing].[AuditLog] (
    [Id]            INT              IDENTITY (1, 1) NOT NULL,
    [UserId]        UNIQUEIDENTIFIER NOT NULL,
    [EventDate]     DATETIME2 (7)    NOT NULL,
    [EventType]     INT              NOT NULL,
    [TableName]     NVARCHAR (256)   NOT NULL,
    [RecordId]      UNIQUEIDENTIFIER NOT NULL,
    [OriginalValue] NVARCHAR (MAX)   NULL,
    [NewValue]      NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_AuditLog] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [Organisation].[Organisation]...';


GO
CREATE TABLE [Organisation].[Organisation] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
    [Name]                        NVARCHAR (2048)  NOT NULL,
    [Type]                        NVARCHAR (64)    NOT NULL,
    [RowVersion]                  ROWVERSION       NOT NULL,
    [RegistrationNumber]          NVARCHAR (64)    NULL,
    [AditionalRegistrationNumber] NVARCHAR (64)    NULL,
    [Building]                    NVARCHAR (1024)  NOT NULL,
    [Address1]                    NVARCHAR (1024)  NOT NULL,
    [TownOrCity]                  NVARCHAR (1024)  NOT NULL,
    [Address2]                    NVARCHAR (1024)  NULL,
    [Region]                      NVARCHAR (1024)  NULL,
    [PostalCode]                  NVARCHAR (64)    NOT NULL,
    [Country]                     NVARCHAR (1024)  NOT NULL,
    [FirstName]                   NVARCHAR (150)   NULL,
    [LastName]                    NVARCHAR (150)   NULL,
    [Telephone]                   NVARCHAR (150)   NULL,
    [Fax]                         NVARCHAR (150)   NULL,
    [Email]                       NVARCHAR (150)   NULL,
    CONSTRAINT [PK_Organisation_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [Organisation].[Address]...';


GO
CREATE TABLE [Organisation].[Address] (
    [Id]         UNIQUEIDENTIFIER NOT NULL,
    [Building]   NVARCHAR (1024)  NOT NULL,
    [Address1]   NVARCHAR (1024)  NULL,
    [TownOrCity] NVARCHAR (1024)  NOT NULL,
    [Address2]   NVARCHAR (1024)  NULL,
    [PostalCode] NVARCHAR (64)    NOT NULL,
    [Country]	 NVARCHAR (64)	  NOT NULL,
    [RowVersion] ROWVERSION       NOT NULL,
    CONSTRAINT [PK_Address_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
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
PRINT N'Creating [Identity].[AspNetUsers]...';


GO
CREATE TABLE [Identity].[AspNetUsers] (
    [Id]                   NVARCHAR (128)   NOT NULL,
    [Email]                NVARCHAR (256)   NULL,
    [EmailConfirmed]       BIT              NOT NULL,
    [PasswordHash]         NVARCHAR (MAX)   NULL,
    [SecurityStamp]        NVARCHAR (MAX)   NULL,
    [PhoneNumber]          NVARCHAR (MAX)   NULL,
    [PhoneNumberConfirmed] BIT              NOT NULL,
    [TwoFactorEnabled]     BIT              NOT NULL,
    [LockoutEndDateUtc]    DATETIME         NULL,
    [LockoutEnabled]       BIT              NOT NULL,
    [AccessFailedCount]    INT              NOT NULL,
    [UserName]             NVARCHAR (256)   NOT NULL,
    [FirstName]            NVARCHAR (256)   NULL,
    [Surname]              NVARCHAR (256)   NULL,
    [OrganisationId]       UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_User.AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [Identity].[AspNetUsers].[UserNameIndex]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
    ON [Identity].[AspNetUsers]([UserName] ASC);


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
PRINT N'Creating [Identity].[Consents]...';


GO
CREATE TABLE [Identity].[Consents] (
    [Subject]  NVARCHAR (200)  NOT NULL,
    [ClientId] NVARCHAR (200)  NOT NULL,
    [Scopes]   NVARCHAR (2000) NOT NULL,
    CONSTRAINT [PK_Identity.Consents] PRIMARY KEY CLUSTERED ([Subject] ASC, [ClientId] ASC)
);


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
PRINT N'Creating [Lookup].[CompetentAuthority]...';


GO
CREATE TABLE [Lookup].[CompetentAuthority] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [Name]         NVARCHAR (1023)  NOT NULL,
    [Abbreviation] NVARCHAR (63)    NULL,
    [IsSystemUser] BIT              NOT NULL,
    [RowVersion]   ROWVERSION       NOT NULL,
    CONSTRAINT [PK_CompetentAuthority_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);


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
PRINT N'Creating DF_CompetentAuthority_IsSystemUser...';


GO
ALTER TABLE [Lookup].[CompetentAuthority]
    ADD CONSTRAINT [DF_CompetentAuthority_IsSystemUser] DEFAULT ((0)) FOR [IsSystemUser];


GO
PRINT N'Creating FK_AspNetUsers_Organisation...';


GO
ALTER TABLE [Identity].[AspNetUsers] WITH NOCHECK
    ADD CONSTRAINT [FK_AspNetUsers_Organisation] FOREIGN KEY ([OrganisationId]) REFERENCES [Organisation].[Organisation] ([Id]);


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
PRINT N'Creating FK_User.AspNetUserClaims_User.AspNetUsers_UserId...';


GO
ALTER TABLE [Identity].[AspNetUserClaims] WITH NOCHECK
    ADD CONSTRAINT [FK_User.AspNetUserClaims_User.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[AspNetUsers] ([Id]) ON DELETE CASCADE;


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO

ALTER TABLE [Identity].[AspNetUsers] WITH CHECK CHECK CONSTRAINT [FK_AspNetUsers_Organisation];

ALTER TABLE [Identity].[AspNetUserLogins] WITH CHECK CHECK CONSTRAINT [FK_User.AspNetUserLogins_User.AspNetUsers_UserId];

ALTER TABLE [Identity].[AspNetUserRoles] WITH CHECK CHECK CONSTRAINT [FK_User.AspNetUserRoles_User.AspNetUsers_UserId];

ALTER TABLE [Identity].[AspNetUserRoles] WITH CHECK CHECK CONSTRAINT [FK_User.AspNetUserRoles_User.AspNetRoles_RoleId];

ALTER TABLE [Identity].[AspNetUserClaims] WITH CHECK CHECK CONSTRAINT [FK_User.AspNetUserClaims_User.AspNetUsers_UserId];


GO
PRINT N'Update complete.';


GO
