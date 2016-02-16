/*
	This script fixes an issue with the unique indexes on the PCS.AatfDeliveryLocation table and 
	the PCS.AeDeliveryLocation table. The unique indexes cover the FacilityName and OperatorName
	columns respectively.
	
	SQL and .NET differ in how they compare string data.
	In SQL, values that differ only by trailing whitespace are considered equal.
	In .NET, values that differ only by trailing whitespace are considered unequal.
	For example, 'ABC' and 'ABC ' are considered equal in SQL but not in .NET.
	
	The requirement for WEEE is that trailing whitespace in facility names and operator names
	is both preserved and considered significant.
	
	Assuming that ANSI_PADDING was set to ON (the default value) when these NVARCHAR columns
	were created, the trailing whitespace will be being stored. Therefore the issue is with the
	string comparison used when SQL determines whether an INSERT would violate the unique index.
	
	We cannot change the string comparison behaviour of SQL, but we can add an extra column to
	the index which discriminates between strings with differing whitespace. This would ensure
	that values with different amounts of trailing whitespace generate different keys in the index.
	
	The obvious choice for this would be the LEN function, but unfortunately the LEN function
	trims trailing whitespace. Instead we can use the DATALENGTH function. This function
	returns the amount of space (in bytes) used to store the value. Assuming that whitespace
	is being stroed, this function will return different values for string with differing
	trailing whitespace.
	
	This script adds computed columns to both tables that return the DATALENGTH of the
	NVARCHAR value. These computed columns are deterministic so they can be added to each
	unique index.
*/

-- PCS.AatfDeliveryLocation

ALTER TABLE [PCS].[AatfDeliveryLocation]
ADD [FacilityNameDataLength] AS DATALENGTH(FacilityName)
GO

DROP INDEX [IX_AatfDeliveryLocation_ApprovalNumber_FacilityName] ON [PCS].[AatfDeliveryLocation] WITH ( ONLINE = OFF )
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_AatfDeliveryLocation_ApprovalNumber_FacilityName] ON [PCS].[AatfDeliveryLocation] 
(
	[ApprovalNumber] ASC,
	[FacilityName] ASC,
	[FacilityNameDataLength] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

-- PCS.AeDeliveryLocation

ALTER TABLE [PCS].[AeDeliveryLocation]
ADD [OperatorNameDataLength] AS DATALENGTH(OperatorName)
GO

DROP INDEX [IX_AeDeliveryLocation_ApprovalNumber_OperatorName] ON [PCS].[AeDeliveryLocation] WITH ( ONLINE = OFF )
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_AeDeliveryLocation_ApprovalNumber_OperatorName] ON [PCS].[AeDeliveryLocation] 
(
	[ApprovalNumber] ASC,
	[OperatorName] ASC,
	[OperatorNameDataLength] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

