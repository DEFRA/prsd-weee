/*
 * This script adds a column to support the domain concept of a "CreateDate" of the invoice run domain object.
 */

ALTER TABLE [Charging].[InvoiceRun]
	ADD [CreatedDate] DATETIME NOT NULL