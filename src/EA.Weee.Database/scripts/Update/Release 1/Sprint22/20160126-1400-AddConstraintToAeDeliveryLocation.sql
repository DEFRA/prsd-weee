-- This script removes AE records which have the same approval number and operator name.
-- The WeeeDeliveredAmount table is updated to reference the correct record during the operation.

DECLARE @ae TABLE(
 Id UNIQUEIDENTIFIER,
 GroupNumber BIGINT,
 GroupRowNumber BIGINT
);

INSERT INTO @ae
SELECT
 ADL.Id,
 DENSE_RANK() OVER 
 (
    ORDER BY ADL.ApprovalNumber, ADL.OperatorName
 ) AS GroupNumber,
 ROW_NUMBER() OVER
 (
    PARTITION BY ADL.ApprovalNumber, ADL.OperatorName
    ORDER BY ADL.ApprovalNumber, ADL.OperatorName
 ) AS GroupRowNumber
FROM [PCS].[AeDeliveryLocation] ADL

UPDATE WDA
SET WDA.AeDeliveryLocationId = 
 (SELECT AE2.Id
  FROM @ae AE2
  WHERE 
     AE2.GroupNumber = AE1.GroupNumber
    AND
     AE2.GroupRowNumber = 1)
FROM [PCS].[WeeeDeliveredAmount] WDA
JOIN @ae AE1 ON WDA.AeDeliveryLocationId = AE1.Id
                 AND AE1.GroupRowNumber <> 1

DELETE ADL
FROM [PCS].[AeDeliveryLocation] ADL
JOIN @ae AE ON ADL.Id = AE.Id
                AND AE.GroupRowNumber <> 1

-- Create a unique index for the approval number and facility name columns
CREATE UNIQUE NONCLUSTERED INDEX [IX_AeDeliveryLocation_ApprovalNumber_OperatorName] ON [PCS].[AeDeliveryLocation]
(
    [ApprovalNumber] ASC,
    [OperatorName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO