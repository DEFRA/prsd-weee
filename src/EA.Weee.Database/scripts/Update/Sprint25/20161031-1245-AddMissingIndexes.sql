CREATE NONCLUSTERED INDEX [IX_Scheme_OrganisationId] ON [PCS].[Scheme]
(
	[OrganisationId] ASC
)
GO


CREATE NONCLUSTERED INDEX [IX_DataReturn_CurrentDataReturnVersionId] ON [PCS].[DataReturn]
(
	[CurrentDataReturnVersionId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_DataReturnVersion_EeeOutputReturnVersionId] ON [PCS].[DataReturnVersion]
(
	[EeeOutputReturnVersionId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_EeeOutputAmount_RegisteredProducerId] ON [PCS].[EeeOutputAmount]
(
	[RegisteredProducerId] ASC
)
GO