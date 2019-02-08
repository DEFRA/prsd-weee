ALTER TABLE [AATF].[WeeeReceivedAmount] DROP COLUMN ObligationType
ALTER TABLE [AATF].[WeeeReceivedAmount] DROP COLUMN Tonnage
ALTER TABLE [AATF].[WeeeReceivedAmount] DROP COLUMN RowVersion
ALTER TABLE [AATF].[WeeeReceivedAmount] ADD HouseholdTonnage decimal(28, 3)
ALTER TABLE [AATF].[WeeeReceivedAmount] ADD NonHouseholdTonnage decimal(28, 3)
ALTER TABLE [AATF].[WeeeReceivedAmount] ADD RowVersion timestamp