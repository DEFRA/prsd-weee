ALTER TABLE [AATF].[WeeeReusedAmount] DROP COLUMN ObligationType
ALTER TABLE [AATF].[WeeeReusedAmount] DROP COLUMN Tonnage
ALTER TABLE [AATF].[WeeeReusedAmount] DROP COLUMN RowVersion
ALTER TABLE [AATF].[WeeeReusedAmount] ADD HouseholdTonnage decimal(28, 3)
ALTER TABLE [AATF].[WeeeReusedAmount] ADD NonHouseholdTonnage decimal(28, 3)
ALTER TABLE [AATF].[WeeeReusedAmount] ADD RowVersion timestamp