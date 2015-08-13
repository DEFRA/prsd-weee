

GO
PRINT N'Altering [PCS].[Scheme]...';

GO
ALTER TABLE [PCS].[Scheme]
    ADD [SchemeName]            NVARCHAR (16)    NULL,
        [IbisCustomerReference] NVARCHAR (10)    NULL,
        [ObligationType]        INT              NULL,
        [CompetentAuthorityId]  UNIQUEIDENTIFIER NULL;


GO
PRINT N'Update complete.';


GO
