

GO
PRINT N'Dropping FK_MemberUploadError_MemberUpload...';


GO
ALTER TABLE [PCS].[MemberUploadError] DROP CONSTRAINT [FK_MemberUploadError_MemberUpload];


GO
/*
The column [PCS].[MemberUploadError].[ErrorType] on table [PCS].[MemberUploadError] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.
*/
GO
PRINT N'Starting rebuilding table [PCS].[MemberUploadError]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [PCS].[tmp_ms_xx_MemberUploadError] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [RowVersion]     ROWVERSION       NOT NULL,
    [ErrorLevel]     INT              NOT NULL,
    [ErrorType]      INT              NOT NULL,
    [Description]    NVARCHAR (MAX)   NOT NULL,
    [MemberUploadId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_MemberUploadError] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [PCS].[MemberUploadError])
    BEGIN
        INSERT INTO [PCS].[tmp_ms_xx_MemberUploadError] ([Id], [ErrorLevel], [ErrorType], [Description], [MemberUploadId])
        SELECT   [Id],
                 [ErrorLevel],
                 1,
                 [Description],
                 [MemberUploadId]
        FROM     [PCS].[MemberUploadError]
        ORDER BY [Id] ASC;
    END

DROP TABLE [PCS].[MemberUploadError];

EXECUTE sp_rename N'[PCS].[tmp_ms_xx_MemberUploadError]', N'MemberUploadError';

EXECUTE sp_rename N'[PCS].[tmp_ms_xx_constraint_PK_MemberUploadError]', N'PK_MemberUploadError', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating FK_MemberUploadError_MemberUpload...';


GO
ALTER TABLE [PCS].[MemberUploadError] WITH NOCHECK
    ADD CONSTRAINT [FK_MemberUploadError_MemberUpload] FOREIGN KEY ([MemberUploadId]) REFERENCES [PCS].[MemberUpload] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [PCS].[MemberUploadError] WITH CHECK CHECK CONSTRAINT [FK_MemberUploadError_MemberUpload];


GO
PRINT N'Update complete.';


GO
