

GO
PRINT N'Altering [PCS].[MemberUpload]...';


GO
ALTER TABLE [PCS].[MemberUpload]
    ADD [ComplianceYear] INT              NOT NULL,
        [IsSubmitted]    BIT              NOT NULL,
        [SchemeId]       UNIQUEIDENTIFIER NULL;


GO
PRINT N'Creating FK_MemberUpload_Scheme...';


GO
ALTER TABLE [PCS].[MemberUpload] WITH NOCHECK
    ADD CONSTRAINT [FK_MemberUpload_Scheme] FOREIGN KEY ([SchemeId]) REFERENCES [PCS].[Scheme] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [PCS].[MemberUpload] WITH CHECK CHECK CONSTRAINT [FK_MemberUpload_Scheme];


GO
PRINT N'Update complete.';


GO
