IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '[Identity].[PasswordHistories]')
BEGIN
    CREATE TABLE [Identity].[PasswordHistories]
    (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        UserId NVARCHAR(128) NOT NULL,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT FK_UserPasswordHistories_Users
            FOREIGN KEY (UserId)
            REFERENCES [Identity].[AspNetUsers](Id)
    );

    CREATE INDEX IX_PasswordHistories_UserId_CreatedDate
    ON [Identity].[PasswordHistories] (UserId, CreatedDate DESC);
END