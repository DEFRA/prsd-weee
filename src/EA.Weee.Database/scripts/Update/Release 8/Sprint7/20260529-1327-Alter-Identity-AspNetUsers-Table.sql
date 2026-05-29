IF COL_LENGTH('Identity.AspNetUsers','UserCreated') IS NULL
BEGIN
	ALTER TABLE [Identity].[AspNetUsers] ADD UserCreated DATETIME2(7) NULL;
    PRINT N'Added UserCreated column';
END


IF OBJECT_ID('Identity.DF_AspNetUsers_UserCreated', 'D') IS NULL
BEGIN
	ALTER TABLE [Identity].[AspNetUsers]
	ADD CONSTRAINT [DF_AspNetUsers_UserCreated] 
	DEFAULT SYSDATETIME() FOR UserCreated;
	PRINT N'Added DF_AspNetUsers_UserCreated constraint';
END


IF COL_LENGTH('Identity.AspNetUsers','LastLoginDate') IS NULL
BEGIN
	ALTER TABLE [Identity].[LastLoginDate] ADD LastLoginDate DATETIME2(7) NULL
    PRINT N'Added LastLoginDate column';
END