GO
PRINT N'Altering [PCS].[MemberUpload]...';


GO
ALTER TABLE [PCS].[MemberUpload] ADD [UserId] NVARCHAR (128) NULL;

ALTER TABLE [PCS].[MemberUpload] ADD [Date] DATETIME NULL;


GO

PRINT N'Update existing data for UserId and Date in [PCS].[MemberUpload]...'
GO

IF OBJECT_ID('tempdb..#temp_table') IS NOT NULL
		DROP TABLE #temp_table

 select 
		A.EventDate,
		MU.Id as MuID,
		A.UserId     
	into #temp_table
  FROM           
  [EA.Weee].[PCS].[MemberUpload] MU 
  inner join [EA.Weee].[Auditing].[AuditLog] A on MU.Id = a.RecordId

  update PCS.MemberUpload
	Set Date = T.EventDate, 
		UserId = T.UserId

  from #temp_table T 
  inner join PCs.MemberUpload M on T.MuId = M.Id

  PRINT N'Update complete.';
GO