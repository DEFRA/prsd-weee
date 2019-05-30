CREATE TABLE [AATF].[Address](
	[Id]				uniqueidentifier NOT NULL,
	[Name]				nvarchar(256) NOT NULL,
	[Address1]			nvarchar(35) NOT NULL,
	[Address2]			nvarchar(35) NULL,
	[TownOrCity]		nvarchar(35) NOT NULL,
	[CountyOrRegion]	nvarchar(35) NULL,
	[Postcode]			nvarchar(10) NULL,
	[CountryId]			uniqueidentifier NOT NULL,
	[RowVersion]		timestamp NOT NULL,

	CONSTRAINT [PK_Address_Id] PRIMARY KEY CLUSTERED  ([Id] ASC)
);

ALTER TABLE [AATF].[WeeeReusedSite] ADD AddressId uniqueidentifier NOT NULL;
ALTER TABLE [AATF].[WeeeReusedSite] ADD CONSTRAINT FK_WeeeReused_Address_AddressId FOREIGN KEY (AddressId) REFERENCES [AATF].[Address](Id);