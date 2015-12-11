﻿CREATE UNIQUE NONCLUSTERED INDEX [IX_DataReturn_Unique_SchemeId_Quarter_ComplianceYear] ON [PCS].[DataReturn]
(
	[ComplianceYear] ASC,
	[SchemeId] ASC,
	[Quarter] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO