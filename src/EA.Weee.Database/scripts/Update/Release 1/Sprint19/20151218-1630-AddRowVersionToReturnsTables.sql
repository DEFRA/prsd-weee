ALTER TABLE [PCS].[AatfDeliveryLocation]
ADD [RowVersion] TIMESTAMP NOT NULL

ALTER TABLE [PCS].[AeDeliveryLocation]
ADD [RowVersion] TIMESTAMP NOT NULL

ALTER TABLE [PCS].[EeeOutputAmount]
ADD [RowVersion] TIMESTAMP NOT NULL

ALTER TABLE [PCS].[EeeOutputReturnVersion]
ADD [RowVersion] TIMESTAMP NOT NULL

ALTER TABLE [PCS].[EeeOutputReturnVersionAmount]
ADD [RowVersion] TIMESTAMP NOT NULL

ALTER TABLE [PCS].[WeeeCollectedAmount]
ADD [RowVersion] TIMESTAMP NOT NULL

ALTER TABLE [PCS].[WeeeCollectedReturnVersion]
ADD [RowVersion] TIMESTAMP NOT NULL

ALTER TABLE [PCS].[WeeeCollectedReturnVersionAmount]
ADD [RowVersion] TIMESTAMP NOT NULL

ALTER TABLE [PCS].[WeeeDeliveredAmount]
ADD [RowVersion] TIMESTAMP NOT NULL

ALTER TABLE [PCS].[WeeeDeliveredReturnVersion]
ADD [RowVersion] TIMESTAMP NOT NULL

ALTER TABLE [PCS].[WeeeDeliveredReturnVersionAmount]
ADD [RowVersion] TIMESTAMP NOT NULL