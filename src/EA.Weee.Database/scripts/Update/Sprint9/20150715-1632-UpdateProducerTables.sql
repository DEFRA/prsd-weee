
GO
PRINT N'Altering [Producer].[SICCode]...';


GO
ALTER TABLE [Producer].[SICCode] 
	DROP Column [SICCODE];

GO	
ALTER TABLE [Producer].[SICCode] 
ADD [Name] nvarchar(10) NOT NULL,
    [RowVersion] ROWVERSION NOT NULL;

GO
PRINT N'Altering [Producer].[BrandName]...';

GO
ALTER TABLE [Producer].[BrandName] 
	DROP Column [BrandName];

GO
ALTER TABLE [Producer].[BrandName] 
ADD [Name] nvarchar(10) NOT NULL;
 
 GO
PRINT N'Altering [Producer].[Company]...';

ALTER TABLE [Producer].[Company] DROP CONSTRAINT [FK_Company_Contact];

ALTER TABLE [Producer].[Company]
	DROP COLUMN [RegisteredOfficeId];

ALTER TABLE [Producer].[Company]
	ADD [RegisteredOfficeContactId] [uniqueidentifier] NOT NULL;
 GO

ALTER TABLE [Producer].[Company]  WITH CHECK ADD  CONSTRAINT [FK_Company_Contact] FOREIGN KEY([RegisteredOfficeContactId])
REFERENCES [Producer].[Contact] ([Id])
GO
ALTER TABLE [Producer].[Company] CHECK CONSTRAINT [FK_Company_Contact]
GO

GO
PRINT N'Altering [Producer].[Partnership]...';

ALTER TABLE [Producer].[Partnership] DROP CONSTRAINT [FK_Partnership_Contact];

ALTER TABLE [Producer].[Partnership]
	DROP COLUMN [PrinciplaPlaceOfBusinessId];

ALTER TABLE [Producer].[Partnership]
	ADD [PrincipalPlaceOfBusinessId] [uniqueidentifier] NOT NULL;

ALTER TABLE [Producer].[Partnership]  WITH CHECK ADD  CONSTRAINT [FK_Partnership_Contact] FOREIGN KEY([PrincipalPlaceOfBusinessId])
REFERENCES [Producer].[Contact] ([Id])
GO

ALTER TABLE [Producer].[Partnership] CHECK CONSTRAINT [FK_Partnership_Contact]
GO

PRINT N'Altering [Producer].[AuthorisedRepresentative]...';
GO
ALTER TABLE [Producer].[AuthorisedRepresentative]
 ALTER COLUMN [OverseasContactId] [uniqueidentifier] NULL;

PRINT N'Altering [Producer].[Producer]...';

ALTER TABLE [Producer].[Producer] DROP CONSTRAINT [FK_Producer_Scheme];

ALTER TABLE [Producer].[Producer]
	DROP COLUMN [PCSId];

ALTER TABLE [Producer].[Producer]
 ALTER COLUMN [AuthorisedRepresentativeId] [uniqueidentifier] NULL;

ALTER TABLE [Producer].[Producer] DROP CONSTRAINT [DF_Producer_AnnualTurnover];
GO

 ALTER TABLE [Producer].[Producer]
	ALTER COLUMN [AnnualTurnover] [decimal](18,2) NOT NULL;
GO

ALTER TABLE [Producer].[Producer] ADD  CONSTRAINT [DF_Producer_AnnualTurnover]  DEFAULT ((0)) FOR [AnnualTurnover]
GO

ALTER TABLE [Producer].[Producer]
 ALTER COLUMN [CeaseToExist] [datetime] NULL;

 ALTER TABLE [Producer].[Producer]
 ADD [TradingName] nvarchar(255) NOT NULL,
     [SchemeId] [uniqueidentifier] NOT NULL;

 ALTER TABLE [Producer].[Producer]  WITH CHECK ADD  CONSTRAINT [FK_Producer_Scheme] FOREIGN KEY([SchemeId])
REFERENCES [PCS].[Scheme] ([Id])
GO

