/*
 * This script renames the [Charging].[1B1STransactionNumberSequence] table tp
 * [Charging].[IbisTransactionNumberSequence] to be more consistent with the
 * [Charging].[IbisFileData] table.
 *
 * It also updates the stored procedure that atomically reads and updates the
 * value in this table to reference the new table name and to remove the
 * unneccesary explicit transaction.
 */

EXEC sp_rename '[Charging].[1B1STransactionNumberSequence]', 'IbisTransactionNumberSequence'
GO

DROP PROCEDURE [Charging].[SpgNext1B1STransactionNumber]

GO
CREATE PROCEDURE [Charging].[SpgNextIbisTransactionNumber]
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @NextNumber INT
	
	SELECT @NextNumber = [NextNumber]
	FROM [Charging].[IbisTransactionNumberSequence]
	
	UPDATE [Charging].[IbisTransactionNumberSequence]
	SET [NextNumber] = @NextNumber + 1
	WHERE [NextNumber] = @NextNumber
			
	IF @@ROWCOUNT <> 1
		RAISERROR ('Concurrency check failed for update to IbisTransactionNumberSequence table.', 16, 1)
	ELSE		
		SELECT @NextNumber
END