ALTER TABLE [AATF].[WeeeReused] DROP COLUMN AATFReturnId
ALTER TABLE [AATF].[WeeeReused] ADD  ReturnId uniqueidentifier NOT NULL
ALTER TABLE [AATF].[WeeeReused] ADD CONSTRAINT FK_WeeeReused_Return_ReturnId FOREIGN KEY (ReturnId) REFERENCES [AATF].[Return](Id);
ALTER TABLE [AATF].[WeeeReused] Add AatfId uniqueidentifier NOT NULL
ALTER TABLE [AATF].[WeeeReused] ADD CONSTRAINT FK_WeeeReused_Aatf_AatfId FOREIGN KEY (AatfId) REFERENCES [AATF].[AATF](Id);