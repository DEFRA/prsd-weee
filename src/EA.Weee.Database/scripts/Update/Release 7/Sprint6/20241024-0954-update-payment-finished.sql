UPDATE [Producer].DirectProducerSubmission
SET PaymentFinished = 0
WHERE PaymentFinished IS NULL;

ALTER TABLE [Producer].DirectProducerSubmission
ADD CONSTRAINT DF_DirectProducerSubmission_PaymentFinished
DEFAULT 0 FOR PaymentFinished;

ALTER TABLE [Producer].DirectProducerSubmission ALTER COLUMN PaymentFinished BIT NOT NULL;