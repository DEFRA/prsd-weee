/*
 * Now that all of the data in the producer table has been moved to the registered producer
 * and producer submission tables, the producer table can be dropped.
 */

ALTER TABLE [Producer].[Producer] DROP CONSTRAINT [FK_Producer_AuthorisedRepresentative]
ALTER TABLE [Producer].[Producer] DROP CONSTRAINT [FK_Producer_Business]
ALTER TABLE [Producer].[Producer] DROP CONSTRAINT [FK_Producer_ChargeBandAmount]
ALTER TABLE [Producer].[Producer] DROP CONSTRAINT [FK_Producer_MemberUpload]
ALTER TABLE [Producer].[Producer] DROP CONSTRAINT [FK_Producer_Scheme]
ALTER TABLE [Producer].[Producer] DROP CONSTRAINT [DF_Producer_AnnualTurnover]
ALTER TABLE [Producer].[Producer] DROP CONSTRAINT [DF_Producer_IsCurrentForComplianceYear]
GO

DROP TABLE [Producer].[Producer]
GO