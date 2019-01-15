/*
 * This script adds a column to the [Charging].[InvoiceRun] table to store the file ID
 * of the generated 1B1S files. This allows us to enforce uniqueness at a database
 * level and supports maintenance of the system if we need to identify an invoice run
 * from the 1B1S file ID.
 */

ALTER TABLE [Charging].[InvoiceRun]
	ADD [IbisFileId] BIGINT NOT NULL

GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_InvoiceRun_FileId] ON [Charging].[InvoiceRun] 
(
	[IbisFileId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
