GO
PRINT N'Creating [PCS].[Scheme]...';

CREATE TABLE [PCS].[Scheme](
	[Id] [uniqueidentifier] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[ApprovalNumber] [nvarchar](50) NULL,
	[OrganisationId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Scheme] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [PCS].[Scheme]  WITH CHECK ADD  CONSTRAINT [FK_Scheme_Organisation] FOREIGN KEY([OrganisationId])
REFERENCES [Organisation].[Organisation] ([Id])
GO

ALTER TABLE [PCS].[Scheme] CHECK CONSTRAINT [FK_Scheme_Organisation]
GO


GO
PRINT N'Creating [Producer]...';


GO
CREATE SCHEMA [Producer]
    AUTHORIZATION [dbo];


GO
PRINT N'Creating [Producer].[Address]...';
GO

CREATE TABLE [Producer].[Address](
	[Id] [uniqueidentifier] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[PrimaryName] [nvarchar](100) NOT NULL,
	[SecondaryName] [nvarchar](100) NOT NULL,
	[Street] [nvarchar](50) NOT NULL,
	[Town] [nvarchar](50) NOT NULL,
	[Locality] [nvarchar](50) NOT NULL,
	[AdministrativeArea] [nvarchar](50) NOT NULL,
	[CountryId] [uniqueidentifier] NOT NULL,
	[PostCode] [nvarchar](35) NOT NULL,
 CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Producer].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_Country] FOREIGN KEY([CountryId])
REFERENCES [Lookup].[Country] ([Id])
GO

ALTER TABLE [Producer].[Address] CHECK CONSTRAINT [FK_Address_Country]
GO

GO
PRINT N'Creating [Producer].[Contact]...';
GO

CREATE TABLE [Producer].[Contact](
	[Id] [uniqueidentifier] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[Title] [nvarchar](35) NOT NULL,
	[Forename] [nvarchar](35) NOT NULL,
	[Surname] [nvarchar](35) NOT NULL,
	[Telephone] [nvarchar](35) NOT NULL,
	[Mobile] [nvarchar](35) NOT NULL,
	[Fax] [nvarchar](35) NOT NULL,
	[Email] [nvarchar](35) NOT NULL,
	[AddressId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Contact] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Producer].[Contact]  WITH CHECK ADD  CONSTRAINT [FK_Contact_Address] FOREIGN KEY([AddressId])
REFERENCES [Producer].[Address] ([Id])
GO

ALTER TABLE [Producer].[Contact] CHECK CONSTRAINT [FK_Contact_Address]
GO

PRINT N'Creating [Producer].[AuthorisedRepresentative]...';
GO

CREATE TABLE [Producer].[AuthorisedRepresentative](
	[Id] [uniqueidentifier] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[OverseasProducerName] [nvarchar](50) NOT NULL,
	[OverseasContactId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_AuthorisedRepresentative] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Producer].[AuthorisedRepresentative]  WITH CHECK ADD  CONSTRAINT [FK_AuthorisedRepresentative_OverseasContact] FOREIGN KEY([OverseasContactId])
REFERENCES [Producer].[Contact] ([Id])
GO

ALTER TABLE [Producer].[AuthorisedRepresentative] CHECK CONSTRAINT [FK_AuthorisedRepresentative_OverseasContact]

GO
PRINT N'Creating [Producer].[Company]...';
GO

CREATE TABLE [Producer].[Company](
	[Id] [uniqueidentifier] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[CompanyNumber] [nvarchar](8) NOT NULL,
	[RegisteredOfficeId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Producer].[Company]  WITH CHECK ADD  CONSTRAINT [FK_Company_Contact] FOREIGN KEY([RegisteredOfficeId])
REFERENCES [Producer].[Contact] ([Id])
GO

ALTER TABLE [Producer].[Company] CHECK CONSTRAINT [FK_Company_Contact]
GO

PRINT N'Creating [Producer].[Partnership]...';
GO

CREATE TABLE [Producer].[Partnership](
	[Id] [uniqueidentifier] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[PrinciplaPlaceOfBusinessId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Partnership] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Producer].[Partnership]  WITH CHECK ADD  CONSTRAINT [FK_Partnership_Contact] FOREIGN KEY([PrinciplaPlaceOfBusinessId])
REFERENCES [Producer].[Contact] ([Id])
GO

ALTER TABLE [Producer].[Partnership] CHECK CONSTRAINT [FK_Partnership_Contact]
GO

PRINT N'Creating [Producer].[Partner]...';
GO
CREATE TABLE [Producer].[Partner](
	[Id] [uniqueidentifier] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[Name] [nvarchar](70) NOT NULL,
	[PartnershipId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Partner] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Producer].[Partner]  WITH CHECK ADD  CONSTRAINT [FK_Partner_Partnership] FOREIGN KEY([PartnershipId])
REFERENCES [Producer].[Partnership] ([Id])
GO

ALTER TABLE [Producer].[Partner] CHECK CONSTRAINT [FK_Partner_Partnership]
GO

PRINT N'Creating [Producer].[Business]...';
GO
CREATE TABLE [Producer].[Business](
	[Id] [uniqueidentifier] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CorrespondentForNoticesContactId] [uniqueidentifier] NULL,
	[CompanyId] [uniqueidentifier] NULL,
	[PartnershipId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Business] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Producer].[Business]  WITH CHECK ADD  CONSTRAINT [FK_Business_Company] FOREIGN KEY([CompanyId])
