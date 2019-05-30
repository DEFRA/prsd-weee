EXEC sp_rename 'AATF.AATFReturnReportOn','ReturnReportOn'

ALTER TABLE [AATF].[ReturnReportOn] DROP COLUMN AATFReturnId
ALTER TABLE [AATF].[ReturnReportOn] ADD  ReturnId uniqueidentifier NOT NULL
ALTER TABLE [AATF].[ReturnReportOn] ADD CONSTRAINT FK_ReturnReportOn_Return_ReturnId FOREIGN KEY (ReturnId) REFERENCES [AATF].[Return](Id);