GO
PRINT N'Altering [PCS].[MemberUpload]...';


GO
ALTER TABLE [PCS].[MemberUpload] ADD [UserId] NVARCHAR (128) NULL;

ALTER TABLE [PCS].[MemberUpload] ADD [Date] DATETIME NULL;

GO

  PRINT N'Update complete.';
GO