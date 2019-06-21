GO
PRINT N'Starting rebuilding table [Lookup].[CompetentAuthority]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [Lookup].[tmp_ms_xx_CompetentAuthority](
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR (1023)    NOT NULL,
    [Abbreviation]  NVARCHAR (65)    NOT NULL,
    [CountryId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_CompetentAuthority_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

DROP TABLE [Lookup].[CompetentAuthority];

EXECUTE sp_rename N'[Lookup].[tmp_ms_xx_CompetentAuthority]', N'CompetentAuthority';

EXECUTE sp_rename N'[Lookup].[tmp_ms_xx_constraint_PK_CompetentAuthority_Id]', N'PK_CompetentAuthority_Id', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

ALTER TABLE [Lookup].[CompetentAuthority] WITH NOCHECK
    ADD CONSTRAINT [FK_CompetentAuthority_Country] FOREIGN KEY ([CountryId]) REFERENCES [Lookup].[Country] ([Id]);
GO
PRINT N'Update complete.';
GO


GO