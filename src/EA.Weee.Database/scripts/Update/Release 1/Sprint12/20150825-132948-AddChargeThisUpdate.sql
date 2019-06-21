

GO
PRINT N'Dropping DF_Producer_AnnualTurnover...';


GO
ALTER TABLE [Producer].[Producer] DROP CONSTRAINT [DF_Producer_AnnualTurnover];


GO
PRINT N'Dropping DF_Producer_IsCurrentForComplianceYear...';


GO
ALTER TABLE [Producer].[Producer] DROP CONSTRAINT [DF_Producer_IsCurrentForComplianceYear];


GO
PRINT N'Dropping FK_Producer_Business...';


GO
ALTER TABLE [Producer].[Producer] DROP CONSTRAINT [FK_Producer_Business];


GO
PRINT N'Dropping FK_Producer_Scheme...';


GO
ALTER TABLE [Producer].[Producer] DROP CONSTRAINT [FK_Producer_Scheme];


GO
PRINT N'Dropping FK_BrandNameList_Producer...';


GO
ALTER TABLE [Producer].[BrandName] DROP CONSTRAINT [FK_BrandNameList_Producer];


GO
PRINT N'Dropping FK_Producer_AuthorisedRepresentative...';


GO
ALTER TABLE [Producer].[Producer] DROP CONSTRAINT [FK_Producer_AuthorisedRepresentative];


GO
PRINT N'Dropping FK_Producer_MemberUpload...';


GO
ALTER TABLE [Producer].[Producer] DROP CONSTRAINT [FK_Producer_MemberUpload];


GO
PRINT N'Dropping FK_SICCodeList_Producer...';


GO
ALTER TABLE [Producer].[SICCode] DROP CONSTRAINT [FK_SICCodeList_Producer];


GO
PRINT N'Starting rebuilding table [Producer].[Producer]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [Producer].[tmp_ms_xx_Producer] (
    [Id]                         UNIQUEIDENTIFIER NOT NULL,
    [RowVersion]                 ROWVERSION       NOT NULL,
    [RegistrationNumber]         NVARCHAR (50)    NOT NULL,
    [VATRegistered]              BIT              NOT NULL,
    [AnnualTurnover]             DECIMAL (18, 2)  CONSTRAINT [DF_Producer_AnnualTurnover] DEFAULT ((0)) NOT NULL,
    [CeaseToExist]               DATETIME         NULL,
    [ObligationType]             INT              NOT NULL,
    [EEEPlacedOnMarketBandType]  INT              NOT NULL,
    [AnnualTurnoverBandType]     INT              NOT NULL,
    [SellingTechniqueType]       INT              NOT NULL,
    [ChargeBandType]             INT              NOT NULL,
    [ChargeThisUpdate]           DECIMAL (18, 2)  NOT NULL,
    [MemberUploadId]             UNIQUEIDENTIFIER NOT NULL,
    [AuthorisedRepresentativeId] UNIQUEIDENTIFIER NULL,
    [ProducerBusinessId]         UNIQUEIDENTIFIER NOT NULL,
    [UpdatedDate]                DATETIME         NOT NULL,
    [TradingName]                NVARCHAR (255)   NOT NULL,
    [SchemeId]                   UNIQUEIDENTIFIER NOT NULL,
    [IsCurrentForComplianceYear] BIT              CONSTRAINT [DF_Producer_IsCurrentForComplianceYear] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_Producer] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [Producer].[Producer])
    BEGIN
        INSERT INTO [Producer].[tmp_ms_xx_Producer] ([Id], [RegistrationNumber], [VATRegistered], [AnnualTurnover], [CeaseToExist], [ObligationType], [EEEPlacedOnMarketBandType], [AnnualTurnoverBandType], [SellingTechniqueType], [ChargeBandType], [ChargeThisUpdate], [MemberUploadId], [AuthorisedRepresentativeId], [ProducerBusinessId], [UpdatedDate], [TradingName], [SchemeId], [IsCurrentForComplianceYear])
        SELECT   [Id],
                 [RegistrationNumber],
                 [VATRegistered],
                 [AnnualTurnover],
                 [CeaseToExist],
                 [ObligationType],
                 [EEEPlacedOnMarketBandType],
                 [AnnualTurnoverBandType],
                 [SellingTechniqueType],
                 [ChargeBandType],
                 0,
                 [MemberUploadId],
                 [AuthorisedRepresentativeId],
                 [ProducerBusinessId],
                 [UpdatedDate],
                 [TradingName],
                 [SchemeId],
                 [IsCurrentForComplianceYear]
        FROM     [Producer].[Producer]
        ORDER BY [Id] ASC;
    END

