/*
 * This script adds a column to the [dbo].[SystemData] table to store
 * the first file ID that will be used when generating 1B1S files.
 */

ALTER TABLE [dbo].[SystemData]
	ADD [InitialIbisFileId] BIGINT NULL

GO
UPDATE [dbo].[SystemData]
SET [InitialIbisFileId] = 0

GO
ALTER TABLE [dbo].[SystemData]
	ALTER COLUMN [InitialIbisFileId] BIGINT NOT NULL