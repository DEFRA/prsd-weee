ALTER TABLE [Producer].[RegisteredProducer]
ADD [RemovedDate] DATETIME NULL;

GO

-- Populate the new column by retrieving the date/time the change happened from audit log. Fallback to
-- using the last submission date for the producer if that fails.
UPDATE RP
SET RP.[RemovedDate] = 
   COALESCE(
            (SELECT AL.EventDate
             FROM [Auditing].[AuditLog] AL
             WHERE AL.RecordId = RP.Id 
             AND (
                   (AL.NewValue like '%"IsAligned":false%' AND AL.OriginalValue like '%"IsAligned":true%') OR
                   (AL.NewValue like '%"Removed":true%' AND AL.OriginalValue like '%"Removed":false%')
                 )
             ),
             
             (SELECT DATEADD(MINUTE, 1, MU.SubmittedDate)
              FROM [Producer].[ProducerSubmission] PS
              JOIN [PCS].[MemberUpload] MU ON PS.MemberUploadId = MU.Id
              WHERE PS.Id = RP.CurrentSubmissionId
             )
           )
FROM [Producer].[RegisteredProducer] RP
WHERE RP.Removed = 1;