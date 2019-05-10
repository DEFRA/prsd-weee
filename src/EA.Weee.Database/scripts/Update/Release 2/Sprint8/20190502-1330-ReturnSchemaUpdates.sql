BEGIN TRANSACTION

ALTER TABLE [AATF].[Return] ADD CreatedBy NVARCHAR(128) NULL;
GO
ALTER TABLE [AATF].[Return] ADD SubmittedBy NVARCHAR(128) NULL;
GO
ALTER TABLE [AATF].[Return] ADD CreatedDate DATETIME NULL;
GO
ALTER TABLE [AATF].[Return] ADD SubmittedDate DATETIME NULL;
GO
ALTER TABLE [AATF].[Return] ADD ParentId UNIQUEIDENTIFIER NULL;
GO

IF EXISTS (SELECT * FROM [AATF].[Return])
BEGIN 
	DECLARE @Id NVARCHAR(128)
	SELECT TOP 1 @Id = Id FROM [Identity].AspNetUsers
	UPDATE [AATF].[Return] SET CreatedBy = @Id, SubmittedBy = @Id, CreatedDate = GETDATE(), SubmittedDate = GETDATE()
END
GO
ALTER TABLE [AATF].[Return] ALTER COLUMN CreatedBy NVARCHAR(128) NOT NULL;
GO
ALTER TABLE [AATF].[Return] ALTER COLUMN CreatedDate DATETIME NOT NULL;
GO
ALTER TABLE [AATF].[Return] ADD CONSTRAINT FK_Return_CreatedBy_UserId FOREIGN KEY (CreatedBy) REFERENCES [Identity].AspNetUsers(Id);
GO
ALTER TABLE [AATF].[Return] ADD CONSTRAINT FK_Return_Submitted_UserId FOREIGN KEY (SubmittedBy) REFERENCES [Identity].AspNetUsers(Id);
GO
ALTER TABLE [AATF].[Return] ADD CONSTRAINT FK_Return_ParentId FOREIGN KEY (ParentId) REFERENCES [AATF].[Return](Id);
GO

EXEC sp_rename 'AATF.Return.CreatedBy', 'CreatedById';  
EXEC sp_rename 'AATF.Return.SubmittedBy', 'SubmittedById';  

COMMIT TRANSACTION