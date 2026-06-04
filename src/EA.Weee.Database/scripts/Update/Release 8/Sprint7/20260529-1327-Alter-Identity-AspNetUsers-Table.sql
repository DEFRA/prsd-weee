-- Modified Date: 29/05/2026 13:27:00 Craig Macbeth - Addition of two new columns to the Identity.AspNetUsers table.
--													  LastLoginDate will store the date and time the user last logged in.
--													  UserCreated will store the date and time the User was created.
--													  UserCreated will also have a default constraint to automatically
--													  populate it on creation of a new record.
-- =============================================================================================================

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