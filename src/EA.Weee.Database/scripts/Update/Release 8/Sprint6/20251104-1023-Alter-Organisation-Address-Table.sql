
IF COL_LENGTH('[Organisation].[Address]','Fax') IS NULL
BEGIN
    ALTER TABLE [Organisation].[Address] ADD [Fax] NVARCHAR(35) NULL;
    PRINT N'Added Fax column';
END
