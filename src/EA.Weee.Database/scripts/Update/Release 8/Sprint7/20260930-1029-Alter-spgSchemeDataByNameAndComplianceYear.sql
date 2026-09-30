SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
/*
========================================================================================
Object: StoredProcedure [PCS].[spgSchemeDataByNameAndComplianceYear]
-- Modified date: 2026 Aug 21
-- Description: Aggregates Scheme data from multiple sources
--              which can be filtered by SchemeName, ComplianceYear,
--              CompetentAuthority or any combination of these
========================================================================================
*/

ALTER PROCEDURE [PCS].[spgSchemeDataByNameAndComplianceYear]
    @ComplianceYear INT = NULL,
    @SchemeName NVARCHAR(70) = NULL,
    @CompetentAuthorityId NVARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH SchemeData AS
    (
        -- Member Upload
        SELECT
            s.SchemeName,
            s.Id AS SchemeId,
            s.ApprovalNumber,
            mu.ComplianceYear,
            s.CompetentAuthorityId
        FROM [PCS].[Scheme] AS s
        INNER JOIN [PCS].[MemberUpload] AS mu
            ON mu.SchemeId = s.Id

        UNION

        -- Obligation Scheme
        SELECT
            s.SchemeName,
            s.Id AS SchemeId,
            s.ApprovalNumber,
            os.ComplianceYear,
            s.CompetentAuthorityId
        FROM [PCS].[Scheme] AS s
        INNER JOIN [PCS].[ObligationScheme] AS os
            ON os.SchemeId = s.Id

        UNION

        -- Data Return Upload
        SELECT
            s.SchemeName,
            s.Id AS SchemeId,
            s.ApprovalNumber,
            dru.ComplianceYear,
            s.CompetentAuthorityId
        FROM [PCS].[Scheme] AS s
        INNER JOIN [PCS].[DataReturnUpload] AS dru
            ON dru.SchemeId = s.Id

        UNION

        -- Data Return
        SELECT
            s.SchemeName,
            s.Id AS SchemeId,
            s.ApprovalNumber,
            dr.ComplianceYear,
            s.CompetentAuthorityId
        FROM [PCS].[Scheme] AS s
        INNER JOIN [PCS].[DataReturn] AS dr
            ON dr.SchemeId = s.Id

        UNION

        -- Registered Producer
        SELECT
            s.SchemeName,
            s.Id AS SchemeId,
            s.ApprovalNumber,
            rp.ComplianceYear,
            s.CompetentAuthorityId
        FROM [PCS].[Scheme] AS s
        INNER JOIN [Producer].[RegisteredProducer] AS rp
            ON rp.SchemeId = s.Id
    )
    SELECT
        SchemeName,
        SchemeId,
        ApprovalNumber,
        ComplianceYear
    FROM SchemeData
    WHERE
        ComplianceYear IS NOT NULL
        AND ComplianceYear < YEAR(GETDATE()) - 7

        AND (
            @ComplianceYear IS NULL
            OR ComplianceYear = @ComplianceYear
        )

        AND (
            @SchemeName IS NULL
            OR SchemeName = @SchemeName
        )

        AND (
            @CompetentAuthorityId IS NULL
            OR CompetentAuthorityId = @CompetentAuthorityId
        )

    ORDER BY
        SchemeName,
        ComplianceYear;
END
