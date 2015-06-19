INSERT INTO [Lookup].[CompetentAuthority] ([Id], [Name], [Abbreviation],[CountryId])
VALUES (newid(),'Environment Agency', 'EA',(SELECT Id From [Lookup].[Country] WHERE Name = 'UK - England')),
(newid(), 'Scottish Environment Protection Agency', 'SEPA',(SELECT Id From [Lookup].[Country] WHERE Name = 'UK - Scotland')),
(newid(), 'Northern Ireland Environment Agency', 'NIEA', (SELECT Id From [Lookup].[Country] WHERE Name = 'UK - Northern Ireland')),
(newid(), 'Natural Resources Wales', 'NRW', (SELECT Id From [Lookup].[Country] WHERE Name = 'UK - Wales'));
GO

