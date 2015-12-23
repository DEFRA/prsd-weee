/*
 * This script creates the [Charging] schema and the [InvoiceRun] table.
 * It adds a nullable column [InvoiceRunId] to the [PCS].[MemberUpload] table with a FK to the [InvoiceRun] table.
 * It adds an index to the [PCS].[MemberUpload] table on the [InvoiceRunId] column.
 */

GO
CREATE SCHEMA [Charging]
GO

CREATE TABLE [Charging].[InvoiceRun]
(
	[Id]							UNIQUEIDENTIFIER	NOT NULL,
	[RowVersion]					TIMESTAMP			NOT NULL,
	[CompetentAuthorityId]			UNIQUEIDENTIFIER	NOT NULL,

	CONSTRAINT [PK_InvoiceRun] PRIMARY KEY CLUSTERED ( [Id] ASC )
	WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]

) ON [PRIMARY]
GO

ALTER TABLE [Charging].[InvoiceRun]  WITH CHECK ADD
	CONSTRAINT [FK_InvoiceRun_CompetentAuthority] FOREIGN KEY([CompetentAuthorityId])
	REFERENCES [Lookup].[CompetentAuthority] ([Id])
GO

ALTER TABLE [PCS].[MemberUpload]
	ADD [InvoiceRunId] UNIQUEIDENTIFIER NULL
GO

ALTER TABLE [PCS].[MemberUpload]  WITH CHECK ADD
	CONSTRAINT [FK_MemberUpload_InvoiceRun] FOREIGN KEY([InvoiceRunId])
	REFERENCES [Charging].[InvoiceRun] ([Id])
GO

CREATE NONCLUSTERED INDEX [IX_MemberUpload_InvoiceRunId] ON [PCS].[MemberUpload] 
(
	[InvoiceRunId] ASC
)
INCLUDE ( [Id], [SchemeId], [ComplianceYear], [TotalCharges]) 
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
