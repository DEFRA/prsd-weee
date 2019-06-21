/*
 * This script moves the columns for storing 1B1S file data into a separate table.
 * This allows the 1B1S file data to be represented by an entity that is involved
 * in an optional relationship with the invoice run.
 */
GO

DROP INDEX [UQ_InvoiceRun_FileId] ON [Charging].[InvoiceRun] WITH ( ONLINE = OFF )
GO

ALTER TABLE [Charging].[InvoiceRun]
	DROP COLUMN [IbisFileId]

ALTER TABLE [Charging].[InvoiceRun]
	DROP COLUMN [IbisCustomerFileName]

ALTER TABLE [Charging].[InvoiceRun]
	DROP COLUMN [IbisCustomerFileData]

ALTER TABLE [Charging].[InvoiceRun]
	DROP COLUMN [IbisTransactionFileName]

ALTER TABLE [Charging].[InvoiceRun]
	DROP COLUMN [IbisTransactionFileData]
GO

CREATE TABLE [Charging].[IbisFileData](
	[Id] [uniqueidentifier] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[FileId] [bigint] NOT NULL,
	[CustomerFileName] [nvarchar](100) NOT NULL,
	[CustomerFileData] [nvarchar](max) NOT NULL,
	[TransactionFileName] [nvarchar](100) NOT NULL,
	[TransactionFileData] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_IbisFileData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [UQ_IbisFileData_FileId] ON [Charging].[IbisFileData] 
(
	[FileId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

ALTER TABLE [Charging].[InvoiceRun]
	ADD [IbisFileDataId] [uniqueidentifier] NULL
GO

ALTER TABLE [Charging].[InvoiceRun]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceRun_IbisFileData] FOREIGN KEY([IbisFileDataId])
REFERENCES [Charging].[IbisFileData] ([Id])
GO

ALTER TABLE [Charging].[InvoiceRun] CHECK CONSTRAINT [FK_InvoiceRun_IbisFileData]
GO