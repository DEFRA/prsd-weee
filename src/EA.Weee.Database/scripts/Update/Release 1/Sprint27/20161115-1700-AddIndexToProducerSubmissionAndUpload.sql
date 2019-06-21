CREATE NONCLUSTERED INDEX [IX_ProducerSubmission_MemberUploadId] ON [Producer].[ProducerSubmission]
(
	[MemberUploadId] ASC
)

GO

CREATE NONCLUSTERED INDEX [IX_MemberUpload_SubmittedDate] ON [PCS].[MemberUpload]
(
	[SubmittedDate] ASC
)

GO