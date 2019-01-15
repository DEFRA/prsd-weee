

GO
PRINT N'Dropping FK_MemberUpload_Organisation...';


GO
ALTER TABLE [PCS].[MemberUpload] DROP CONSTRAINT [FK_MemberUpload_Organisation];


GO
PRINT N'Dropping FK_Producer_MemberUpload...';


GO
ALTER TABLE [Producer].[Producer] DROP CONSTRAINT [FK_Producer_MemberUpload];


GO
PRINT N'Dropping FK_MemberUploadError_MemberUpload...';


GO
ALTER TABLE [PCS].[MemberUploadError] DROP CONSTRAINT [FK_MemberUploadError_MemberUpload];


GO
PRINT N'Dropping FK_MemberUpload_Scheme...';


GO
ALTER TABLE [PCS].[MemberUpload] DROP CONSTRAINT [FK_MemberUpload_Scheme];


GO
PRINT N'Starting rebuilding table [PCS].[MemberUpload]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [PCS].[tmp_ms_xx_MemberUpload] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [RowVersion]     ROWVERSION       NOT NULL,
    [OrganisationId] UNIQUEIDENTIFIER NOT NULL,
    [Data]           NVARCHAR (MAX)   NOT NULL,
    [ComplianceYear] INT              NOT NULL,
    [SchemeId]       UNIQUEIDENTIFIER NULL,
    [IsSubmitted]    BIT              NOT NULL,
    [TotalCharges]   DECIMAL (18, 2)  CONSTRAINT [DF_MemberUpload_TotalCharges] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_MemberUpload] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [PCS].[MemberUpload])
    BEGIN
        INSERT INTO [PCS].[tmp_ms_xx_MemberUpload] ([Id], [OrganisationId], [SchemeId], [Data], [ComplianceYear], [IsSubmitted])
        SELECT   [Id],
                 [OrganisationId],
                 [SchemeId],
                 [Data],
                 [ComplianceYear],
                 [IsSubmitted]
        FROM     [PCS].[MemberUpload]
        ORDER BY [Id] ASC;
    END

DROP TABLE [PCS].[MemberUpload];

EXECUTE sp_rename N'[PCS].[tmp_ms_xx_MemberUpload]', N'MemberUpload';

EXECUTE sp_rename N'[PCS].[tmp_ms_xx_constraint_PK_MemberUpload]', N'PK_MemberUpload', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating [Producer].[ProducerChargeBand]...';


GO
CREATE TABLE [Producer].[ProducerChargeBand] (
    [Id]     UNIQUEIDENTIFIER NOT NULL,
    [Name]   NVARCHAR (10)    NOT NULL,
    [Amount] DECIMAL (18, 2)  NOT NULL,
	[RowVersion] ROWVERSION       NOT NULL,
    CONSTRAINT [PK_ProducerChargeBand] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating FK_MemberUpload_Organisation...';


GO
ALTER TABLE [PCS].[MemberUpload] WITH NOCHECK
    ADD CONSTRAINT [FK_MemberUpload_Organisation] FOREIGN KEY ([OrganisationId]) REFERENCES [Organisation].[Organisation] ([Id]);


GO
PRINT N'Creating FK_Producer_MemberUpload...';


GO
ALTER TABLE [Producer].[Producer] WITH NOCHECK
    ADD CONSTRAINT [FK_Producer_MemberUpload] FOREIGN KEY ([MemberUploadId]) REFERENCES [PCS].[MemberUpload] ([Id]);


GO
PRINT N'Creating FK_MemberUploadError_MemberUpload...';


GO
ALTER TABLE [PCS].[MemberUploadError] WITH NOCHECK
    ADD CONSTRAINT [FK_MemberUploadError_MemberUpload] FOREIGN KEY ([MemberUploadId]) REFERENCES [PCS].[MemberUpload] ([Id]);


GO
PRINT N'Creating FK_MemberUpload_Scheme...';


GO
ALTER TABLE [PCS].[MemberUpload] WITH NOCHECK
    ADD CONSTRAINT [FK_MemberUpload_Scheme] FOREIGN KEY ([SchemeId]) REFERENCES [PCS].[Scheme] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [PCS].[MemberUpload] WITH CHECK CHECK CONSTRAINT [FK_MemberUpload_Organisation];

ALTER TABLE [Producer].[Producer] WITH CHECK CHECK CONSTRAINT [FK_Producer_MemberUpload];

ALTER TABLE [PCS].[MemberUploadError] WITH CHECK CHECK CONSTRAINT [FK_MemberUploadError_MemberUpload];

ALTER TABLE [PCS].[MemberUpload] WITH CHECK CHECK CONSTRAINT [FK_MemberUpload_Scheme];

GO
PRINT N'Adding records for producerChargeBand...';

GO
INSERT INTO [Producer].[ProducerChargeBand] ([Id], [Name], [Amount]) VALUES(NEWID(), 'A', 445)

INSERT INTO [Producer].[ProducerChargeBand] ([Id], [Name], [Amount]) VALUES(NEWID(), 'B', 210)

INSERT INTO [Producer].[ProducerChargeBand] ([Id], [Name], [Amount]) VALUES(NEWID(), 'C', 30)

INSERT INTO [Producer].[ProducerChargeBand] ([Id], [Name], [Amount]) VALUES(NEWID(), 'D', 30)

INSERT INTO [Producer].[ProducerChargeBand] ([Id], [Name], [Amount]) VALUES(NEWID(), 'E', 30)

GO
PRINT N'Update complete.';


GO
