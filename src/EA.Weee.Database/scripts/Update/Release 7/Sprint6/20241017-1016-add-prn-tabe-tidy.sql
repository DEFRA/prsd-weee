ALTER TABLE [Producer].[DirectRegistrant] ADD [ProducerRegistrationNumber] [nvarchar](50) NULL;


DECLARE @ConstraintName nvarchar(200)
SELECT @ConstraintName = name
FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID('[Producer].[DirectProducerSubmission]')
AND parent_column_id = (
    SELECT column_id 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('[Producer].[DirectProducerSubmission]')
    AND name = 'Removed'
)

IF @ConstraintName IS NOT NULL
    EXEC('ALTER TABLE [Producer].[DirectProducerSubmission] DROP CONSTRAINT ' + @ConstraintName)

ALTER TABLE [Producer].[DirectProducerSubmission] DROP COLUMN Removed;

DROP INDEX [IX_DirectRegistrant_SICCodeId] ON [Producer].[DirectRegistrant]
GO

ALTER TABLE [Producer].[DirectRegistrant] DROP CONSTRAINT [FK_DirectRegistrant_SICCode]
GO

ALTER TABLE [Producer].[DirectRegistrant] DROP COLUMN SICCodeId;