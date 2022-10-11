SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [Evidence].[getEvidenceNotesOriginalTonnage]
	@ComplianceYear SMALLINT,
	@RecipientOrganisationId UNIQUEIDENTIFIER = NULL,
	@AatfId UNIQUEIDENTIFIER = NULL
WITH RECOMPILE
AS
BEGIN
SET NOCOUNT ON;

IF @AatfId IS NOT NULL BEGIN
	SELECT
		@AatfId = a2.Id 
	FROM
		[AATF].[Aatf] a1 
		INNER JOIN [AATF].AATF a2 ON a1.AatfId = a2.AatfId AND a2.ComplianceYear = @ComplianceYear
	WHERE
		a1.Id = @AatfId
END

SELECT
	*
FROM 
	[Evidence].[vwEvidenceSummary] es 
	INNER JOIN [Evidence].vwEvidenceByCategory ec WITH (NOLOCK) ON es.Id = ec.Id
WHERE
	es.NoteType = 1 AND
	(es.ComplianceYear = @ComplianceYear) AND
		(
			(@RecipientOrganisationId IS NULL OR es.RecipientOrganisationId = @RecipientOrganisationId) AND
			(@AatfId IS NULL OR es.AatfId = @AatfId)
	) AND
	((NOT @RecipientOrganisationId IS NULL AND es.StatusId  <> 1) OR @RecipientOrganisationId IS NULL)
ORDER BY
	es.ReferenceId ASC

END