SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [AATF].getAeSubmissions
	@AeId UNIQUEIDENTIFIER
AS
BEGIN
SET NOCOUNT ON;

SELECT
	r.Id AS ReturnId,
	r.ComplianceYear,
	r.Quarter,
	CONCAT(u.FirstName , ' ', u.Surname) AS SubmittedBy,
	r.SubmittedDate AS SubmittedDate
FROM
	[AATF].[Return] r WITH (NOLOCK)
	INNER JOIN [AATF].[ReturnAatf] ra ON ra.ReturnId = r.Id
	INNER JOIN [Identity].AspNetUsers u ON r.SubmittedById = u.Id
WHERE
	r.ReturnStatus = 2
	AND r.FacilityType = 2
	AND ra.AatfId = @AeId

END
GO