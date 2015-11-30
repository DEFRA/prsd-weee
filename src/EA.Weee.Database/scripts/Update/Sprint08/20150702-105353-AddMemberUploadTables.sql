

GO
PRINT N'Creating [PCS]...';


GO
CREATE SCHEMA [PCS]
    AUTHORIZATION [dbo];


GO
PRINT N'Creating [PCS].[MemberUploadError]...';


GO
CREATE TABLE [PCS].[MemberUploadError] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [RowVersion]     ROWVERSION       NOT NULL,
    [ErrorLevel]     INT              NOT NULL,
    [Description]    NVARCHAR (MAX)   NOT NULL,
    [MemberUploadId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_MemberUploadError] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [PCS].[MemberUpload]...';


GO
CREATE TABLE [PCS].[MemberUpload] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [RowVersion]     ROWVERSION       NOT NULL,
    [OrganisationId] UNIQUEIDENTIFIER NOT NULL,
    [Data]           NVARCHAR (MAX)   NOT NULL,
    CONSTRAINT [PK_MemberUpload] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating FK_MemberUploadError_MemberUpload...';


GO
ALTER TABLE [PCS].[MemberUploadError] WITH NOCHECK
    ADD CONSTRAINT [FK_MemberUploadError_MemberUpload] FOREIGN KEY ([MemberUploadId]) REFERENCES [PCS].[MemberUpload] ([Id]);


GO
PRINT N'Creating FK_MemberUpload_Organisation...';


GO
ALTER TABLE [PCS].[MemberUpload] WITH NOCHECK
    ADD CONSTRAINT [FK_MemberUpload_Organisation] FOREIGN KEY ([OrganisationId]) REFERENCES [Organisation].[Organisation] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [PCS].[MemberUploadError] WITH CHECK CHECK CONSTRAINT [FK_MemberUploadError_MemberUpload];

ALTER TABLE [PCS].[MemberUpload] WITH CHECK CHECK CONSTRAINT [FK_MemberUpload_Organisation];


GO
PRINT N'Update complete.';


GO
