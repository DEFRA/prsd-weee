-- This script removes AATF records which have the same approval number and facility name.
-- The WeeeDeliveredAmount table is updated to reference the correct record during the operation.

DECLARE @aatf TABLE(
 Id UNIQUEIDENTIFIER,
 GroupNumber BIGINT,
 GroupRowNumber BIGINT
);

INSERT INTO @aatf
SELECT
 ADL.Id,
 DENSE_RANK() OVER 
 (
    ORDER BY ADL.ApprovalNumber, ADL.FacilityName 
 ) AS GroupNumber,
 ROW_NUMBER() OVER
 (
    PARTITION BY ADL.ApprovalNumber, ADL.FacilityName
    ORDER BY ADL.ApprovalNumber, ADL.FacilityName
 ) AS GroupRowNumber
FROM [PCS].[AatfDeliveryLocation] ADL

UPDATE WDA
SET WDA.AatfDeliveryLocationId = 
 (SELECT AATF2.Id
  FROM @aatf AATF2
  WHERE 
     AATF2.GroupNumber = AATF1.GroupNumber
    AND
     AATF2.GroupRowNumber = 1)
FROM [PCS].[WeeeDeliveredAmount] WDA
JOIN @aatf AATF1 ON WDA.AatfDeliveryLocationId = AATF1.Id
                 AND AATF1.GroupRowNumber <> 1
   
DELETE ADL
FROM [PCS].[AatfDeliveryLocation] ADL
JOIN @aatf AATF ON ADL.Id = AATF.Id
                AND AATF.GroupRowNumber <> 1

-- Create a unique index for the approval number and facility name columns
CREATE UNIQUE NONCLUSTERED INDEX [IX_AatfDeliveryLocation_ApprovalNumber_FacilityName] ON [PCS].[AatfDeliveryLocation]
(
    [ApprovalNumber] ASC,
    [FacilityName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO