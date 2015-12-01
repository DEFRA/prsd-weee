-- Rename column to be more accurate to what is being stored
EXEC sp_rename '[PCS].[MemberUpload].[ValidationTime]', 'ProcessTime', 'COLUMN'