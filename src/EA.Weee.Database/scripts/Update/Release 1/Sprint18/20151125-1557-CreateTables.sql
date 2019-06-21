/*
 * This script creates two new table in the producer schema called 'RegisteredProducer' and 'ProducerSubmission'
 *
 * Each row in the registered producer table provides a link between:
 * - a producer (identified by their producer registration number, e.g. WEEE/AA1234AA) 
 * - a compliance year
 * - a producer compliance scheme
 *
 * All columns in this table are immutable except for the current submission Id.
 * The current submission ID column will always be null when registrations are created, i.e.
 * when an XML file has been uploaded and validated. It will only be populated for registrations
 * that are complete, i.e. the XML file has been submitted.
 *
 * Each row in the producer submission table provides a set of details for a registration.
 * These rows will be created when an XML file has been uploaded and validated.
 * All columns in this table are immutable.
 */

-- Create [Producer].[RegisteredProducer]
CREATE TABLE [Producer].[RegisteredProducer]
(
	[Id]							UNIQUEIDENTIFIER	NOT NULL,
	[RowVersion]					TIMESTAMP			NOT NULL,
	[ProducerRegistrationNumber]	NVARCHAR(50)		NOT NULL,
	[SchemeId]						UNIQUEIDENTIFIER	NOT NULL,
	[ComplianceYear]				INT					NOT NULL,
	[CurrentSubmissionId]			UNIQUEIDENTIFIER	NULL

	CONSTRAINT [PK_RegisteredProducer] PRIMARY KEY CLUSTERED ( [Id] ASC )
	WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]

) ON [PRIMARY]
GO

-- Create [Producer].[ProducerSubmission]
CREATE TABLE [Producer].[ProducerSubmission]
(
	[Id]							UNIQUEIDENTIFIER	NOT NULL,
	[RowVersion]					TIMESTAMP			NOT NULL,
	[UpdatedDate]					DATETIME			NOT NULL,
	[RegisteredProducerId]			UNIQUEIDENTIFIER	NOT NULL,
	[ObligationType]				NVARCHAR(4)			NOT NULL,
	[TradingName]					NVARCHAR(255)		NOT NULL,
	[VATRegistered]					BIT					NOT NULL,
	[EEEPlacedOnMarketBandType]		INT					NOT NULL,
	[SellingTechniqueType]			INT					NOT NULL,
	[MemberUploadId]				UNIQUEIDENTIFIER	NOT NULL,
	[AuthorisedRepresentativeId]	UNIQUEIDENTIFIER	NULL,
	[ProducerBusinessId]			UNIQUEIDENTIFIER	NOT NULL,
	[AnnualTurnover]				DECIMAL(28, 12)		NULL,
	[AnnualTurnoverBandType]		INT					NOT NULL,
	[ChargeBandAmountId]			UNIQUEIDENTIFIER	NOT NULL,
	[ChargeThisUpdate]				DECIMAL(18, 2)		NOT NULL,
	[CeaseToExist]					DATETIME			NULL

	CONSTRAINT [PK_ProducerSubmission] PRIMARY KEY CLUSTERED ( [Id] ASC )
	WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]

) ON [PRIMARY]
GO

-- This check constraint ensures that the obligation type can only have values of 'B2B', 'B2C' or 'Both'.
ALTER TABLE [Producer].[ProducerSubmission]  WITH CHECK ADD
	CONSTRAINT [CK_ProducerSubmission_ObligationType] CHECK ([ObligationType] IN ('B2B', 'B2C', 'Both'))
GO

ALTER TABLE [Producer].[ProducerSubmission]
	CHECK CONSTRAINT [CK_ProducerSubmission_ObligationType]
GO

-- Add Foreign Key constraints.
ALTER TABLE [Producer].[RegisteredProducer]  WITH CHECK ADD
	CONSTRAINT [FK_RegisteredProducer_Scheme] FOREIGN KEY([SchemeId])
	REFERENCES [PCS].[Scheme] ([Id])
GO

ALTER TABLE [Producer].[RegisteredProducer]
	CHECK CONSTRAINT [FK_RegisteredProducer_Scheme]
GO


ALTER TABLE [Producer].[RegisteredProducer]  WITH CHECK ADD
	CONSTRAINT [FK_RegisteredProducer_ProducerSubmission] FOREIGN KEY([CurrentSubmissionId])
	REFERENCES [Producer].[ProducerSubmission] ([Id])
GO

ALTER TABLE [Producer].[RegisteredProducer]
	CHECK CONSTRAINT [FK_RegisteredProducer_ProducerSubmission]
GO


ALTER TABLE [Producer].[ProducerSubmission]  WITH CHECK ADD
	CONSTRAINT [FK_ProducerSubmission_RegisteredProducer] FOREIGN KEY([RegisteredProducerId])
	REFERENCES [Producer].[RegisteredProducer] ([Id])
GO

ALTER TABLE [Producer].[ProducerSubmission]
	CHECK CONSTRAINT [FK_ProducerSubmission_RegisteredProducer]
GO


ALTER TABLE [Producer].[ProducerSubmission]  WITH CHECK ADD
	CONSTRAINT [FK_ProducerSubmission_MemberUpload] FOREIGN KEY([MemberUploadId])
	REFERENCES [PCS].[MemberUpload] ([Id])
GO

ALTER TABLE [Producer].[ProducerSubmission]
	CHECK CONSTRAINT [FK_ProducerSubmission_MemberUpload]
GO


ALTER TABLE [Producer].[ProducerSubmission]  WITH CHECK ADD
	CONSTRAINT [FK_ProducerSubmission_AuthorisedRepresentative] FOREIGN KEY([AuthorisedRepresentativeId])
	REFERENCES [Producer].[AuthorisedRepresentative] ([Id])
GO

ALTER TABLE [Producer].[ProducerSubmission]
	CHECK CONSTRAINT [FK_ProducerSubmission_AuthorisedRepresentative]
GO


ALTER TABLE [Producer].[ProducerSubmission]  WITH CHECK ADD
	CONSTRAINT [FK_ProducerSubmission_Business] FOREIGN KEY([ProducerBusinessId])
	REFERENCES [Producer].[Business] ([Id])
GO

ALTER TABLE [Producer].[ProducerSubmission]
	CHECK CONSTRAINT [FK_ProducerSubmission_Business]
GO


ALTER TABLE [Producer].[ProducerSubmission]  WITH CHECK ADD
	CONSTRAINT [FK_ProducerSubmission_ChargeBandAmount] FOREIGN KEY([ChargeBandAmountId])
	REFERENCES [Lookup].[ChargeBandAmount] ([Id])
GO

ALTER TABLE [Producer].[ProducerSubmission]
	CHECK CONSTRAINT [FK_ProducerSubmission_ChargeBandAmount]
GO

-- Add Indexes
CREATE NONCLUSTERED INDEX [IX_RegisteredProducer_PrnAndComplianceYear] ON [Producer].[RegisteredProducer] 
(
	[ProducerRegistrationNumber] ASC,
	[ComplianceYear] ASC
)
INCLUDE ( [Id], [SchemeId]) 
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


CREATE NONCLUSTERED INDEX [IX_ProducerSubmission_RegisteredProducerId] ON [Producer].[ProducerSubmission] 
(
	[RegisteredProducerId] ASC
)
INCLUDE ( [Id] ) 
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
