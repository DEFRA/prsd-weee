INSERT INTO [Lookup].[CompetentAuthority] ([Id], [Name], [Abbreviation], [IsSystemUser],[Region])
VALUES (newid(),'Environment Agency', 'EA',  1,'England'),
(newid(), 'Scottish Environment Protection Agency', 'SEPA', 0, 'Scotland'),
(newid(), 'Northern Ireland Environment Agency', 'NIEA', 0, 'Northern Ireland'),
(newid(), 'Natural Resources Wales', 'NRW', 0, 'Wales');
GO

