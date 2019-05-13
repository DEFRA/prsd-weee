CREATE TABLE [AATF].[Contact](
	[Id] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](35) NOT NULL,
	[LastName] [nvarchar](35) NOT NULL,
	[Position] [nvarchar](35) NOT NULL,
	[Address1] [nvarchar](35) NOT NULL,
	[Address2] [nvarchar](35) NULL,
	[TownOrCity] [nvarchar](35) NOT NULL,
	[CountyOrRegion] [nvarchar](35) NULL,
	[Postcode] [nvarchar](10) NULL,
	[CountryId] [uniqueidentifier] NOT NULL,
	[Telephone] [nvarchar](20) NOT NULL,
	[Email] [nvarchar](256) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Contact_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);