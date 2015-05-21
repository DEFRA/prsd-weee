PRINT 'Rename waste action column';
EXECUTE sp_rename N'[Notification].[Notification].WasteAction', N'NotificationType', 'COLUMN';
GO

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating [Business].[Facility]...';


GO
CREATE TABLE [Business].[Facility] (
    [Id]                           UNIQUEIDENTIFIER NOT NULL,
    [Name]                         NVARCHAR (1024)  NOT NULL,
    [IsActualSiteOfTreatment]      BIT              NOT NULL,
    [Type]                         NVARCHAR (64)    NOT NULL,
    [RegistrationNumber]           NVARCHAR (64)    NOT NULL,
    [AdditionalRegistrationNumber] NVARCHAR (64)    NULL,
    [Building]                     NVARCHAR (1024)  NOT NULL,
    [Address1]                     NVARCHAR (1024)  NOT NULL,
    [Address2]                     NVARCHAR (1024)  NULL,
    [TownOrCity]                   NVARCHAR (1024)  NOT NULL,
    [PostalCode]                   NVARCHAR (64)    NOT NULL,
	[Country]					   NVARCHAR(1024)	NOT NULL,
    [CountryId]                    UNIQUEIDENTIFIER NOT NULL,
    [FirstName]                    NVARCHAR (1024)  NOT NULL,
    [LastName]                     NVARCHAR (1024)  NOT NULL,
    [Telephone]                    NVARCHAR (150)   NOT NULL,
    [Fax]                          NVARCHAR (150)   NULL,
    [Email]                        NVARCHAR (256)   NOT NULL,
    [RowVersion]                   ROWVERSION       NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [Notification].[NotificationFacility]...';


GO
CREATE TABLE [Notification].[NotificationFacility] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [NotificationId] UNIQUEIDENTIFIER NOT NULL,
    [FacilityId]     UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_NotificationFacility] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating DF_NotificationFacilityId...';


GO
ALTER TABLE [Notification].[NotificationFacility]
    ADD CONSTRAINT [DF_NotificationFacilityId] DEFAULT (newid()) FOR [Id];


GO


GO
PRINT N'Creating FK_Facility_Country...';


GO
ALTER TABLE [Business].[Facility] WITH NOCHECK
    ADD CONSTRAINT [FK_Facility_Country] FOREIGN KEY ([CountryId]) REFERENCES [Lookup].[Country] ([Id]);


GO
PRINT N'Creating FK_NotificationFacility_Notification...';


GO
ALTER TABLE [Notification].[NotificationFacility] WITH NOCHECK
    ADD CONSTRAINT [FK_NotificationFacility_Notification] FOREIGN KEY ([NotificationId]) REFERENCES [Notification].[Notification] ([Id]);


GO
PRINT N'Creating FK_NotificationFacility_Facility...';


GO
ALTER TABLE [Notification].[NotificationFacility] WITH NOCHECK
    ADD CONSTRAINT [FK_NotificationFacility_Facility] FOREIGN KEY ([FacilityId]) REFERENCES [Business].[Facility] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO

ALTER TABLE [Business].[Facility] WITH CHECK CHECK CONSTRAINT [FK_Facility_Country];

ALTER TABLE [Notification].[NotificationFacility] WITH CHECK CHECK CONSTRAINT [FK_NotificationFacility_Notification];

ALTER TABLE [Notification].[NotificationFacility] WITH CHECK CHECK CONSTRAINT [FK_NotificationFacility_Facility];


GO
PRINT N'Update complete.';


GO
