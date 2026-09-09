SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Create date: 2026 Aug 21
-- Modified date: 2026 Aug 21
-- Description:	This stored procedure aggregates Scheme data from multiple sources
--				which can be filtered by SchemeName, ComplianceYear or both
-- =============================================
CREATE PROCEDURE [PCS].[spgSchemeDataByNameAndComplianceYear]
    @ComplianceYear INT NULL,
	@SchemeName	NVARCHAR NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		s.SchemeName,
		s.Id AS SchemeId,
		s.ApprovalNumber,
		mu.ComplianceYear
	INTO
		#TempSchemeData
	FROM
		[PCS].[Scheme] s
	INNER JOIN
		[PCS].[MemberUpload] mu
			ON mu.SchemeId = s.Id
	WHERE
		(mu.ComplianceYear = @ComplianceYear 
			OR @ComplianceYear IS NULL)
	AND		
		mu.ComplianceYear < YEAR(GETDATE()) - 7
	AND
		(s.SchemeName = @SchemeName 
			OR @SchemeName IS NULL)

	UNION

	SELECT
		s.SchemeName,
		s.Id AS SchemeId,
		s.ApprovalNumber,
		os.ComplianceYear
	FROM
		[PCS].[Scheme] s
	INNER JOIN
		[PCS].[ObligationScheme] os
			ON os.SchemeId = s.Id
	WHERE
		(os.ComplianceYear = @ComplianceYear 
			OR @ComplianceYear IS NULL)
	AND		
		os.ComplianceYear < YEAR(GETDATE()) - 7
	AND
		(s.SchemeName = @SchemeName 
			OR @SchemeName IS NULL)

	UNION

	SELECT
		s.SchemeName,
		s.Id AS SchemeId,
		s.ApprovalNumber,
		dru.ComplianceYear
	FROM
		[PCS].[Scheme] s
	INNER JOIN
		[PCS].[DataReturnUpload] dru
			ON dru.SchemeId = s.Id
	WHERE
		(dru.ComplianceYear = @ComplianceYear 
			OR @ComplianceYear IS NULL)
	AND		
		dru.ComplianceYear < YEAR(GETDATE()) - 7
	AND
		(s.SchemeName = @SchemeName 
			OR @SchemeName IS NULL)

	UNION

	SELECT
		s.SchemeName,
		s.Id AS SchemeId,
		s.ApprovalNumber,
		dr.ComplianceYear
	FROM
		[PCS].[Scheme] s
	INNER JOIN
		[PCS].[DataReturn] dr
			ON dr.SchemeId = s.Id
	WHERE
		(dr.ComplianceYear = @ComplianceYear 
			OR @ComplianceYear IS NULL)
	AND		
		dr.ComplianceYear < YEAR(GETDATE()) - 7
	AND
		(s.SchemeName = @SchemeName 
			OR @SchemeName IS NULL)

	UNION

	SELECT
		s.SchemeName,
		s.Id AS SchemeId,
		s.ApprovalNumber,
		rp.ComplianceYear
	FROM
		[PCS].[Scheme] s
	INNER JOIN
		[Producer].[RegisteredProducer] rp
			ON rp.SchemeId = s.Id
	WHERE
		(rp.ComplianceYear = @ComplianceYear 
			OR @ComplianceYear IS NULL)
	AND		
		rp.ComplianceYear < YEAR(GETDATE()) - 7
	AND
		(s.SchemeName = @SchemeName 
			OR @SchemeName IS NULL);

	DELETE
		#TempSchemeData
	WHERE
		ComplianceYear IS NULL;

	SELECT
		*
	FROM
		#TempSchemeData
	ORDER BY
		SchemeName,
		ComplianceYear;
END