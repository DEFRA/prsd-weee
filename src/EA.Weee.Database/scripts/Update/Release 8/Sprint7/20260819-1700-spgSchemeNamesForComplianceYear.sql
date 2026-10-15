SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Create date: 2026 Aug 19
-- Modified date: 2026 Aug 19
-- Description:	This stored procedure aggregates Scheme Names from multiple
--				sources for which there is data for a given Compliance Year.
-- =============================================
CREATE PROCEDURE [PCS].[spgSchemeNamesForComplianceYear]
    @ComplianceYear INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		s.SchemeName
	FROM
		[PCS].[Scheme] s
	INNER JOIN
		[PCS].[MemberUpload] mu
			ON mu.SchemeId = s.Id
	WHERE
		(mu.ComplianceYear = @ComplianceYear OR @ComplianceYear IS NULL)
	AND
		mu.ComplianceYear < YEAR(GETDATE()) - 7

	UNION

	SELECT	
		s.SchemeName
	FROM
		[PCS].[Scheme] s
	INNER JOIN
		[PCS].[ObligationScheme] os
			ON os.SchemeId = s.Id
	WHERE	
		(os.ComplianceYear = @ComplianceYear OR @ComplianceYear IS NULL)
	AND
		os.ComplianceYear < YEAR(GETDATE()) - 7

	UNION

	SELECT	
		s.SchemeName
	FROM
		[PCS].[Scheme] s
	INNER JOIN
		[PCS].[DataReturnUpload] dru
			ON dru.SchemeId = s.Id
	WHERE	
		(dru.ComplianceYear = @ComplianceYear OR @ComplianceYear IS NULL)
	AND
		dru.ComplianceYear < YEAR(GETDATE()) - 7

	UNION

	SELECT	
		s.SchemeName
	FROM
		[PCS].[Scheme] s
	INNER JOIN
		[PCS].[DataReturn] dr
			ON dr.SchemeId = s.Id
	WHERE	
		(dr.ComplianceYear = @ComplianceYear OR @ComplianceYear IS NULL)
	AND
		dr.ComplianceYear < YEAR(GETDATE()) - 7

	UNION

	SELECT	
		s.SchemeName
	FROM
		[PCS].[Scheme] s
	INNER JOIN
		[Producer].[RegisteredProducer] rp
			ON rp.SchemeId = s.Id
	WHERE	
		(rp.ComplianceYear = @ComplianceYear OR @ComplianceYear IS NULL)
	AND
		rp.ComplianceYear < YEAR(GETDATE()) - 7
END