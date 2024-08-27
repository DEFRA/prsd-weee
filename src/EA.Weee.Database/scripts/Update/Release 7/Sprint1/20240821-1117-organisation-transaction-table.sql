-- Check if the table exists and create it if it doesn't
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Organisation].[OrganisationTransaction]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Organisation].[OrganisationTransaction] (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        UserId NVARCHAR(128) NOT NULL,
        OrganisationJson NVARCHAR(MAX) NOT NULL,
        CompletionStatus INT NOT NULL,
        CreatedDateTime [datetime] NULL,
        CompletedDateTime [datetime] NULL,
        [RowVersion] [timestamp] NOT NULL
    );
END

-- Create indexes if they don't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrganisationTransaction_UserId' AND object_id = OBJECT_ID('[Organisation].[OrganisationTransaction]'))
BEGIN
    CREATE INDEX IX_OrganisationTransaction_UserId ON [Organisation].[OrganisationTransaction](UserId);
END

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Organisation].[FK_OrganisationTransaction_AspNetUser_UserId]') AND parent_object_id = OBJECT_ID(N'[Organisation].[OrganisationTransaction]'))
BEGIN
    ALTER TABLE [Organisation].[OrganisationTransaction] WITH CHECK ADD CONSTRAINT [FK_OrganisationTransaction_AspNetUser_UserId] FOREIGN KEY(UserId)
    REFERENCES [Identity].[AspNetUsers] ([Id]);
END
