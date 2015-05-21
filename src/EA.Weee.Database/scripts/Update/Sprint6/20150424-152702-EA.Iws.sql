

GO
PRINT N'Creating [Auditing]...';


GO
CREATE SCHEMA [Auditing]
    AUTHORIZATION [dbo];


GO
PRINT N'Creating [Business]...';


GO
CREATE SCHEMA [Business]
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
PRINT N'Creating [Business].[Address]...';


GO
CREATE TABLE [Business].[Address] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Building]       NVARCHAR (1024)  NOT NULL,
    [StreetOrSuburb] NVARCHAR (1024)  NULL,
    [TownOrCity]     NVARCHAR (1024)  NOT NULL,
    [Region]         NVARCHAR (1024)  NULL,
    [PostalCode]     NVARCHAR (64)    NOT NULL,
    [CountryId]      UNIQUEIDENTIFIER NOT NULL,
    [RowVersion]     ROWVERSION       NOT NULL,
    CONSTRAINT [PK_Address_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [Business].[Organisation]...';


GO
CREATE TABLE [Business].[Organisation] (
    [Id]         UNIQUEIDENTIFIER NOT NULL,
    [Name]       NVARCHAR (2048)  NOT NULL,
    [AddressId]  UNIQUEIDENTIFIER NOT NULL,
    [Type]       NVARCHAR (64)    NOT NULL,
    [RowVersion] ROWVERSION       NOT NULL,
    CONSTRAINT [PK_Organisation_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [Lookup].[CompetentAuthority]...';


GO
CREATE TABLE [Lookup].[CompetentAuthority] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [Name]         NVARCHAR (1023)  NOT NULL,
    [Abbreviation] NVARCHAR (63)    NULL,
    [IsSystemUser]   BIT              NOT NULL,
    [RowVersion]   ROWVERSION       NOT NULL,
    CONSTRAINT [PK_CompetentAuthority_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [Lookup].[Country]...';


GO
CREATE TABLE [Lookup].[Country] (
    [Id]                    UNIQUEIDENTIFIER NOT NULL,
    [Name]                  NVARCHAR (2048)  NOT NULL,
    [IsoAlpha2Code]         NCHAR (2)        NOT NULL,
    [IsEuropeanUnionMember] BIT              NOT NULL,
    [RowVersion]            ROWVERSION       NOT NULL,
    CONSTRAINT [PK_Country_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating DF_CompetentAuthority_IsSystemUser...';


GO
ALTER TABLE [Lookup].[CompetentAuthority]
    ADD CONSTRAINT [DF_CompetentAuthority_IsSystemUser] DEFAULT ((0)) FOR [IsSystemUser];


GO
PRINT N'Creating FK_Address_Country...';


GO
ALTER TABLE [Business].[Address] WITH NOCHECK
    ADD CONSTRAINT [FK_Address_Country] FOREIGN KEY ([CountryId]) REFERENCES [Lookup].[Country] ([Id]);


GO
PRINT N'Creating FK_Organisation_Country...';


GO
ALTER TABLE [Business].[Organisation] WITH NOCHECK
    ADD CONSTRAINT [FK_Organisation_Country] FOREIGN KEY ([AddressId]) REFERENCES [Business].[Address] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [Business].[Address] WITH CHECK CHECK CONSTRAINT [FK_Address_Country];

ALTER TABLE [Business].[Organisation] WITH CHECK CHECK CONSTRAINT [FK_Organisation_Country];


GO
PRINT N'Update complete.';


GO
