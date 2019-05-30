
ALTER TABLE [AATF].[Return] ADD OrganisationId UNIQUEIDENTIFIER NULL
GO
ALTER TABLE [AATF].[Return] ADD CONSTRAINT FK_Return_Organisation FOREIGN KEY (OrganisationId)  REFERENCES [Organisation].[Organisation](Id) 
GO

UPDATE
	r
SET
	r.OrganisationId = o.OrganisationId
FROM
	[AATF].[Return] r
	INNER JOIN [AATF].[Operator] o ON o.Id = r.OperatorId

ALTER TABLE [AATF].[Return] ALTER COLUMN OrganisationId UNIQUEIDENTIFIER NOT NULL
GO