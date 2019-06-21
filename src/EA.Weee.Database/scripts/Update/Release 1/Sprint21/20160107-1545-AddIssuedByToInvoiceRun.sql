/*
 * This script adds column [IssuedByUserId] to the [Charging].[InvoiceRun] table
 * with a FK to the [Identity].[AspNetUsers] table.
 *
 * As we don't know who issued any existing invoice runs, we will choose an
 * arbitrary user if necessary.
 *
 * It also renames the [CreatedDate] column to [IssuedDate] as this is the domain
 * terminology.
 */

ALTER TABLE [Charging].[InvoiceRun]
	ADD [IssuedDate] DATETIME NULL
GO

UPDATE
	[Charging].[InvoiceRun]
SET
	[IssuedDate] = [CreatedDate]
GO

ALTER TABLE [Charging].[InvoiceRun]
	ALTER COLUMN [IssuedDate] DATETIME
GO

ALTER TABLE [Charging].[InvoiceRun]
	DROP COLUMN [CreatedDate]
GO

ALTER TABLE [Charging].[InvoiceRun]
	ADD [IssuedByUserId] NVARCHAR(128) NULL
GO

IF (EXISTS(SELECT * FROM [Charging].[InvoiceRun]))
BEGIN

	DECLARE @UserId NVARCHAR(128)
	SELECT
		@UserId = [Id]
	FROM
		[Identity].[AspNetUsers]

	UPDATE
		[Charging].[InvoiceRun]
	SET
		[IssuedByUserId] = @UserId

END

ALTER TABLE [Charging].[InvoiceRun]
	ALTER COLUMN [IssuedByUserId] NVARCHAR(128) NOT NULL
GO

ALTER TABLE [Charging].[InvoiceRun]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceRun_AspNetUsers_IssuedBy] FOREIGN KEY([IssuedByUserId])
REFERENCES [Identity].[AspNetUsers] ([Id])
GO

ALTER TABLE [Charging].[InvoiceRun] CHECK CONSTRAINT [FK_InvoiceRun_AspNetUsers_IssuedBy]
GO




