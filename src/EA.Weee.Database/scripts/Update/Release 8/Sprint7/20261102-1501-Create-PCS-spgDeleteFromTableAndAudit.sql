SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




-- Create date: 2026 Oct 29
-- Modified date: 2026 Oct 29
-- Description:	This stored procedure deletes data from a table
--				and also its audit table references using passed in IDs
-- =============================================
CREATE	PROCEDURE [PCS].[spgDeleteFromTableAndAudit]
	@Ids		[PCS].[MyUniqueIDs] READONLY,
    @TableName	NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE	@DeleteFromTable	NVARCHAR(MAX);
	DECLARE	@DeleteFromAudit	NVARCHAR(MAX);

	SET	@DeleteFromTable = 'DELETE FROM ' + @TableName + ' WHERE Id In (SELECT Id FROM @MyIds);';
	SET	@DeleteFromAudit = 'DELETE FROM [Auditing].[AuditLog] WHERE TableName = ''' + @TableName + ''' AND RecordId In (SELECT Id FROM @MyIds);';

	PRINT 'Delete from ' + @TableName;
	EXEC sp_executesql @DeleteFromTable, N'@MyIDs [PCS].[MyUniqueIDs] READONLY', @MyIDs = @IDs;
	PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);
	PRINT 'Delete from [Auditing].[AuditLog] for entries relating to ' + @TableName;
	EXEC sp_executesql @DeleteFromAudit, N'@MyIDs [PCS].[MyUniqueIDs] READONLY', @MyIDs = @IDs;
	PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10);

END