DROP TABLE [Producer].[Producer];

EXECUTE sp_rename N'[Producer].[tmp_ms_xx_Producer]', N'Producer';

EXECUTE sp_rename N'[Producer].[tmp_ms_xx_constraint_PK_Producer]', N'PK_Producer', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating [Producer].[Producer].[IX_Producer_IsCurrentOnly]...';


GO
CREATE NONCLUSTERED INDEX [IX_Producer_IsCurrentOnly]
    ON [Producer].[Producer]([MemberUploadId] ASC, [SchemeId] ASC)
    INCLUDE([RegistrationNumber]) WHERE ([IsCurrentForComplianceYear]=(1));


GO
PRINT N'Creating FK_Producer_Business...';


GO
ALTER TABLE [Producer].[Producer] WITH NOCHECK
    ADD CONSTRAINT [FK_Producer_Business] FOREIGN KEY ([ProducerBusinessId]) REFERENCES [Producer].[Business] ([Id]);


GO
PRINT N'Creating FK_Producer_Scheme...';


GO
ALTER TABLE [Producer].[Producer] WITH NOCHECK
    ADD CONSTRAINT [FK_Producer_Scheme] FOREIGN KEY ([SchemeId]) REFERENCES [PCS].[Scheme] ([Id]);


GO
PRINT N'Creating FK_BrandNameList_Producer...';


GO
ALTER TABLE [Producer].[BrandName] WITH NOCHECK
    ADD CONSTRAINT [FK_BrandNameList_Producer] FOREIGN KEY ([ProducerId]) REFERENCES [Producer].[Producer] ([Id]);


GO
PRINT N'Creating FK_Producer_AuthorisedRepresentative...';


GO
ALTER TABLE [Producer].[Producer] WITH NOCHECK
    ADD CONSTRAINT [FK_Producer_AuthorisedRepresentative] FOREIGN KEY ([AuthorisedRepresentativeId]) REFERENCES [Producer].[AuthorisedRepresentative] ([Id]);


GO
PRINT N'Creating FK_Producer_MemberUpload...';


GO
ALTER TABLE [Producer].[Producer] WITH NOCHECK
    ADD CONSTRAINT [FK_Producer_MemberUpload] FOREIGN KEY ([MemberUploadId]) REFERENCES [PCS].[MemberUpload] ([Id]);


GO
PRINT N'Creating FK_SICCodeList_Producer...';


GO
ALTER TABLE [Producer].[SICCode] WITH NOCHECK
    ADD CONSTRAINT [FK_SICCodeList_Producer] FOREIGN KEY ([ProducerId]) REFERENCES [Producer].[Producer] ([Id]);


GO
PRINT N'Refreshing [Producer].[sppRefreshProducerIsCurrent]...';


GO
EXECUTE sp_refreshsqlmodule N'[Producer].[sppRefreshProducerIsCurrent]';


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [Producer].[Producer] WITH CHECK CHECK CONSTRAINT [FK_Producer_Business];

ALTER TABLE [Producer].[Producer] WITH CHECK CHECK CONSTRAINT [FK_Producer_Scheme];

ALTER TABLE [Producer].[BrandName] WITH CHECK CHECK CONSTRAINT [FK_BrandNameList_Producer];

ALTER TABLE [Producer].[Producer] WITH CHECK CHECK CONSTRAINT [FK_Producer_AuthorisedRepresentative];

ALTER TABLE [Producer].[Producer] WITH CHECK CHECK CONSTRAINT [FK_Producer_MemberUpload];

ALTER TABLE [Producer].[SICCode] WITH CHECK CHECK CONSTRAINT [FK_SICCodeList_Producer];


GO
PRINT N'Update complete.';


GO
