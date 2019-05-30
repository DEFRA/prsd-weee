DELETE FROM [AATF].NonObligatedWeee
DELETE FROM [AATF].ReturnScheme
DELETE FROM [AATF].WeeeReceivedAmount
DELETE FROM [AATF].WeeeReceived
DELETE FROM [AATF].WeeeReusedAmount
DELETE FROM [AATF].WeeeReusedSite
DELETE FROM [AATF].WeeeReused
DELETE FROM [AATF].WeeeSentOnAmount
DELETE FROM [AATF].WeeeSentOn
DELETE FROM [AATF].ReturnReportOn
DELETE FROM [AATF].AATF
DELETE FROM [AATF].[Address]
DELETE FROM [AATF].[Return]
IF EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[AATF].[Operator]') AND type in (N'U')) BEGIN
	DELETE FROM [AATF].Operator
END
