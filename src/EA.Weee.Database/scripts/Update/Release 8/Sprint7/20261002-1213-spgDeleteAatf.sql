/****** Object:  StoredProcedure [AATF].[DeleteAatf]    Script Date: 02/10/2026 11:11:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [AATF].[DeleteAatf]
	@AatfId UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRANSACTION;

		------------------------------------------------------------
		-- Capture Contact and Address IDs
		------------------------------------------------------------
		DECLARE @ContactId UNIQUEIDENTIFIER;
		DECLARE @SiteAddressId UNIQUEIDENTIFIER;

		SELECT
			@ContactId = ContactId,
			@SiteAddressId = SiteAddressId
		FROM [AATF].[AATF]
		WHERE Id = @AatfId;

		------------------------------------------------------------
		-- Capture Return IDs
		-- Return.Id is INT
		------------------------------------------------------------
		DECLARE @ReturnIds TABLE
		(
			ReturnId UNIQUEIDENTIFIER PRIMARY KEY
		);

		INSERT INTO @ReturnIds (ReturnId)
		SELECT DISTINCT ReturnId
		FROM [AATF].[ReturnAatf]
		WHERE AatfId = @AatfId;

		------------------------------------------------------------
		-- Capture ReportOnQuestion IDs
		-- ReportOnQuestion.Id is INT
		------------------------------------------------------------
		DECLARE @ReportOnQuestionIds TABLE
		(
			ReportOnQuestionId INT PRIMARY KEY
		);

		INSERT INTO @ReportOnQuestionIds (ReportOnQuestionId)
		SELECT DISTINCT ReportOnQuestionId
		FROM [AATF].[ReturnReportOn]
		WHERE ReturnId IN
		(
			SELECT ReturnId
			FROM @ReturnIds
		)
		AND ReportOnQuestionId IS NOT NULL;

		------------------------------------------------------------
		-- 1. Delete NoteStatusHistory
		------------------------------------------------------------
		DELETE NSH
		FROM [Evidence].[NoteStatusHistory] NSH
		INNER JOIN [Evidence].[Note] N ON NSH.NoteId = N.Id
		WHERE N.AatfId = @AatfId;

		------------------------------------------------------------
		-- 2. Delete NoteTransferTonnage
		------------------------------------------------------------
		DELETE NTT
		FROM [Evidence].[NoteTransferTonnage] NTT
		INNER JOIN [Evidence].[Note] N ON NTT.TransferNoteId = N.Id
		WHERE N.AatfId = @AatfId;

		------------------------------------------------------------
		-- 3. Delete NoteTonnage
		------------------------------------------------------------
		DELETE NT
		FROM [Evidence].[NoteTonnage] NT
		INNER JOIN [Evidence].[Note] N ON NT.NoteId = N.Id
		WHERE N.AatfId = @AatfId;

		------------------------------------------------------------
		-- 4. Delete Note
		------------------------------------------------------------
		DELETE FROM [Evidence].[Note]
		WHERE AatfId = @AatfId;

		------------------------------------------------------------
		-- 5. Delete WeeeSentOnAmount
		------------------------------------------------------------
		DELETE WSA
		FROM [AATF].[WeeeSentOnAmount] WSA
		INNER JOIN [AATF].[WeeeSentOn] WSO ON WSA.WeeeSentOnId = WSO.Id
		WHERE WSO.AatfId = @AatfId;

		------------------------------------------------------------
		-- 6. Delete WeeeSentOn
		------------------------------------------------------------
		DELETE FROM [AATF].[WeeeSentOn]
		WHERE AatfId = @AatfId;

		------------------------------------------------------------
		-- 7. Delete WeeeReusedAmount
		------------------------------------------------------------
		DELETE WRA
		FROM [AATF].[WeeeReusedAmount] WRA
		INNER JOIN [AATF].[WeeeReused] WR ON WRA.WeeeReusedId = WR.Id
		WHERE WR.AatfId = @AatfId;

		------------------------------------------------------------
		-- 8. Delete WeeeReusedSite
		------------------------------------------------------------
		DELETE WRS
		FROM [AATF].[WeeeReusedSite] WRS
		INNER JOIN [AATF].[WeeeReused] WR ON WRS.WeeeReusedId = WR.Id
		WHERE WR.AatfId = @AatfId;

		------------------------------------------------------------
		-- 9. Delete WeeeReused
		------------------------------------------------------------
		DELETE FROM [AATF].[WeeeReused]
		WHERE AatfId = @AatfId;

		------------------------------------------------------------
		-- 10. Delete WeeeReceivedAmount
		------------------------------------------------------------
		DELETE WRA
		FROM [AATF].[WeeeReceivedAmount] WRA
		INNER JOIN [AATF].[WeeeReceived] WR ON WRA.WeeeReceivedId = WR.Id
		WHERE WR.AatfId = @AatfId;

		------------------------------------------------------------
		-- 11. Delete WeeeReceived
		------------------------------------------------------------
		DELETE FROM [AATF].[WeeeReceived]
		WHERE AatfId = @AatfId;

		------------------------------------------------------------
		-- 12. Delete ReturnReportOn
		-- ReturnReportOn.ReturnId -> Return.Id
		-- ReturnReportOn.ReportOnQuestionId -> ReportOnQuestion.Id
		------------------------------------------------------------
		DELETE RRO
		FROM [AATF].[ReturnReportOn] RRO
		INNER JOIN @ReturnIds RI ON RRO.ReturnId = RI.ReturnId;

		------------------------------------------------------------
		-- 13. Delete ReportOnQuestion
		-- Only delete if no other ReturnReportOn references it.
		------------------------------------------------------------
		DELETE ROQ
		FROM [AATF].[ReportOnQuestion] ROQ
		INNER JOIN @ReportOnQuestionIds RQI ON ROQ.Id = RQI.ReportOnQuestionId
		WHERE NOT EXISTS
		(
			SELECT 1
			FROM [AATF].[ReturnReportOn] RRO
			WHERE RRO.ReportOnQuestionId = ROQ.Id
		);

		------------------------------------------------------------
		-- 14. Delete NonObligatedWeee
		-- Only delete if Return will also be deleted.
		------------------------------------------------------------
		DELETE NOW
		FROM [AATF].[NonObligatedWeee] NOW
		INNER JOIN @ReturnIds RI ON NOW.ReturnId = RI.ReturnId
		WHERE NOT EXISTS
		(
			SELECT 1
			FROM [AATF].[ReturnAatf] RA
			WHERE RA.ReturnId = RI.ReturnId AND RA.AatfId <> @AatfId
		);

		------------------------------------------------------------
		-- 15. Delete ReturnScheme
		-- Only delete if Return will also be deleted.
		------------------------------------------------------------
		DELETE RS
		FROM [AATF].[ReturnScheme] RS
		INNER JOIN @ReturnIds RI ON RS.ReturnId = RI.ReturnId
		WHERE NOT EXISTS
		(
			SELECT 1
			FROM [AATF].[ReturnAatf] RA
			WHERE RA.ReturnId = RI.ReturnId AND RA.AatfId <> @AatfId
		);

		------------------------------------------------------------
		-- 16. Delete ReturnAatf relationship
		------------------------------------------------------------
		DELETE RA
		FROM [AATF].[ReturnAatf] RA
		WHERE RA.AatfId = @AatfId;

		------------------------------------------------------------
		-- 17. Delete Return
		-- Only delete Return if no other AATF references it.
		------------------------------------------------------------
		DELETE R
		FROM [AATF].[Return] R
		INNER JOIN @ReturnIds RI ON R.Id = RI.ReturnId
		WHERE NOT EXISTS
		(
			SELECT 1
			FROM [AATF].[ReturnAatf] RA
			WHERE RA.ReturnId = R.Id
		);

		------------------------------------------------------------
		-- 18. Delete AATF
		------------------------------------------------------------
		DELETE FROM [AATF].[AATF]
		WHERE Id = @AatfId;

		------------------------------------------------------------
		-- 19. Delete Contact
		------------------------------------------------------------
		IF @ContactId IS NOT NULL
		BEGIN
			DELETE FROM [AATF].[Contact]
			WHERE Id = @ContactId;
		END;

		------------------------------------------------------------
		-- 20. Delete Address
		------------------------------------------------------------
		IF @SiteAddressId IS NOT NULL
		BEGIN
			DELETE FROM [AATF].[Address]
			WHERE Id = @SiteAddressId;
		END;

		------------------------------------------------------------
		-- Commit
		------------------------------------------------------------
		COMMIT TRANSACTION;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END;

		RETURN -1;

	END CATCH;
	RETURN 0;
END;
