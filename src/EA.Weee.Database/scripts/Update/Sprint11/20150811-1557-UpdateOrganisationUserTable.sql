PRINT N'Altering [Organisation].[OrganisationUser]...';
GO

EXEC sp_rename '[Organisation].[OrganisationUser].OrganisationUserStatus', 'UserStatus', 'COLUMN';
GO

GO    
PRINT N'Update complete.';

GO