EXEC sp_rename 'PCS.AatfDeliveryLocation.Name', 'FacilityName', 'COLUMN';
GO

EXEC sp_rename 'PCS.AeDeliveryLocation.Name', 'OperatorName', 'COLUMN';
GO