/*
 * This script adds columns to the [Charging].[InvoiceRun] table to store information
 * about the 1B1S files that are generated when the run is created.
 */

ALTER TABLE [Charging].[InvoiceRun]
	ADD [IbisCustomerFileName] NVARCHAR(100) NOT NULL

ALTER TABLE [Charging].[InvoiceRun]
	ADD [IbisCustomerFileData] NVARCHAR(MAX) NOT NULL

ALTER TABLE [Charging].[InvoiceRun]
	ADD [IbisTransactionFileName] NVARCHAR(100) NOT NULL

ALTER TABLE [Charging].[InvoiceRun]
	ADD [IbisTransactionFileData] NVARCHAR(MAX) NOT NULL