ALTER TABLE [Producer].[ProducerSubmission]
ADD [Invoiced] [bit] NULL
GO

UPDATE P
SET P.[Invoiced] = 
(CASE 
   WHEN M.[InvoiceRunId] IS NULL THEN 0
   ELSE 1
 END)
FROM [Producer].[ProducerSubmission] P
JOIN [PCS].[MemberUpload] M ON P.MemberUploadId = M.ID
GO

ALTER TABLE [Producer].[ProducerSubmission]
ALTER COLUMN [Invoiced] [bit] NOT NULL
GO