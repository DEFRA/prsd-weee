--===================================================================================================
-- Modified Date: 19/11/2026 13:27:00 
-- Addition of one new columns to the Identity.AspNetUsers table.
-- The date and time of the user's most recent password change will be stored in PasswordChangedDate.
-- ==================================================================================================

IF COL_LENGTH('Identity.AspNetUsers','PasswordChangedDate') IS NULL
BEGIN
	ALTER TABLE [Identity].[AspNetUsers]
	ADD PasswordChangedDate DATETIME NOT NULL
	CONSTRAINT DF_AspNetUser_PasswordChangedDate
	DEFAULT (DATEADD(MONTH, -6, GETDATE()));
END
