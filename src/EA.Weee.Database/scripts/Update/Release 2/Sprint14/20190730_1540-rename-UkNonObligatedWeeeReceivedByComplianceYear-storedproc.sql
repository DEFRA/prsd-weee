IF OBJECT_ID('[AATF].getUkNonObligatedWeeeReceivedByComplianceYear', 'P') IS NOT NULL BEGIN
	DROP PROCEDURE [AATF].[getUkNonObligatedWeeeReceivedByComplianceYear]
END
GO
IF OBJECT_ID('[AATF].UkNonObligatedWeeeReceivedByComplianceYear', 'P') IS NOT NULL BEGIN
	EXEC sp_rename '[AATF].UkNonObligatedWeeeReceivedByComplianceYear', 'getUkNonObligatedWeeeReceivedByComplianceYear'
END