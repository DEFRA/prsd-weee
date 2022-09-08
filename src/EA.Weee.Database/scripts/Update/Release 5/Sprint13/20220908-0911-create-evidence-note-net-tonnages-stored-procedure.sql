IF OBJECT_ID('[Evidence].[getEvidenceNotesNetTonnage]', 'P') IS NOT NULL
	DROP PROC [Evidence].getEvidenceNotesNetTonnage
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Evidence].getEvidenceNotesNetTonnage
	@ComplianceYear SMALLINT,
	@OriginatingOrganisationId UNIQUEIDENTIFIER = NULL,
	@RecipientOrganisationId UNIQUEIDENTIFIER = NULL
AS
BEGIN
SET NOCOUNT ON;

SELECT
	*
FROM 
	[Evidence].[vwEvidenceSummary] es 
	INNER JOIN [Evidence].vwEvidenceByCategoryNetOfTransfer ec ON es.Id = ec.Id
WHERE
	es.NoteType = 1 AND
	(es.ComplianceYear = @ComplianceYear) AND
		(
			(@OriginatingOrganisationId IS NULL OR es.OriginatingOrganisationId = @OriginatingOrganisationId) AND
			(@RecipientOrganisationId IS NULL OR es.RecipientOrganisationId = @RecipientOrganisationId)
		)
ORDER BY
	es.Reference ASC

END
