IF OBJECT_ID('Evidence.vwTransferTonnageSummary', 'V') IS NOT NULL
	DROP VIEW Evidence.vwTransferTonnageSummary;
GO

CREATE VIEW [Evidence].[vwTransferTonnageSummary] AS

SELECT DISTINCT
	n.Id AS TransferNoteId,
	n2.Id AS OriginalNoteId,
	receivedCat.[1] AS Cat1Received,
	receivedCat.[2] AS Cat2Received,
	receivedCat.[3] AS Cat3Received,
	receivedCat.[4] AS Cat4Received,
	receivedCat.[5] AS Cat5Received,
	receivedCat.[6] AS Cat6Received,
	receivedCat.[7] AS Cat7Received,
	receivedCat.[8] AS Cat8Received,
	receivedCat.[9] AS Cat9Received,
	receivedCat.[10] AS Cat10Received,
	receivedCat.[11] AS Cat11Received,
	receivedCat.[12] AS Cat12Received,
	receivedCat.[13] AS Cat13Received,
	receivedCat.[14] AS Cat14Received,
	receivedCat.[1] + receivedCat.[2] + receivedCat.[3]+ receivedCat.[4] + receivedCat.[5] + receivedCat.[6] + receivedCat.[7] + receivedCat.[8] + receivedCat.[9] + receivedCat.[10] + receivedCat.[11]
		+ receivedCat.[12] + receivedCat.[13] + receivedCat.[14] AS TotalReceived,
	reusedCat.[1] AS Cat1Reused, 
	reusedCat.[2] AS Cat2Reused,
	reusedCat.[3] AS Cat3Reused,
	reusedCat.[4] AS Cat4Reused,
	reusedCat.[5] AS Cat5Reused,
	reusedCat.[6] AS Cat6Reused,
	reusedCat.[7] AS Cat7Reused,
	reusedCat.[8] AS Cat8Reused,
	reusedCat.[9] AS Cat9Reused,
	reusedCat.[10] AS Cat10Reused,
	reusedCat.[11] AS Cat11Reused,
	reusedCat.[12] AS Cat12Reused,
	reusedCat.[13] AS Cat13Reused,
	reusedCat.[14] AS Cat14Reused,
	reusedCat.[1] + reusedCat.[2] + reusedCat.[3]+ reusedCat.[4] + reusedCat.[5] + reusedCat.[6] + reusedCat.[7] + reusedCat.[8] + reusedCat.[9] + reusedCat.[10] + reusedCat.[11]
		+ reusedCat.[12] + reusedCat.[13] + reusedCat.[14] AS TotalReused
FROM
	[Evidence].Note n
	INNER JOIN [Evidence].NoteTransferTonnage ntt on ntt.TransferNoteId = n.Id
	INNER JOIN [Evidence].NoteTonnage nt ON nt.Id = ntt.NoteTonnageId
	INNER JOIN [Evidence].Note n2 ON n2.Id = nt.NoteId
	CROSS APPLY
		(

		SELECT
			pvt.TransferNoteId,
			pvt.OriginalNoteId,
			COALESCE(CAST([1] AS DECIMAL(28, 3)), 0) AS [1], COALESCE(CAST([2] AS DECIMAL(28, 3)), 0) AS [2], COALESCE(CAST([3] AS DECIMAL(28, 3)), 0) AS [3], COALESCE(CAST([4] AS DECIMAL(28, 3)), 0) AS [4], 
				COALESCE(CAST([5] AS DECIMAL(28, 3)), 0) AS [5], COALESCE(CAST([6] AS DECIMAL(28, 3)), 0) AS [6], COALESCE(CAST([7] AS DECIMAL(28, 3)), 0) AS [7],	COALESCE(CAST([8] AS DECIMAL(28, 3)), 0) AS [8], 
				COALESCE(CAST([9] AS DECIMAL(28, 3)), 0) AS [9], COALESCE(CAST([10] AS DECIMAL(28, 3)), 0) AS [10], COALESCE(CAST([11] AS DECIMAL(28, 3)), 0) AS [11], COALESCE(CAST([12] AS DECIMAL(28, 3)), 0) AS [12], 
				COALESCE(CAST([13] AS DECIMAL(28, 3)), 0) AS [13], COALESCE(CAST([14] AS DECIMAL(28, 3)), 0) AS [14]
		FROM
				(SELECT
					COALESCE(ntt.Received, 0) AS ReceivedTransferred,
					nt.CategoryId,
					nt.NoteId AS OriginalNoteId,
					ntt.TransferNoteId
				FROM    
					[Evidence].NoteTransferTonnage ntt
					INNER JOIN [Evidence].NoteTonnage nt ON nt.Id = ntt.NoteTonnageId
					INNER JOIN [Evidence].Note nz ON nz.Id = ntt.TransferNoteId
				WHERE 
					ntt.TransferNoteId = n.Id
					AND nt.NoteId = n2.Id
				GROUP BY
					ntt.TransferNoteId,
					nt.NoteId,
					nt.CategoryId,
					ntt.Received
				) AS t
				PIVOT
				(   AVG(ReceivedTransferred) FOR CategoryId IN ([1], [2], [3], [4], [5], [6], [7],	[8], [9], [10], [11], [12], [13], [14])
				) pvt
			) receivedCat 
	CROSS APPLY
		(

			SELECT
				pvt.TransferNoteId,
				pvt.OriginalNoteId,
				COALESCE(CAST([1] AS DECIMAL(28, 3)), 0) AS [1], COALESCE(CAST([2] AS DECIMAL(28, 3)), 0) AS [2], COALESCE(CAST([3] AS DECIMAL(28, 3)), 0) AS [3], COALESCE(CAST([4] AS DECIMAL(28, 3)), 0) AS [4], 
					COALESCE(CAST([5] AS DECIMAL(28, 3)), 0) AS [5], COALESCE(CAST([6] AS DECIMAL(28, 3)), 0) AS [6], COALESCE(CAST([7] AS DECIMAL(28, 3)), 0) AS [7], COALESCE(CAST([8] AS DECIMAL(28, 3)), 0) AS [8], 
					COALESCE(CAST([9] AS DECIMAL(28, 3)), 0) AS [9], COALESCE(CAST([10] AS DECIMAL(28, 3)), 0) AS [10], COALESCE(CAST([11] AS DECIMAL(28, 3)), 0) AS [11], COALESCE(CAST([12] AS DECIMAL(28, 3)), 0) AS [12], 
					COALESCE(CAST([13] AS DECIMAL(28, 3)), 0) AS [13], COALESCE(CAST([14] AS DECIMAL(28, 3)), 0) AS [14]
			FROM
					(SELECT
						COALESCE(ntt.Reused, 0) AS ReusedTransferred,
						nt.CategoryId,
						nt.NoteId AS OriginalNoteId,
						ntt.TransferNoteId
					FROM    
						[Evidence].NoteTransferTonnage ntt
						INNER JOIN [Evidence].NoteTonnage nt ON nt.Id = ntt.NoteTonnageId
						INNER JOIN [Evidence].Note nz ON nz.Id = ntt.TransferNoteId
					WHERE 
						ntt.TransferNoteId = n.Id
						AND nt.NoteId = n2.Id
					GROUP BY
						ntt.TransferNoteId,
						nt.NoteId,
						nt.CategoryId,
						ntt.Reused
					) AS t
					PIVOT
					(   AVG(ReusedTransferred) FOR CategoryId IN ([1], [2], [3], [4], [5], [6], [7],	[8], [9], [10], [11], [12], [13], [14])
					) pvt
				) reusedCat 

GO