GO
PRINT N'Altering [PCS].[MemberUploadError]...';


GO
ALTER TABLE [PCS].[MemberUploadError]
    ADD [LineNumber] INT CONSTRAINT [DF_MemberUploadError_LineNumber] DEFAULT ((0)) NOT NULL;


GO
PRINT N'Update complete.';


GO
