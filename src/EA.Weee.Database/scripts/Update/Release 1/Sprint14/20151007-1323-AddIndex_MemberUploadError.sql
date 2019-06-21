GO

/****** Object:  Index [IX_MemberUploadError_MemberUploadId]    Script Date: 07/10/2015 13:22:39 ******/
CREATE NONCLUSTERED INDEX [IX_MemberUploadError_MemberUploadId] ON [PCS].[MemberUploadError]
(
	[MemberUploadId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
