IF NOT EXISTS(SELECT TOP 1 * FROM [Identity].[AspNetUsers] WHERE Email=@Email) 
BEGIN

	BEGIN TRANSACTION

	DECLARE @UserId uniqueidentifier = NEWID()

	INSERT INTO [Identity].[AspNetUsers] (
	Id
	, Email
	, EmailConfirmed
	, PhoneNumberConfirmed
	, TwoFactorEnabled
	, AccessFailedCount
	, PasswordHash
	, SecurityStamp
	, LockoutEnabled
	, UserName
	, FirstName
	, Surname
	)
	VALUES(
	@UserId 
	, @Email
	, 1
	, 0
	, 0
	, 0
	, @HashedPassword
	, @SecurityStamp
	, 1
	, @Email
	, @FirstName
	, @Surname
	)

	IF @@ERROR != 0
	  BEGIN
		PRINT 'Errors Found ... Rolling back'
		ROLLBACK TRANSACTION
	  END
	ELSE
	  BEGIN
		PRINT 'No Errors ... Committing changes'
		COMMIT TRANSACTION
	  END
END