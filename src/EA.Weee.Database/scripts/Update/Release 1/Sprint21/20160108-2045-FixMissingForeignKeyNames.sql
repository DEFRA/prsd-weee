/*
 * The three foreign keys between the [PCS].[MemberUpload] table and the [Identity].[AspNetUser]
 * table are incorrectly named.
 * Two of them do not haev a name specified, which causes them to be given a random name
 * which is different in each environment. Furthermore, this causes unnecessary noise when
 * the EDMX used by the data access tests is refreshed.
 *
 * This script finds and drops all three FKs and gives them deterministic consistent names.
 */

DECLARE @MemberUploadCreatedByUserConstraintName NVARCHAR(1000)

SELECT
	@MemberUploadCreatedByUserConstraintName = CONSTRAINT_NAME
FROM
	INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE
	TABLE_SCHEMA = 'PCS'
	AND TABLE_NAME = 'MemberUpload'
	AND COLUMN_NAME = 'CreatedById'

EXEC('
ALTER TABLE [PCS].[MemberUpload]
	DROP CONSTRAINT [' + @MemberUploadCreatedByUserConstraintName + ']')

GO

ALTER TABLE [PCS].[MemberUpload]  WITH CHECK ADD CONSTRAINT [FK_MemberUpload_AspNetUser_CreatedBy] FOREIGN KEY([CreatedById])
	REFERENCES [Identity].[AspNetUsers] ([Id])
GO

DECLARE @MemberUploadUpdatedByUserConstraintName NVARCHAR(1000)

SELECT
	@MemberUploadUpdatedByUserConstraintName = CONSTRAINT_NAME
FROM
	INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE
	TABLE_SCHEMA = 'PCS'
	AND TABLE_NAME = 'MemberUpload'
	AND COLUMN_NAME = 'UpdatedById'

EXEC('
ALTER TABLE [PCS].[MemberUpload]
	DROP CONSTRAINT [' + @MemberUploadUpdatedByUserConstraintName + ']')

GO

ALTER TABLE [PCS].[MemberUpload]  WITH CHECK ADD CONSTRAINT [FK_MemberUpload_AspNetUser_UpdatedBy] FOREIGN KEY([UpdatedById])
	REFERENCES [Identity].[AspNetUsers] ([Id])
GO

ALTER TABLE [PCS].[MemberUpload]
	DROP CONSTRAINT [FK_MemberUpload_User_SubmittedBy]

GO

ALTER TABLE [PCS].[MemberUpload]  WITH CHECK ADD CONSTRAINT [FK_MemberUpload_AspNetUser_SubmittedBy] FOREIGN KEY([SubmittedByUserId])
	REFERENCES [Identity].[AspNetUsers] ([Id])
GO
