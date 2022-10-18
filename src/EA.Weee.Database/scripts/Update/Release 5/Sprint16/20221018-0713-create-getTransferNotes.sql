IF OBJECT_ID('[Evidence].[getTransferNotes]', 'P') IS NOT NULL
	DROP PROC [Evidence].getTransferNotes
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Evidence].[getTransferNotes]
	@ComplianceYear SMALLINT,
	@OrganisationId UNIQUEIDENTIFIER = NULL
WITH RECOMPILE
AS
BEGIN
SET NOCOUNT ON;
	SELECT
		ts.Reference AS TransferReference,
		ts.Status AS TransferStatus,
		ts.TransferApprovedDateTime AS TransferApprovalDate,
		ts.TransferredSchemeName AS TransferredByName,
		ts.TransferredApprovalNumber AS TransferredByApprovalNumber,
		ts.RecipientSchemeName AS RecipientName,
		ts.RecipientApprovalNumber AS RecipientApprovalNumber,
		es.Reference AS EvidenceNoteReference,
		es.ApprovedDateTime AS EvidenceNoteApprovalDate,
		es.AatfName AS AatfIssuedByName,
		es.AatfApprovalNumber AS AatfIssuedByApprovalNumber,
		es.Protocol AS Protocol,
		tts.Cat1Received,
		tts.Cat2Received,
		tts.Cat3Received,
		tts.Cat4Received,
		tts.Cat5Received,
		tts.Cat6Received,
		tts.Cat7Received,
		tts.Cat8Received,
		tts.Cat9Received,
		tts.Cat10Received,
		tts.Cat11Received,
		tts.Cat12Received,
		tts.Cat13Received,
		tts.Cat14Received,
		tts.TotalReceived,
		tts.Cat1Reused,
		tts.Cat2Reused,
		tts.Cat3Reused,
		tts.Cat4Reused,
		tts.Cat5Reused,
		tts.Cat6Reused,
		tts.Cat7Reused,
		tts.Cat8Reused,
		tts.Cat9Reused,
		tts.Cat10Reused,
		tts.Cat11Reused,
		tts.Cat12Reused,
		tts.Cat13Reused,
		tts.Cat14Reused,
		tts.TotalReused
	FROM 
		Evidence.vwTransferTonnageSummary tts 
		INNER JOIN Evidence.vwTransferSummary ts ON ts.Id = tts.TransferNoteId
		INNER JOIN Evidence.vwEvidenceSummary es ON es.Id = tts.OriginalNoteId AND es.StatusId = 3
	WHERE
		ts.ComplianceYear = @ComplianceYear
		AND (ts.RecipientOrganisationId = @OrganisationId OR ts.OriginatingOrganisationId = @OrganisationId OR @OrganisationId IS NULL)
	ORDER BY
		ts.ReferenceId ASC

END