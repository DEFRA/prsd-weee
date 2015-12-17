IF NOT EXISTS(SELECT * FROM sys.database_principals WHERE name=@Username) 
BEGIN

	BEGIN TRANSACTION

	EXEC('CREATE USER ' + @Username + ' FOR LOGIN ' + @Login)

	EXEC('ALTER ROLE ' + @Role + ' ADD MEMBER ' + @Username)
	
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