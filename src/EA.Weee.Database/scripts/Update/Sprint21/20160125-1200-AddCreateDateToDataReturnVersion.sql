-- Add new CreatedDate column
ALTER TABLE [PCS].[DataReturnVersion]
ADD [CreatedDate] DATETIME NULL
GO

-- Populate the column with the value from the upload table
UPDATE DRV
SET DRV.[CreatedDate] = DRU.[Date]
FROM [PCS].[DataReturnVersion] DRV
JOIN [PCS].[DataReturnUpload] DRU ON DRU.[DataReturnVersionId] = DRV.[Id]
GO

-- Set CreatedDate as not nullable
ALTER TABLE [PCS].[DataReturnVersion]
ALTER COLUMN [CreatedDate] DATETIME NOT NULL
GO

GO

-- Create index on the CreateDate column
CREATE NONCLUSTERED INDEX [IX_DataReturnVersion_CreatedDate] ON [PCS].[DataReturnVersion]
(
	[CreatedDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
