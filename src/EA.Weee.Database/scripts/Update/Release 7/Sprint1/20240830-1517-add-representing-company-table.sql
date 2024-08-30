CREATE TABLE [Organisation].[RepresentingCompany](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[TradingName] [nvarchar](256) NULL,
	[Address1] [nvarchar](60) NOT NULL,
	[Address2] [nvarchar](60) NULL,
	[TownOrCity] [nvarchar](35) NOT NULL,
	[CountyOrRegion] [nvarchar](35) NULL,
	[Postcode] [nvarchar](10) NULL,
	[CountryId] [uniqueidentifier] NOT NULL,
	[Telephone] [nvarchar](20) NULL,
	[Email] [nvarchar](256) NULL,
	[RowVersion] [timestamp] NOT NULL
 CONSTRAINT [PK_RepresentingCompany_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [Organisation].[RepresentingCompany]  WITH CHECK ADD  CONSTRAINT [FK_RepresentingCompany_Country] FOREIGN KEY([CountryId])
REFERENCES [Lookup].[Country] ([Id])
GO

ALTER TABLE [Organisation].[RepresentingCompany] CHECK CONSTRAINT [FK_RepresentingCompany_Country]
GO

ALTER TABLE [Producer].[DirectRegistrant] DROP CONSTRAINT [FK_DirectRegistrant_RepresentingCompany]
ALTER TABLE [Producer].[DirectRegistrant] WITH CHECK ADD CONSTRAINT [FK_DirectRegistrant_RepresentingCompany] 
    FOREIGN KEY([RepresentingCompanyId]) REFERENCES [Organisation].[RepresentingCompany] ([Id]);

DROP INDEX [IX_DirectRegistrant_RepresentingCompanyId] ON [Producer].[DirectRegistrant];
CREATE NONCLUSTERED INDEX [IX_DirectRegistrant_RepresentingCompanyId] ON [Producer].[DirectRegistrant] ([RepresentingCompanyId]);