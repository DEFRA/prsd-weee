PRINT N'Altering [Admin].[CompetentAuthorityUser]...';
GO

EXEC sp_rename '[Admin].[CompetentAuthorityUser].CompetentAuthorityUserStatus', 'UserStatus', 'COLUMN';
GO

GO    
PRINT N'Update complete.';

GO