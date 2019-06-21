-- Add new Removed column
ALTER TABLE [Producer].[RegisteredProducer]
ADD Removed BIT NULL
GO

-- Populate the Removed column values from the IsAligned values
UPDATE P
SET P.Removed = 
 (CASE P.IsAligned
  WHEN 1 THEN 0
  ELSE 1
  END)
FROM [Producer].[RegisteredProducer] P
GO

-- Make the Removed column not nullable
ALTER TABLE [Producer].[RegisteredProducer]
ALTER COLUMN Removed BIT NOT NULL
GO

-- Drop the existing index
ALTER TABLE [Producer].[RegisteredProducer] 
    DROP CONSTRAINT [CN_RegisteredProducer_Unique_SchemeId_ProducerRegistrationNumber_ComplianceYear_IsAligned]
Go

-- Drop the default constraint on the IsAligned column. As this was not name specifically at the time of creation, we need to discover its name.
DECLARE @RegisteredProducerIsAlignedConstraintName NVARCHAR(1000)
SELECT
    @RegisteredProducerIsAlignedConstraintName = OBJECT_NAME(constid)
FROM
    sysconstraints
WHERE 
    OBJECT_NAME(id) = 'RegisteredProducer' 
AND COL_NAME(id, colid) = 'IsAligned'

EXEC('
ALTER TABLE [Producer].[RegisteredProducer]
    DROP CONSTRAINT [' + @RegisteredProducerIsAlignedConstraintName + ']')
GO

-- Drop the IsAligned column
ALTER TABLE [Producer].[RegisteredProducer]
DROP COLUMN IsAligned
GO

-- Add new index
ALTER TABLE [Producer].[RegisteredProducer] ADD CONSTRAINT [CN_RegisteredProducer_Unique_SchemeId_ProducerRegistrationNumber_ComplianceYear_Removed] UNIQUE NONCLUSTERED 
(
    [SchemeId] ASC,
    [ProducerRegistrationNumber] ASC,
    [ComplianceYear] ASC,
    [Removed] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO