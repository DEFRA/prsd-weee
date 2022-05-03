IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Reason'
          AND Object_ID = Object_ID(N'[Evidence].[NoteStatusHistory]'))
BEGIN
    ALTER TABLE [Evidence].[NoteStatusHistory]
        ALTER COLUMN [Reason] NVARCHAR(200)
END