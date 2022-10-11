SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [Evidence].[getEvidenceNotesNetTonnage]
	@ComplianceYear SMALLINT,
	@OriginatingOrganisationId UNIQUEIDENTIFIER = NULL,
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
	INNER JOIN [Evidence].vwEvidenceByCategoryNetOfTransfer ec WITH (NOLOCK) ON es.Id = ec.Id
WHERE
	es.NoteType = 1 AND
	(es.ComplianceYear = @ComplianceYear) AND
		(
			(@OriginatingOrganisationId IS NULL OR es.OriginatingOrganisationId = @OriginatingOrganisationId) AND
			(@RecipientOrganisationId IS NULL OR es.RecipientOrganisationId = @RecipientOrganisationId) AND
			(@AatfId IS NULL OR es.AatfId = @AatfId)
		) AND
	((NOT @RecipientOrganisationId IS NULL AND es.StatusId  <> 1) OR @RecipientOrganisationId IS NULL)
ORDER BY
	es.ReferenceId ASC

END