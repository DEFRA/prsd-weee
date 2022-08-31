IF OBJECT_ID('[Evidence].[getEvidenceByCategoryNetOfTransfer]', 'P') IS NOT NULL
	DROP PROC [Evidence].[getEvidenceByCategoryNetOfTransfer]
GO

CREATE PROCEDURE [Evidence].[getEvidenceByCategoryNetOfTransfer]
	@Originator BIT,
	@ComplianceYear SMALLINT,
	@OrganisationId UNIQUEIDENTIFIER
AS
BEGIN
SET NOCOUNT ON;

SELECT
	*
FROM
	[Evidence].[vwEvidenceSummary] summary WITH (NOLOCK)
	INNER JOIN [Evidence].[vwEvidenceByCategoryNetOfTransfer] ec WITH (NOLOCK) ON ec.Id = summary.Id
WHERE
	(summary.ComplianceYear = @ComplianceYear OR @ComplianceYear IS NULL) AND
	(
		(@Originator IS NULL)
		OR (@Originator = 1 AND summary.OriginatingOrganisationId = @OrganisationId)
		OR (@Originator = 0 AND summary.RecipientOrganisationId = @OrganisationId)
	)
ORDER BY
	summary.Reference DESC
END
GO


