SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Create date: 2026 Aug 19
-- Modified date: 2026 Aug 19
-- Description:	This stored procedure aggregates Compliance Years from 
--				multiple sources which exceded the retention period of 7 years.
-- =============================================
CREATE PROCEDURE [PCS].[spgSchemeComplianceYearsExceedingRetentionPeriod]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT	
		mu.ComplianceYear
	FROM	
		[PCS].[MemberUpload] mu
	WHERE	
		mu.ComplianceYear IS NOT NULL
	AND		
		mu.ComplianceYear < YEAR(GETDATE()) - 7

	UNION

	SELECT	
		os.ComplianceYear
	FROM	
		[PCS].[ObligationScheme] os
	WHERE	
		os.ComplianceYear IS NOT NULL
	AND		
		os.ComplianceYear < YEAR(GETDATE()) - 7

	UNION

	SELECT	
		dru.ComplianceYear
	FROM	
		[PCS].[DataReturnUpload] dru
	WHERE	
	dru.ComplianceYear IS NOT NULL
	AND	
		dru.ComplianceYear < YEAR(GETDATE()) - 7

	UNION

	SELECT	
		dr.ComplianceYear
	FROM	
		[PCS].[DataReturn] dr
	WHERE	
		dr.ComplianceYear IS NOT NULL
	AND		
		dr.ComplianceYear < YEAR(GETDATE()) - 7

	UNION

	SELECT	
		rp.ComplianceYear
	FROM	
		[Producer].[RegisteredProducer] rp
	WHERE	
		rp.ComplianceYear IS NOT NULL
	AND		
		rp.ComplianceYear < YEAR(GETDATE()) - 7

END
GO