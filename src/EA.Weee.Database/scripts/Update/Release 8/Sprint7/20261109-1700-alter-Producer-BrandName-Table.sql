-- Modified Date: 09/11/2026 17:00:00 Craig Macbeth - Alteration of a column in the Producer.BrandName table.
--													  Increase the size of the column to 1000 characters.
-- =============================================================================================================

IF  COL_LENGTH('Producer.BrandName','Name') < 2000
BEGIN
	ALTER TABLE [Producer].[BrandName]
	ALTER COLUMN [Name] NVARCHAR(1000) NOT NULL;
    PRINT N'Increased size of Name column on BrandName table';
END
