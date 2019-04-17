IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'StatusType'
          AND Object_ID = Object_ID(N'[Producer].[ProducerSubmission]'))
BEGIN
    ALTER TABLE [Producer].[ProducerSubmission]
	ADD StatusType INT NULL
END
GO