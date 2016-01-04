/*
 * This script creates a table to store the next available number that can be used for 1B1S transaction references.
 * It also adds a stored produced that can be used to atomically retrieve and increment hte current value.
 */

CREATE TABLE [Charging].[1B1STransactionNumberSequence] (
	[NextNumber] INT NOT NULL
)
GO

INSERT INTO [Charging].[1B1STransactionNumberSequence]
VALUES ( 800001 )

GO
CREATE PROCEDURE [Charging].[SpgNext1B1STransactionNumber]
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @NextNumber INT
	
	BEGIN TRAN
		
		SELECT @NextNumber = [NextNumber]
		FROM [Charging].[1B1STransactionNumberSequence]
	
		UPDATE [Charging].[1B1STransactionNumberSequence]
		SET [NextNumber] = @NextNumber + 1
		WHERE [NextNumber] = @NextNumber
		
		IF @@ROWCOUNT <> 1
			RAISERROR ('Concurrency check failed for update to 1B1S transaction number sequence table.', 16, 1)
		
	COMMIT
	
	SELECT @NextNumber
END
GO