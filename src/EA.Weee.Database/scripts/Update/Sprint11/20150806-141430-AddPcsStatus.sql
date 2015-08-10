

GO
PRINT N'Dropping FK_Producer_Scheme...';


GO
ALTER TABLE [Producer].[Producer] DROP CONSTRAINT [FK_Producer_Scheme];


GO
PRINT N'Dropping FK_MemberUpload_Scheme...';


GO
ALTER TABLE [PCS].[MemberUpload] DROP CONSTRAINT [FK_MemberUpload_Scheme];


GO
PRINT N'Dropping FK_Scheme_Organisation...';


GO
ALTER TABLE [PCS].[Scheme] DROP CONSTRAINT [FK_Scheme_Organisation];


GO
PRINT N'Starting rebuilding table [PCS].[Scheme]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [PCS].[tmp_ms_xx_Scheme] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [RowVersion]     ROWVERSION       NOT NULL,
    [PCSStatus]      INT              NOT NULL,
    [ApprovalNumber] NVARCHAR (50)    NULL,
    [OrganisationId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_Scheme] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [PCS].[Scheme])
    BEGIN
        INSERT INTO [PCS].[tmp_ms_xx_Scheme] ([Id], [PCSStatus], [ApprovalNumber], [OrganisationId])
        SELECT   [Id],
                 1, /* pending */
                 [ApprovalNumber],
                 [OrganisationId]
        FROM     [PCS].[Scheme]
        ORDER BY [Id] ASC;
    END

DROP TABLE [PCS].[Scheme];

EXECUTE sp_rename N'[PCS].[tmp_ms_xx_Scheme]', N'Scheme';

EXECUTE sp_rename N'[PCS].[tmp_ms_xx_constraint_PK_Scheme]', N'PK_Scheme', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating FK_Producer_Scheme...';


GO
ALTER TABLE [Producer].[Producer] WITH NOCHECK
    ADD CONSTRAINT [FK_Producer_Scheme] FOREIGN KEY ([SchemeId]) REFERENCES [PCS].[Scheme] ([Id]);


GO
PRINT N'Creating FK_MemberUpload_Scheme...';


GO
ALTER TABLE [PCS].[MemberUpload] WITH NOCHECK
    ADD CONSTRAINT [FK_MemberUpload_Scheme] FOREIGN KEY ([SchemeId]) REFERENCES [PCS].[Scheme] ([Id]);


GO
PRINT N'Creating FK_Scheme_Organisation...';


GO
ALTER TABLE [PCS].[Scheme] WITH NOCHECK
    ADD CONSTRAINT [FK_Scheme_Organisation] FOREIGN KEY ([OrganisationId]) REFERENCES [Organisation].[Organisation] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [Producer].[Producer] WITH CHECK CHECK CONSTRAINT [FK_Producer_Scheme];

ALTER TABLE [PCS].[MemberUpload] WITH CHECK CHECK CONSTRAINT [FK_MemberUpload_Scheme];

ALTER TABLE [PCS].[Scheme] WITH CHECK CHECK CONSTRAINT [FK_Scheme_Organisation];


GO
PRINT N'Update complete.';


GO
