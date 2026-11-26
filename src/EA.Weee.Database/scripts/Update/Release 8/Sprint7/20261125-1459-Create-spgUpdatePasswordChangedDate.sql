CREATE PROCEDURE [Identity].[UpdatePasswordChangedDate]
    @UserId NVARCHAR(128),
    @PasswordChangedDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [Identity].[AspNetUsers]
    SET PasswordChangedDate = @PasswordChangedDate
    WHERE Id = @UserId;
END