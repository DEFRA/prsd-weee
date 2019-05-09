
GO

ALTER TABLE [AATF].AATF ADD SiteAddressId uniqueidentifier  NULL;
ALTER TABLE [AATF].AATF ADD CONSTRAINT FK_AATFAddress_SiteAddressId FOREIGN KEY (SiteAddressId) REFERENCES [AATF].Address(Id);

ALTER TABLE [AATF].AATF ADD Size int NULL;

ALTER TABLE [AATF].AATF ADD ApprovalDate [datetime2](7)  NULL;
GO

UPDATE [AATF].AATF SET Size = 2;

DECLARE @defaultSiteId uniqueidentifier 
SELECT @defaultSiteId = Id FROM [AATF].Address

UPDATE [AATF].AATF SET SiteAddressId = @defaultSiteId


ALTER TABLE [AATF].AATF ALTER COLUMN SiteAddressId uniqueidentifier NOT NULL;
GO

ALTER TABLE [AATF].AATF ALTER COLUMN Size int NOT NULL;
GO

