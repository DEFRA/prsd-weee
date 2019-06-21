
-- Add the [ProcessTime] column.
ALTER TABLE [PCS].[DataReturnsUpload]
ADD [ProcessTime] TIME NULL
GO

-- Set process time of all existing entries to 0
DECLARE @time TIME = '00:00:00.0000000'
UPDATE [PCS].[DataReturnsUpload]
SET [ProcessTime] = @time

-- Make [ProcessTime] column non-nullable.
ALTER TABLE [PCS].[DataReturnsUpload]
ALTER COLUMN [ProcessTime] TIME NOT NULL
GO
