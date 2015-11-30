IF NOT EXISTS (SELECT * FROM dbo.sysusers WHERE NAME = 'weee_application') CREATE ROLE weee_application AUTHORIZATION db_securityadmin
GO