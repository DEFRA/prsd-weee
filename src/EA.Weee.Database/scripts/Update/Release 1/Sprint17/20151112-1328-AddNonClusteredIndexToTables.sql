/****** Object:  Index [IX_MemberUpload_IsSubmitted]    Script Date: 12/11/2015 13:25:55 ******/
CREATE NONCLUSTERED INDEX [IX_MemberUpload_IsSubmitted] ON [PCS].[MemberUpload]
(
	[IsSubmitted] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


CREATE NONCLUSTERED INDEX [IX_SICCode_ProducerId] ON [Producer].[SICCode]
(
	[ProducerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

/****** Object:  Index [IX_Partner_PartnershipId]    Script Date: 12/11/2015 13:26:32 ******/
CREATE NONCLUSTERED INDEX [IX_Partner_PartnershipId] ON [Producer].[Partner]
(
	[PartnershipId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

/****** Object:  Index [IX_Producer_MemberUploadId]    Script Date: 12/11/2015 13:27:01 ******/
CREATE NONCLUSTERED INDEX [IX_Producer_MemberUploadId] ON [Producer].[Producer]
(
	[MemberUploadId] ASC
)
INCLUDE ( 	[RegistrationNumber],
	[UpdatedDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
