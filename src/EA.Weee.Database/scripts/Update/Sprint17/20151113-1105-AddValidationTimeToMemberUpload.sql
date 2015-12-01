/*
	This script adds a [ValidationTime] column to the [PCS].[MemberUpload] table and
	sets the validation time of all existing records to 0 seconds
*/

-- Add the [ValidationTime] column.
ALTER TABLE [PCS].[MemberUpload]
ADD [ValidationTime] TIME NULL
GO

-- Set validation time of all existing entries to 0
DECLARE @time TIME = '00:00:00.0000000'
UPDATE [PCS].[MemberUpload]
SET [ValidationTime] = @time

-- Make [ValidationTime] column non-nullable.
ALTER TABLE [PCS].[MemberUpload]
ALTER COLUMN [ValidationTime] TIME NOT NULL
GO