REFERENCES [Producer].[Company] ([Id])
GO

ALTER TABLE [Producer].[Business] CHECK CONSTRAINT [FK_Business_Company]
GO

ALTER TABLE [Producer].[Business]  WITH CHECK ADD  CONSTRAINT [FK_Business_Contact] FOREIGN KEY([CorrespondentForNoticesContactId])
REFERENCES [Producer].[Contact] ([Id])
GO

ALTER TABLE [Producer].[Business] CHECK CONSTRAINT [FK_Business_Contact]
GO

ALTER TABLE [Producer].[Business]  WITH CHECK ADD  CONSTRAINT [FK_Business_Partnership] FOREIGN KEY([PartnershipId])
REFERENCES [Producer].[Partnership] ([Id])
GO

ALTER TABLE [Producer].[Business] CHECK CONSTRAINT [FK_Business_Partnership]
GO

PRINT N'Creating [Producer].[Producer]...';
GO
CREATE TABLE [Producer].[Producer](
	[Id] [uniqueidentifier] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[RegistrationNumber] [nvarchar](50) NOT NULL,
	[VATRegistered] [bit] NOT NULL,
	[AnnualTurnover] [float] NOT NULL,
	[CeaseToExist] [datetime] NOT NULL,
	[ObligationType] [int] NOT NULL,
	[EEEPlacedOnMarketBandType] [int] NOT NULL,
	[AnnualTurnoverBandType] [int] NOT NULL,
	[SellingTechniqueType] [int] NOT NULL,
	[ChargeBandType] [int] NOT NULL,
	[PCSId] [uniqueidentifier] NOT NULL,
	[MemberUploadId] [uniqueidentifier] NOT NULL,
	[AuthorisedRepresentativeId] [uniqueidentifier] NOT NULL,
	[ProducerBusinessId] [uniqueidentifier] NOT NULL,
	[LastSubmitted] [datetime] NOT NULL,
 CONSTRAINT [PK_Producer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Producer].[Producer] ADD  CONSTRAINT [DF_Producer_AnnualTurnover]  DEFAULT ((0)) FOR [AnnualTurnover]
GO

ALTER TABLE [Producer].[Producer]  WITH CHECK ADD  CONSTRAINT [FK_Producer_AuthorisedRepresentative] FOREIGN KEY([AuthorisedRepresentativeId])
REFERENCES [Producer].[AuthorisedRepresentative] ([Id])
GO

ALTER TABLE [Producer].[Producer] CHECK CONSTRAINT [FK_Producer_AuthorisedRepresentative]
GO

ALTER TABLE [Producer].[Producer]  WITH CHECK ADD  CONSTRAINT [FK_Producer_Business] FOREIGN KEY([ProducerBusinessId])
REFERENCES [Producer].[Business] ([Id])
GO

ALTER TABLE [Producer].[Producer] CHECK CONSTRAINT [FK_Producer_Business]
GO

ALTER TABLE [Producer].[Producer]  WITH CHECK ADD  CONSTRAINT [FK_Producer_MemberUpload] FOREIGN KEY([MemberUploadId])
REFERENCES [PCS].[MemberUpload] ([Id])
GO

ALTER TABLE [Producer].[Producer] CHECK CONSTRAINT [FK_Producer_MemberUpload]
GO

ALTER TABLE [Producer].[Producer]  WITH CHECK ADD  CONSTRAINT [FK_Producer_Scheme] FOREIGN KEY([PCSId])
REFERENCES [PCS].[Scheme] ([Id])
GO

ALTER TABLE [Producer].[Producer] CHECK CONSTRAINT [FK_Producer_Scheme]
GO

GO
PRINT N'Creating [Producer].[BrandName]...';
CREATE TABLE [Producer].[BrandName](
	[Id] [uniqueidentifier] NOT NULL,
	[BrandName] [nvarchar](255) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[ProducerId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_BrandNameList] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Producer].[BrandName]  WITH CHECK ADD  CONSTRAINT [FK_BrandNameList_Producer] FOREIGN KEY([ProducerId])
REFERENCES [Producer].[Producer] ([Id])
GO

ALTER TABLE [Producer].[BrandName] CHECK CONSTRAINT [FK_BrandNameList_Producer]
GO

PRINT N'Creating [Producer].[BrandName]...';

CREATE TABLE [Producer].[SICCode](
	[Id] [uniqueidentifier] NOT NULL,
	[ProducerId] [uniqueidentifier] NOT NULL,
	[SICCode] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_SICCodeList] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Producer].[SICCode]  WITH CHECK ADD  CONSTRAINT [FK_SICCodeList_Producer] FOREIGN KEY([ProducerId])
REFERENCES [Producer].[Producer] ([Id])
GO

ALTER TABLE [Producer].[SICCode] CHECK CONSTRAINT [FK_SICCodeList_Producer]
GO

PRINT N'Update complete.';
