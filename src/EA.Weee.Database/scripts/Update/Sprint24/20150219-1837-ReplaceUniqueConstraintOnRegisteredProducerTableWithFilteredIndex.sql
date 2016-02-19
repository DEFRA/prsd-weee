/*
	This script replaces the existing unique constraint on the [Producer].[RegisteredProducer]
	table with a unique filtered index.
	
	Currently, the constraint prevents more the table from containing more than one
	active registration for the same scheme, compliance year and producer registration number.
	
	However, the constraint includes the [Removed] column, which also prevents the table
	from containing more than one *inactive* registration for the same scheme, compliance
	year and producer registration number. This is incorrect.
	
	Instead of using a constraint, a filtered index will be used that enforces uniqueness
	only for the active registrations.
*/

ALTER TABLE [Producer].[RegisteredProducer]
DROP CONSTRAINT [CN_RegisteredProducer_Unique_SchemeId_ProducerRegistrationNumber_ComplianceYear_Removed]
GO

CREATE NONCLUSTERED INDEX [IX_RegisteredProducer_Unique_SchemeId_ProducerRegistrationNumber_ComplianceYear_WhereNotRemoved] ON [Producer].[RegisteredProducer] 
(
	[SchemeId] ASC,
	[ProducerRegistrationNumber] ASC,
	[ComplianceYear] ASC
)
WHERE [Removed] = 0
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
