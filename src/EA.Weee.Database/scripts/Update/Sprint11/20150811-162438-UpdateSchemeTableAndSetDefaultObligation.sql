

GO
PRINT N'Altering [PCS].[Scheme]...';

Go
Update [PCS].[Scheme] set [ObligationType] = 0

GO
ALTER TABLE [PCS].[Scheme]
    ADD [SchemeName]            NVARCHAR (16)    NULL,
        [IbisCustomerReference] NVARCHAR (10)    NULL,
        [ObligationType]        INT              CONSTRAINT [DF_Scheme_ObligationType] DEFAULT ((0)) NOT NULL,
        [CompetentAuthorityId]  UNIQUEIDENTIFIER NULL;


GO
PRINT N'Update complete.';


GO
