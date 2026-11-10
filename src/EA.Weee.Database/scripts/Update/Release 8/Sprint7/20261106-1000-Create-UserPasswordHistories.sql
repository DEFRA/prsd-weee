IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '[Identity].[UserPasswordHistories]')
BEGIN
    CREATE TABLE [Identity].[UserPasswordHistories]
    (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        UserId NVARCHAR(128) NOT NULL,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT FK_UserPasswordHistories_Users
            FOREIGN KEY (UserId)
            REFERENCES [Identity].[AspNetUsers](Id)
    );

    CREATE INDEX IX_UserPasswordHistories_UserId_CreatedDate
    ON [Identity].[UserPasswordHistories] (UserId, CreatedDate DESC);
END