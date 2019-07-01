﻿SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [AATF].getAatfSubmissions
	@AatfId UNIQUEIDENTIFIER
AS
BEGIN
SET NOCOUNT ON;

DECLARE @ReturnData TABLE
(
	ReturnId UNIQUEIDENTIFIER NOT NULL,
	WeeeReceivedNonHouseHold DECIMAL(35,3) NULL,
	WeeeReceivedHouseHold DECIMAL(35,3) NULL,
	WeeeSentOnNonHouseHold DECIMAL(35,3) NULL,
	WeeeSentOnHouseHold DECIMAL(35,3) NULL,
	WeeeReusedNonHouseHold DECIMAL(35,3) NULL,
	WeeeReusedHouseHold DECIMAL(35,3) NULL
)

-- INSERT Weee Received
INSERT INTO @ReturnData(ReturnId, WeeeReceivedNonHouseHold, WeeeReceivedHouseHold)
SELECT
	r.Id,
	SUM(wra.NonHouseholdTonnage) AS ReceivedTotalNonhouseHold,
	SUM(wra.HouseholdTonnage) AS ReceivedTotalHouseHold 
FROM
	[AATF].WeeeReceived wr
	INNER JOIN [AATF].WeeeReceivedAmount wra WITH (NOLOCK) ON wr.Id = wra.WeeeReceivedId 
	INNER JOIN [AATF].[Return] r ON r.Id = wr.ReturnId
WHERE
	wr.AatfId = @AatfId
GROUP BY
	r.Id

-- INSERT Weee Sent On for non matched returns
INSERT INTO @ReturnData(ReturnId, WeeeSentOnNonHouseHold, WeeeSentOnHouseHold)
SELECT
	r.Id,
	SUM(wsoa.NonHouseholdTonnage) AS SentOnTotalNonHouseHold,
	SUM(wsoa.HouseholdTonnage) AS SentOnTotalHousehold
FROM
	[AATF].WeeeSentOn wso
	INNER JOIN [AATF].WeeeSentOnAmount wsoa WITH (NOLOCK) ON wso.Id = wsoa.WeeeSentOnId
	INNER JOIN [AATF].[Return] r ON r.Id = wso.ReturnId
WHERE
	wso.AatfId = @AatfId
	AND r.Id NOT IN (SELECT ReturnId FROM @ReturnData)
GROUP BY
	r.Id

-- UPDATE Weee Sent on for matched returns
UPDATE
	@ReturnData
SET
	WeeeSentOnNonHouseHold = SentOnTotalNonHouseHold,
	WeeeSentOnHouseHold = SentOnTotalHouseHold
FROM
	@ReturnData rs
	INNER JOIN (
		SELECT
			r.Id,
			SUM(wsoa.NonHouseholdTonnage) AS SentOnTotalNonHouseHold,
			SUM(wsoa.HouseholdTonnage) AS SentOnTotalHouseHold
		FROM
			[AATF].WeeeSentOn wso
			INNER JOIN [AATF].WeeeSentOnAmount wsoa WITH (NOLOCK) ON wso.Id = wsoa.WeeeSentOnId
			INNER JOIN [AATF].[Return] r ON r.Id = wso.ReturnId
		WHERE
			wso.AatfId = @AatfId
		GROUP BY
			r.Id) sentOn ON sentOn.Id = rs.ReturnId

-- INSERT Weee Reused non matched returns
INSERT INTO @ReturnData(ReturnId, WeeeSentOnNonHouseHold, WeeeSentOnHouseHold)
SELECT
	r.Id,
	SUM(wra.NonHouseholdTonnage) AS ReusedTotalNonHouseHold,
	SUM(wra.HouseholdTonnage) AS ReusedTotalHouseHold
FROM
	[AATF].WeeeReused wr
	INNER JOIN [AATF].WeeeReusedAmount wra ON wr.Id = wra.WeeeReusedId
	INNER JOIN [AATF].[Return] r ON r.Id = wr.ReturnId
WHERE
	wr.AatfId = @AatfId
	AND r.Id NOT IN (SELECT ReturnId FROM @ReturnData)
GROUP BY
	r.Id

-- UPDATE Weee Reused on for matched returns
UPDATE
	@ReturnData
SET
	WeeeReusedNonHouseHold = ReusedTotalNonHouseHold,
	WeeeReusedHouseHold = WeeeReceivedHouseHold
FROM
	@ReturnData rs
	INNER JOIN (
			SELECT
				r.Id,
				SUM(wra.NonHouseholdTonnage) AS ReusedTotalNonHouseHold,
				SUM(wra.HouseholdTonnage) AS ReusedTotalHouseHold
			FROM
				[AATF].WeeeReused wr
				INNER JOIN [AATF].WeeeReusedAmount wra ON wr.Id = wra.WeeeReusedId
				INNER JOIN [AATF].[Return] r ON r.Id = wr.ReturnId
			WHERE
				wr.AatfId = @AatfId
			GROUP BY
				r.Id) reused ON reused.Id = rs.ReturnId
SELECT
	rd.ReturnId,
	rd.WeeeReceivedHouseHold,
	rd.WeeeReceivedNonHouseHold,
	rd.WeeeReusedHouseHold,
	rd.WeeeReusedNonHouseHold,
	rd.WeeeSentOnHouseHold,
	rd.WeeeSentOnNonHouseHold
FROM
	@ReturnData rd
	INNER JOIN [AATF].[Return] r ON r.Id = rd.ReturnId

END
GO