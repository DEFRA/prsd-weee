/****** Object:  StoredProcedure [AATF].[DeleteAatfOrAeRecord]    Script Date: 30/10/2026 09:35:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER   PROCEDURE [AATF].[DeleteAatfOrAeRecord]
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
		DECLARE @OrganisationId UNIQUEIDENTIFIER;
		DECLARE @ComplianceYear INT;
		DECLARE @FacilityType INT;

		SELECT
			@ContactId = ContactId,
			@SiteAddressId = SiteAddressId,
			@OrganisationId = OrganisationId,
			@ComplianceYear = ComplianceYear,
			@FacilityType = FacilityType
		FROM [AATF].[AATF]
		WHERE Id = @AatfId;

		------------------------------------------------------------
		-- Capture Return IDs
		-- Return.Id is INT
		------------------------------------------------------------
		DECLARE @ReturnIds TABLE
		(
			Id UNIQUEIDENTIFIER PRIMARY KEY
		);

		INSERT INTO @ReturnIds (Id)
		SELECT ReturnId
		FROM [AATF].[ReturnAatf]
		WHERE AatfId = @AatfId

		UNION

		SELECT R.Id
		FROM [AATF].[Return] R
		WHERE R.OrganisationId = @OrganisationId
		  AND R.ComplianceYear = @ComplianceYear
		  AND R.FacilityType = @FacilityType
		  AND NOT EXISTS
		  (
			  SELECT 1
			  FROM [AATF].[ReturnAatf] RA
			  WHERE RA.ReturnId = R.Id
				AND RA.AatfId = @AatfId
		  );
			
		------------------------------------------------------------
		-- 1 Delete NoteStatusHistory
		------------------------------------------------------------
		DECLARE @NoteStatusHistoryIds TABLE
		(
			Id UNIQUEIDENTIFIER PRIMARY KEY
		);

		-- Step 1: Capture IDs once
		INSERT INTO @NoteStatusHistoryIds (Id)
			SELECT NSH.Id
			FROM [Evidence].[NoteStatusHistory] NSH
			INNER JOIN [Evidence].[Note] N ON N.Id = NSH.NoteId
			WHERE N.AatfId = @AatfId;

		-- Step 2: Delete audit log records
		DELETE AL
		FROM [Auditing].[AuditLog] AL
		INNER JOIN @NoteStatusHistoryIds T ON T.Id = AL.RecordId
		WHERE AL.TableName = '[Evidence].[NoteStatusHistory]';

		-- Step 3: Delete note status history records
		DELETE NSH
		FROM [Evidence].[NoteStatusHistory] NSH
		INNER JOIN @NoteStatusHistoryIds T ON T.Id = NSH.Id;

		------------------------------------------------------------
		-- 2 Delete NoteTransferTonnage
		------------------------------------------------------------
		DECLARE @NoteTransferTonnageIds TABLE
		(
			Id UNIQUEIDENTIFIER PRIMARY KEY
		);

		-- Step 1: Capture IDs once
		INSERT INTO @NoteTransferTonnageIds (Id)
			SELECT NTT.Id
			FROM [Evidence].[NoteTransferTonnage] NTT
			INNER JOIN [Evidence].[Note] N ON NTT.TransferNoteId = N.Id
			WHERE N.AatfId = @AatfId;

		-- Step 2: Delete audit log records
		DELETE AL
		FROM [Auditing].[AuditLog] AL
		INNER JOIN @NoteTransferTonnageIds T ON T.Id = AL.RecordId
		WHERE AL.TableName = '[Evidence].[NoteTransferTonnage]';

		-- Step 3: Delete Note Transfer Tonnage records
		DELETE NTT
		FROM [Evidence].[NoteTransferTonnage] NTT
		INNER JOIN @NoteTransferTonnageIds T ON T.Id = NTT.Id;

		------------------------------------------------------------
		-- 3 Delete NoteTonnage
		------------------------------------------------------------
		DECLARE @NoteTonnageIds TABLE
		(
			Id UNIQUEIDENTIFIER PRIMARY KEY
		);

		-- Step 1: Capture IDs once
		INSERT INTO @NoteTonnageIds (Id)
			SELECT NT.Id FROM [Evidence].[NoteTonnage] NT
			INNER JOIN [Evidence].[Note] N ON NT.NoteId = N.Id
			WHERE N.AatfId = @AatfId;

		-- Step 2: Delete audit log records
		DELETE AL
		FROM [Auditing].[AuditLog] AL
		INNER JOIN @NoteTonnageIds T ON T.Id = AL.RecordId
		WHERE AL.TableName = '[Evidence].[NoteTonnage]';

		-- Step 3: Delete Note Tonnage records
		DELETE NT
		FROM [Evidence].[NoteTonnage] NT
		INNER JOIN @NoteTonnageIds T ON T.Id = NT.Id;

		------------------------------------------------------------
		-- 4 Delete Note
		------------------------------------------------------------
		DECLARE @NoteIds TABLE
		(
			Id UNIQUEIDENTIFIER PRIMARY KEY
		);

		-- Step 1: Capture IDs once
		INSERT INTO @NoteIds (Id)
			SELECT Id FROM [Evidence].[Note]
			WHERE AatfId = @AatfId;

		-- Step 2: Delete audit log records
		DELETE AL
		FROM [Auditing].[AuditLog] AL
		INNER JOIN @NoteIds T ON T.Id = AL.RecordId
		WHERE AL.TableName = '[Evidence].[Note]';

		-- Step 3: Delete Note records
		DELETE FROM [Evidence].[Note]
		WHERE AatfId = @AatfId;

		------------------------------------------------------------
		-- 5 Delete WeeeSentOnAmount
		------------------------------------------------------------
		DECLARE @WeeeSentOnAmountIds TABLE
		(
			Id UNIQUEIDENTIFIER PRIMARY KEY
		);

		-- Step 1: Capture IDs once
		INSERT INTO @WeeeSentOnAmountIds (Id)
			SELECT WSA.Id FROM [AATF].[WeeeSentOnAmount] WSA
			INNER JOIN [AATF].[WeeeSentOn] WSO ON WSA.WeeeSentOnId = WSO.Id
			WHERE WSO.AatfId = @AatfId;

		-- Step 2: Delete audit log records
		DELETE AL
		FROM [Auditing].[AuditLog] AL
		INNER JOIN @WeeeSentOnAmountIds T ON T.Id = AL.RecordId
		WHERE AL.TableName = '[AATF].[WeeeSentOnAmount]';

		-- Step 3: Delete Note records
		DELETE WSA
		FROM [AATF].[WeeeSentOnAmount] WSA
		INNER JOIN @WeeeSentOnAmountIds T ON T.Id = WSA.Id;

		------------------------------------------------------------
		-- 6 Delete WeeeSentOn
		------------------------------------------------------------
		DECLARE @WeeeSentOnIds TABLE
		(
			Id UNIQUEIDENTIFIER PRIMARY KEY
		);

		-- Step 1: Capture IDs once
		INSERT INTO @WeeeSentOnIds (Id)
			SELECT Id FROM [AATF].[WeeeSentOn]
			WHERE AatfId = @AatfId;

		-- Step 2: Delete audit log records
		DELETE AL
		FROM [Auditing].[AuditLog] AL
		INNER JOIN @WeeeSentOnIds T ON T.Id = AL.RecordId
		WHERE AL.TableName = '[AATF].[WeeeSentOn]';

		-- Step 3: Delete Note records
		DELETE FROM [AATF].[WeeeSentOn]
		WHERE AatfId = @AatfId;

		------------------------------------------------------------
		-- 7 Delete WeeeReusedAmount
		------------------------------------------------------------
		DECLARE @WeeeReusedAmountIds TABLE
		(
			Id UNIQUEIDENTIFIER PRIMARY KEY
		);

		-- Step 1: Capture IDs once
		INSERT INTO @WeeeReusedAmountIds (Id)
			SELECT WRA.Id
			FROM [AATF].[WeeeReusedAmount] WRA
			INNER JOIN [AATF].[WeeeReused] WR ON WRA.WeeeReusedId = WR.Id
			WHERE WR.AatfId = @AatfId;

		-- Step 2: Delete audit log records
		DELETE AL
		FROM [Auditing].[AuditLog] AL
		INNER JOIN @WeeeReusedAmountIds T ON T.Id = AL.RecordId
		WHERE AL.TableName = '[AATF].[WeeeReusedAmount]';

		-- Step 3: Delete WeeeReusedAmount records
		DELETE WRA
		FROM [AATF].[WeeeReusedAmount] WRA
		INNER JOIN @WeeeReusedAmountIds T ON T.Id = WRA.Id;

		------------------------------------------------------------
		-- 8 Delete WeeeReusedSite
		------------------------------------------------------------
		DECLARE @WeeeReusedSiteIds TABLE
		(
			Id UNIQUEIDENTIFIER PRIMARY KEY
		);

		-- Step 1: Capture IDs once
		INSERT INTO @WeeeReusedSiteIds (Id)
			SELECT WRS.Id
			FROM [AATF].[WeeeReusedSite] WRS
			INNER JOIN [AATF].[WeeeReused] WR ON WRS.WeeeReusedId = WR.Id
			WHERE WR.AatfId = @AatfId;

		-- Step 2: Delete audit log records
		DELETE AL
		FROM [Auditing].[AuditLog] AL
		INNER JOIN @WeeeReusedSiteIds T ON T.Id = AL.RecordId
		WHERE AL.TableName = '[AATF].[WeeeReusedSite]';

		-- Step 3: Delete WeeeReusedSite records
		DELETE WRS
		FROM [AATF].[WeeeReusedSite] WRS
		INNER JOIN @WeeeReusedSiteIds T ON T.Id = WRS.Id;

		------------------------------------------------------------
		-- 9 Delete WeeeReused
		------------------------------------------------------------
		DECLARE @WeeeReusedIds TABLE
		(
			Id UNIQUEIDENTIFIER PRIMARY KEY
		);

		-- Step 1: Capture IDs once
		INSERT INTO @WeeeReusedIds (Id)
			SELECT Id
			FROM [AATF].[WeeeReused]
			WHERE AatfId = @AatfId;

		-- Step 2: Delete audit log records
		DELETE AL
		FROM [Auditing].[AuditLog] AL
		INNER JOIN @WeeeReusedIds T ON T.Id = AL.RecordId
		WHERE AL.TableName = '[AATF].[WeeeReused]';

		-- Step 3: Delete WeeeReused records
		DELETE WR
		FROM [AATF].[WeeeReused] WR
		INNER JOIN @WeeeReusedIds T ON T.Id = WR.Id;

		------------------------------------------------------------
		-- 10 Delete WeeeReceivedAmount
		------------------------------------------------------------
		DECLARE @WeeeReceivedAmountIds TABLE
		(
			Id UNIQUEIDENTIFIER PRIMARY KEY
		);

		-- Step 1: Capture IDs once
		INSERT INTO @WeeeReceivedAmountIds (Id)
			SELECT WRA.Id
			FROM [AATF].[WeeeReceivedAmount] WRA
			INNER JOIN [AATF].[WeeeReceived] WR ON WRA.WeeeReceivedId = WR.Id
			WHERE WR.AatfId = @AatfId;

		-- Step 2: Delete audit log records
		DELETE AL
		FROM [Auditing].[AuditLog] AL
		INNER JOIN @WeeeReceivedAmountIds T ON T.Id = AL.RecordId
		WHERE AL.TableName = '[AATF].[WeeeReceivedAmount]';

		-- Step 3: Delete WeeeReceivedAmount records
		DELETE WRA
		FROM [AATF].[WeeeReceivedAmount] WRA
		INNER JOIN @WeeeReceivedAmountIds T ON T.Id = WRA.Id;

		------------------------------------------------------------
		-- 11 Delete WeeeReceived
		------------------------------------------------------------
		DECLARE @WeeeReceivedIds TABLE
		(
			Id UNIQUEIDENTIFIER PRIMARY KEY
		);

		-- Step 1: Capture IDs once
		INSERT INTO @WeeeReceivedIds (Id)
			SELECT Id
			FROM [AATF].[WeeeReceived]
			WHERE AatfId = @AatfId;

		-- Step 2: Delete audit log records
		DELETE AL
		FROM [Auditing].[AuditLog] AL
		INNER JOIN @WeeeReceivedIds T ON T.Id = AL.RecordId
		WHERE AL.TableName = '[AATF].[WeeeReceived]';

		-- Step 3: Delete WeeeReceived records
		DELETE WR
		FROM [AATF].[WeeeReceived] WR
		INNER JOIN @WeeeReceivedIds T ON T.Id = WR.Id;

		------------------------------------------------------------
		-- 12 Delete ReturnReportOn
		-- ReturnReportOn.ReturnId -> Return.Id
		-- ReturnReportOn.ReportOnQuestionId -> ReportOnQuestion.Id
		------------------------------------------------------------
		DECLARE @ReturnReportOnIds TABLE
		(
			Id UNIQUEIDENTIFIER PRIMARY KEY
		);

		-- Step 1: Capture IDs once
		INSERT INTO @ReturnReportOnIds (Id)
			SELECT RRO.Id
			FROM [AATF].[ReturnReportOn] RRO
			INNER JOIN @ReturnIds RI ON RRO.ReturnId = RI.Id;

		-- Step 2: Delete audit log records
		DELETE AL
		FROM [Auditing].[AuditLog] AL
		INNER JOIN @ReturnReportOnIds T ON T.Id = AL.RecordId
		WHERE AL.TableName = '[AATF].[ReturnReportOn]';

		-- Step 3: Delete ReturnReportOn records
		DELETE RRO
		FROM [AATF].[ReturnReportOn] RRO
		INNER JOIN @ReturnReportOnIds T ON T.Id = RRO.Id;

		------------------------------------------------------------
		-- 13 Delete NonObligatedWeee
		-- Only delete if Return will also be deleted.
		------------------------------------------------------------
		DECLARE @NonObligatedWeeeIds TABLE
		(
			Id UNIQUEIDENTIFIER PRIMARY KEY
		);

		-- Step 1: Capture IDs eligible for deletion
		INSERT INTO @NonObligatedWeeeIds (Id)
			SELECT [NOW].Id
			FROM [AATF].[NonObligatedWeee] [NOW]
			INNER JOIN @ReturnIds RI ON [NOW].ReturnId = RI.Id;

		-- Step 2: Delete audit log records
		DELETE AL
		FROM [Auditing].[AuditLog] AL
		INNER JOIN @NonObligatedWeeeIds T ON T.Id = AL.RecordId
		WHERE AL.TableName = '[AATF].[NonObligatedWeee]';

		-- Step 3: Delete NonObligatedWeee records
		DELETE [NOW]
		FROM [AATF].[NonObligatedWeee] [NOW]
		INNER JOIN @NonObligatedWeeeIds T ON T.Id = [NOW].Id;

		------------------------------------------------------------
		-- 14 Delete ReturnScheme
		-- Only delete if Return will also be deleted.
		------------------------------------------------------------
		DECLARE @ReturnSchemeIds TABLE
		(
			Id UNIQUEIDENTIFIER PRIMARY KEY
		);

		-- Step 1: Capture IDs eligible for deletion
		INSERT INTO @ReturnSchemeIds (Id)
			SELECT RS.Id
			FROM [AATF].[ReturnScheme] RS
			INNER JOIN @ReturnIds RI ON RS.ReturnId = RI.Id;

		-- Step 2: Delete audit log records
		DELETE AL
		FROM [Auditing].[AuditLog] AL
		INNER JOIN @ReturnSchemeIds T ON T.Id = AL.RecordId
		WHERE AL.TableName = '[AATF].[ReturnScheme]';

		-- Step 3: Delete ReturnScheme records
		DELETE RS
		FROM [AATF].[ReturnScheme] RS
		INNER JOIN @ReturnSchemeIds T ON T.Id = RS.Id;

		------------------------------------------------------------
		-- 15 Delete ReturnAatf relationship
		------------------------------------------------------------
		DECLARE @ReturnAatfIds TABLE
		(
			Id UNIQUEIDENTIFIER PRIMARY KEY
		);

		-- Step 1: Capture IDs once
		INSERT INTO @ReturnAatfIds (Id)
			SELECT RA.Id
			FROM [AATF].[ReturnAatf] RA
			WHERE RA.AatfId = @AatfId;

		-- Step 2: Delete audit log records
		DELETE AL
		FROM [Auditing].[AuditLog] AL
		INNER JOIN @ReturnAatfIds T ON T.Id = AL.RecordId
		WHERE AL.TableName = '[AATF].[ReturnAatf]';

		-- Step 3: Delete ReturnAatf records
		DELETE RA
		FROM [AATF].[ReturnAatf] RA
		INNER JOIN @ReturnAatfIds T ON T.Id = RA.Id;

		------------------------------------------------------------
		-- 16 Delete Return
		-- Only delete Return if:
		--   1. No other AATF references it
		--   2. No other Return references it as ParentId
		------------------------------------------------------------
		DECLARE @ReturnDeleteIds TABLE
		(
			Id UNIQUEIDENTIFIER PRIMARY KEY
		);

		-- Step 1: Capture IDs eligible for deletion
		INSERT INTO @ReturnDeleteIds (Id)
			SELECT R.Id
			FROM [AATF].[Return] R
			INNER JOIN @ReturnIds RI ON R.Id = RI.Id;

		-- Step 2: Delete audit log records
		DELETE AL
		FROM [Auditing].[AuditLog] AL
		INNER JOIN @ReturnDeleteIds T ON T.Id = AL.RecordId
		WHERE AL.TableName = '[AATF].[Return]';

		-- Step 3: Delete Return records
		DELETE 
		FROM [AATF].[Return] 
		WHERE OrganisationId = @OrganisationId AND 
			  ComplianceYear = @ComplianceYear AND
			  FacilityType = @FacilityType;

		------------------------------------------------------------
		-- 17 Delete AATF
		------------------------------------------------------------
		DECLARE @AatfIds TABLE
		(
			Id UNIQUEIDENTIFIER PRIMARY KEY
		);

		-- Step 1: Capture ID
		INSERT INTO @AatfIds (Id)
			SELECT Id
			FROM [AATF].[AATF]
			WHERE Id = @AatfId;

		-- Step 2: Delete audit log records
		DELETE AL
		FROM [Auditing].[AuditLog] AL
		INNER JOIN @AatfIds T ON T.Id = AL.RecordId
		WHERE AL.TableName = '[AATF].[AATF]';

		-- Step 3: Delete AATF record
		DELETE A
		FROM [AATF].[AATF] A
		INNER JOIN @AatfIds T ON T.Id = A.Id;

		------------------------------------------------------------
		-- 18 Delete Contact
		------------------------------------------------------------
		IF @ContactId IS NOT NULL
		BEGIN
			DECLARE @ContactIds TABLE
			(
				Id UNIQUEIDENTIFIER PRIMARY KEY
			);

			-- Step 1: Capture ID
			INSERT INTO @ContactIds (Id)
				SELECT Id
				FROM [AATF].[Contact]
				WHERE Id = @ContactId;

			-- Step 2: Delete audit log records
			DELETE AL
			FROM [Auditing].[AuditLog] AL
			INNER JOIN @ContactIds T ON T.Id = AL.RecordId
			WHERE AL.TableName = '[AATF].[Contact]';

			-- Step 3: Delete Contact record
			DELETE C
			FROM [AATF].[Contact] C
			INNER JOIN @ContactIds T ON T.Id = C.Id;
		END;

		------------------------------------------------------------
		-- 19 Delete Address
		------------------------------------------------------------
		IF @SiteAddressId IS NOT NULL
		BEGIN
			DECLARE @AddressIds TABLE
			(
				Id UNIQUEIDENTIFIER PRIMARY KEY
			);

			-- Step 1: Capture ID
			INSERT INTO @AddressIds (Id)
				SELECT Id
				FROM [AATF].[Address]
				WHERE Id = @SiteAddressId;

			-- Step 2: Delete audit log records
			DELETE AL
			FROM [Auditing].[AuditLog] AL
			INNER JOIN @AddressIds T ON T.Id = AL.RecordId
			WHERE AL.TableName = '[AATF].[Address]';

			-- Step 3: Delete Address record
			DELETE A
			FROM [AATF].[Address] A
			INNER JOIN @AddressIds T ON T.Id = A.Id;
		END;

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
