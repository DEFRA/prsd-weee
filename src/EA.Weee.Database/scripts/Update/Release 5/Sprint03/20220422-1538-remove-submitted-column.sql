IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'SubmittedDate'
          AND Object_ID = Object_ID(N'Evidence.Note'))
BEGIN
    ALTER TABLE Evidence.Note
        DROP COLUMN SubmittedDate;
END

IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'SubmittedById'
          AND Object_ID = Object_ID(N'Evidence.Note'))
BEGIN
    DROP INDEX IDX_Note_SubmittedById ON Evidence.Note;
    ALTER TABLE Evidence.Note DROP CONSTRAINT FK_Note_SubmittedBy_UserId;
    ALTER TABLE Evidence.Note
        DROP COLUMN SubmittedById;
END