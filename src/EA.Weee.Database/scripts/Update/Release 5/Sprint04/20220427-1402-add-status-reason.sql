IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Reason'
          AND Object_ID = Object_ID(N'[Evidence].[NoteStatusHistory]'))
BEGIN
    ALTER TABLE [Evidence].[NoteStatusHistory]
        ADD Reason NVARCHAR(2000) NULL
END