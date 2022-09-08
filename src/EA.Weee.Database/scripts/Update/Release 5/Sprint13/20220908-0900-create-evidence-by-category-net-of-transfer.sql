GO
IF OBJECT_ID('Evidence.vwEvidenceByCategoryNetOfTransfer', 'V') IS NOT NULL
	DROP VIEW Evidence.vwEvidenceByCategoryNetOfTransfer;
GO


CREATE VIEW [Evidence].[vwEvidenceByCategoryNetOfTransfer] AS
SELECT
		n.Id,
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
		receivedCat.[1] + receivedCat.[2] + receivedCat.[3]+ receivedCat.[4] + receivedCat.[5] + receivedCat.[6] + receivedCat.[7] + receivedCat.[8] + receivedCat.[8] + receivedCat.[10] + receivedCat.[11]
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
		reusedCat.[1] + reusedCat.[2] + reusedCat.[3]+ reusedCat.[4] + reusedCat.[5] + reusedCat.[6] + reusedCat.[7] + reusedCat.[8] + reusedCat.[8] + reusedCat.[10] + reusedCat.[11]
			+ reusedCat.[12] + reusedCat.[13] + reusedCat.[14] AS TotalReused
FROM
	[Evidence].Note n
	CROSS APPLY
		(
		SELECT
			pvt.NoteId,
			CAST([1] AS DECIMAL(28, 3)) AS [1], CAST([2] AS DECIMAL(28, 3)) AS [2], CAST([3] AS DECIMAL(28, 3)) AS [3], CAST([4] AS DECIMAL(28, 3)) AS [4], 
				CAST([5] AS DECIMAL(28, 3)) AS [5], CAST([6] AS DECIMAL(28, 3)) AS [6], CAST([7] AS DECIMAL(28, 3)) AS [7],	CAST([8] AS DECIMAL(28, 3)) AS [8], 
				CAST([9] AS DECIMAL(28, 3)) AS [9], CAST([10] AS DECIMAL(28, 3)) AS [10], CAST([11] AS DECIMAL(28, 3)) AS [11], CAST([12] AS DECIMAL(28, 3)) AS [12], 
				CAST([13] AS DECIMAL(28, 3)) AS [13], CAST([14] AS DECIMAL(28, 3)) AS [14]
		FROM
				(SELECT
					COALESCE(nt.Received, 0) - COALESCE(SUM(COALESCE(ntt.Received, 0)), 0) AS Received,
					n1.Id AS NoteId,
					nt.CategoryId AS Category
				 FROM    
					[Evidence].NoteTonnage nt
					INNER JOIN [Evidence].Note n1 ON n1.Id = nt.NoteId AND n1.WasteType = 1 AND n1.Status = 3	
					LEFT JOIN [Evidence].NoteTransferTonnage ntt ON ntt.NoteTonnageId = nt.Id
				WHERE 
					nt.NoteId = n.Id
				GROUP BY
					n1.Id,
					nt.CategoryId,
					nt.Received
				) AS t
				PIVOT
				(   AVG(t.Received) FOR Category IN ([1], [2], [3], [4], [5], [6], [7],	[8], [9], [10], [11], [12], [13], [14])
				) pvt
			) receivedCat 
	CROSS APPLY
			(
			SELECT
				pvt.NoteId,
				CAST([1] AS DECIMAL(28, 3)) AS [1], CAST([2] AS DECIMAL(28, 3)) AS [2], CAST([3] AS DECIMAL(28, 3)) AS [3], CAST([4] AS DECIMAL(28, 3)) AS [4], 
				CAST([5] AS DECIMAL(28, 3)) AS [5], CAST([6] AS DECIMAL(28, 3)) AS [6], CAST([7] AS DECIMAL(28, 3)) AS [7],	CAST([8] AS DECIMAL(28, 3)) AS [8], 
				CAST([9] AS DECIMAL(28, 3)) AS [9], CAST([10] AS DECIMAL(28, 3)) AS [10], CAST([11] AS DECIMAL(28, 3)) AS [11], CAST([12] AS DECIMAL(28, 3)) AS [12], 
				CAST([13] AS DECIMAL(28, 3)) AS [13], CAST([14] AS DECIMAL(28, 3)) AS [14]
			FROM
					(SELECT
						COALESCE(nt.Reused, 0) - COALESCE(SUM(COALESCE(ntt.Reused, 0)), 0) AS Reused,
						n1.Id AS NoteId,
						nt.CategoryId AS Category
					FROM 
						[Evidence].NoteTonnage nt
						INNER JOIN [Evidence].Note n1 ON n1.Id = nt.NoteId AND n1.WasteType = 1 AND n1.Status = 3	
						LEFT JOIN [Evidence].NoteTransferTonnage ntt ON ntt.NoteTonnageId = nt.Id
					WHERE 
						nt.NoteId = n.Id
					GROUP BY
						n1.Id,
						nt.CategoryId,
						nt.Reused
					) AS t
					PIVOT
					(   AVG(t.Reused) FOR Category IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12], [13], [14])
					) pvt
				) reusedCat 
		WHERE
			n.NoteType = 1