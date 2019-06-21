/*
 * This script adds nullable [SubmittedDate] and [SubmittedByUserId] columns to the [PCS].[MemberUpload] table.
 * A foreign key will be added between the [PCS].[MemberUpload] table and the [Identity].[AspNetUser] table.
 * The columns will be populated with the values of the [CreatedDate] and [CreatedByUserId] columns for
 * any member uploads which have been submitted (IsSubmitted = 1).
 */

ALTER TABLE [PCS].[MemberUpload]
	ADD [SubmittedDate] DATETIME NULL

ALTER TABLE [PCS].[MemberUpload]
	ADD [SubmittedByUserId] NVARCHAR(128) NULL

GO

ALTER TABLE [PCS].[MemberUpload] WITH CHECK ADD CONSTRAINT [FK_MemberUpload_User_SubmittedBy] FOREIGN KEY([SubmittedByUserId])
REFERENCES [Identity].[AspNetUsers] ([Id])

GO

UPDATE
	[PCS].[MemberUpload]
SET
	[SubmittedDate] = [CreatedDate],
	[SubmittedByUserId] = [CreatedById]
WHERE
	[IsSubmitted] = 1

GO