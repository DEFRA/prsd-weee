GO

/****** Object:  Index [IX_Producer_RegistrationNumber]    Script Date: 10/06/2015 19:40:04 ******/
CREATE NONCLUSTERED INDEX [IX_Producer_RegistrationNumber] ON [Producer].[Producer] 
(
	[RegistrationNumber] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


