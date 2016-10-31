CREATE NONCLUSTERED INDEX [IX_DataReturn_DataReturnVersion] ON [PCS].[DataReturn]
(
	[CurrentDataReturnVersionId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_DataReturnVersion_EeeOutputReturnVersion] ON [PCS].[DataReturnVersion]
(
	[EeeOutputReturnVersionId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_EeeOutputAmount_RegisteredProducer] ON [PCS].[EeeOutputAmount]
(
	[RegisteredProducerId] ASC
)
GO