ALTER TABLE [Producer].[Producer] CHECK CONSTRAINT [FK_Producer_Scheme]
GO

GO
PRINT N'Altering [PCS].[MemberUpload]...';

GO
PRINT N'Dropping FK_MemberUpload_Organisation...';


GO
ALTER TABLE [PCS].[MemberUpload] DROP CONSTRAINT [FK_MemberUpload_Organisation];
GO

GO
PRINT N'Dropping FK_MemberUploadError_MemberUpload...';

GO
ALTER TABLE [PCS].[MemberUploadError] DROP CONSTRAINT [FK_MemberUploadError_MemberUpload];
GO

GO
PRINT N'Dropping FK_Producer_MemberUpload...';


GO
ALTER TABLE [Producer].[Producer] DROP CONSTRAINT [FK_Producer_MemberUpload];
GO


GO
PRINT N'Starting rebuilding table [PCS].[MemberUpload]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

    CREATE TABLE [PCS].[tmp_ms_xx_MemberUpload](
	[Id] [uniqueidentifier] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[OrganisationId] [uniqueidentifier] NOT NULL,
		[SchemeId] [uniqueidentifier] NULL,
	[Data] [nvarchar](max) NOT NULL,
	[ComplianceYear] [int] NOT NULL,	
	[IsSubmitted] [bit] NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_MemberUpload_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [PCS].[MemberUpload])
    BEGIN
        INSERT INTO [PCS].[tmp_ms_xx_MemberUpload] ([Id], [OrganisationId], [Data], [ComplianceYear], [IsSubmitted])
        SELECT  [Id], 
				[OrganisationId], 
				[Data], 
				2016,  
				1				
        FROM     [PCS].[MemberUpload]
        ORDER BY [Id] ASC;
    END

DROP TABLE [PCS].[MemberUpload];

EXECUTE sp_rename N'[PCS].[tmp_ms_xx_MemberUpload]', N'MemberUpload';

EXECUTE sp_rename N'[PCS].[tmp_ms_xx_constraint_PK_MemberUpload_Id]', N'PK_MemberUpload_Id', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating FK_MemberUpload_Organisation...';

GO
ALTER TABLE [PCS].[MemberUpload]  WITH CHECK ADD  CONSTRAINT [FK_MemberUpload_Organisation] FOREIGN KEY([OrganisationId]) REFERENCES [Organisation].[Organisation] ([Id])
GO

GO
PRINT N'Creating FK_MemberUpload_Scheme...';

ALTER TABLE [PCS].[MemberUpload]  WITH CHECK ADD  CONSTRAINT [FK_MemberUpload_Scheme] FOREIGN KEY([SchemeId]) REFERENCES [PCS].[Scheme] ([Id])
GO

ALTER TABLE [PCS].[MemberUpload] CHECK CONSTRAINT [FK_MemberUpload_Scheme]
GO

GO
PRINT N'Creating FK_MemberUploadError_MemberUpload...';


GO
ALTER TABLE [PCS].[MemberUploadError] WITH NOCHECK
    ADD CONSTRAINT [FK_MemberUploadError_MemberUpload] FOREIGN KEY ([MemberUploadId]) REFERENCES [PCS].[MemberUpload] ([Id]);


GO
PRINT N'Creating FK_Producer_MemberUpload...';

ALTER TABLE [Producer].[Producer]  WITH CHECK ADD  CONSTRAINT [FK_Producer_MemberUpload] FOREIGN KEY([MemberUploadId]) REFERENCES [PCS].[MemberUpload] ([Id])
GO


ALTER TABLE [PCS].[MemberUpload] WITH CHECK CHECK CONSTRAINT [FK_MemberUpload_Organisation];

ALTER TABLE [PCS].[MemberUploadError] WITH CHECK CHECK CONSTRAINT [FK_MemberUploadError_MemberUpload];

ALTER TABLE [Producer].[Producer] WITH CHECK CHECK CONSTRAINT [FK_Producer_MemberUpload];

PRINT N'Update complete.';
GO
