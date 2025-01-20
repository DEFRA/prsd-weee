﻿/****** Object:  View [Evidence].[vwEvidenceByCategoryNetOfTransfer]    Script Date: 16/01/2025 16:28:58 ******/
IF OBJECT_ID('[Evidence].[vwEvidenceByCategoryNetOfTransfer]', 'V') IS NOT NULL
	DROP VIEW [Evidence].[vwEvidenceByCategoryNetOfTransfer]
GO

/****** Object:  View [Evidence].[vwEvidenceByCategoryNetOfTransfer]    Script Date: 16/01/2025 16:28:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
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
		ISNULL(receivedCat.[15], 0) AS Cat15Received,
		receivedCat.[1] + receivedCat.[2] + receivedCat.[3]+ receivedCat.[4] + receivedCat.[5] + 
		receivedCat.[6] + receivedCat.[7] + receivedCat.[8] + receivedCat.[9] + receivedCat.[10] + 
		receivedCat.[11] + receivedCat.[12] + receivedCat.[13] + receivedCat.[14] + ISNULL(receivedCat.[15], 0) AS TotalReceived,
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
		ISNULL(reusedCat.[15], 0) AS Cat15Reused,
		reusedCat.[1] + reusedCat.[2] + reusedCat.[3]+ reusedCat.[4] + reusedCat.[5] + 
		reusedCat.[6] + reusedCat.[7] + reusedCat.[8] + reusedCat.[9] + reusedCat.[10] + 
		reusedCat.[11] + reusedCat.[12] + reusedCat.[13] + reusedCat.[14] + ISNULL(reusedCat.[15], 0) AS TotalReused
FROM
	[Evidence].Note n
	CROSS APPLY
		(
		SELECT
			pvt.NoteId,
			CAST([1] AS DECIMAL(28, 3)) AS [1], 
			CAST([2] AS DECIMAL(28, 3)) AS [2], 
			CAST([3] AS DECIMAL(28, 3)) AS [3], 
			CAST([4] AS DECIMAL(28, 3)) AS [4], 
			CAST([5] AS DECIMAL(28, 3)) AS [5], 
			CAST([6] AS DECIMAL(28, 3)) AS [6], 
			CAST([7] AS DECIMAL(28, 3)) AS [7],	
			CAST([8] AS DECIMAL(28, 3)) AS [8], 
			CAST([9] AS DECIMAL(28, 3)) AS [9], 
			CAST([10] AS DECIMAL(28, 3)) AS [10], 
			CAST([11] AS DECIMAL(28, 3)) AS [11], 
			CAST([12] AS DECIMAL(28, 3)) AS [12], 
			CAST([13] AS DECIMAL(28, 3)) AS [13], 
			CAST([14] AS DECIMAL(28, 3)) AS [14],
			CAST([15] AS DECIMAL(28, 3)) AS [15]
		FROM
				(SELECT
					COALESCE(nt.Received, 0) - COALESCE(transferReceivedSum.TransferReceived, 0) AS Received,
					nt.NoteId AS NoteId,
					nt.CategoryId AS Category
				 FROM    
					[Evidence].NoteTonnage nt
					LEFT JOIN [Evidence].NoteTransferTonnage ntt ON ntt.NoteTonnageId = nt.Id
					LEFT JOIN	
						(
							SELECT
								SUM(COALESCE(ntt.Received, 0)) AS TransferReceived,
								ntt.NoteTonnageId
							FROM
								[Evidence].NoteTransferTonnage ntt
								INNER JOIN [Evidence].Note tn ON tn.Id = ntt.TransferNoteId
							WHERE
								tn.NoteType = 2
								AND tn.Status = 3
							GROUP BY
								ntt.NoteTonnageId
						) transferReceivedSum ON transferReceivedSum.NoteTonnageId = nt.Id
				WHERE 
					nt.NoteId = n.Id
				GROUP BY
					nt.NoteId,
					nt.CategoryId,
					nt.Received,
					transferReceivedSum.TransferReceived
				) AS t
				PIVOT
				(   AVG(t.Received) FOR Category IN ([1], [2], [3], [4], [5], [6], [7],	[8], [9], [10], [11], [12], [13], [14], [15])
				) pvt
			) receivedCat 
	CROSS APPLY
			(
			SELECT
				pvt.NoteId,
				CAST([1] AS DECIMAL(28, 3)) AS [1], 
				CAST([2] AS DECIMAL(28, 3)) AS [2], 
				CAST([3] AS DECIMAL(28, 3)) AS [3], 
				CAST([4] AS DECIMAL(28, 3)) AS [4], 
				CAST([5] AS DECIMAL(28, 3)) AS [5], 
				CAST([6] AS DECIMAL(28, 3)) AS [6], 
				CAST([7] AS DECIMAL(28, 3)) AS [7],	
				CAST([8] AS DECIMAL(28, 3)) AS [8], 
				CAST([9] AS DECIMAL(28, 3)) AS [9], 
				CAST([10] AS DECIMAL(28, 3)) AS [10], 
				CAST([11] AS DECIMAL(28, 3)) AS [11], 
				CAST([12] AS DECIMAL(28, 3)) AS [12], 
				CAST([13] AS DECIMAL(28, 3)) AS [13], 
				CAST([14] AS DECIMAL(28, 3)) AS [14],
				CAST([15] AS DECIMAL(28, 3)) AS [15]
			FROM
					(SELECT
						COALESCE(nt.Reused, 0) - COALESCE(transferReusedSum.TransferReused, 0) AS Reused,
						nt.NoteId AS NoteId,
						nt.CategoryId AS Category
					FROM 
						[Evidence].NoteTonnage nt
						LEFT JOIN [Evidence].NoteTransferTonnage ntt ON ntt.NoteTonnageId = nt.Id
						LEFT JOIN	
						(
							SELECT
								SUM(COALESCE(ntt.Reused, 0)) AS TransferReused,
								ntt.NoteTonnageId
							FROM
								[Evidence].NoteTransferTonnage ntt
								INNER JOIN [Evidence].Note tn ON tn.Id = ntt.TransferNoteId
							WHERE
								tn.NoteType = 2
								AND tn.Status = 3
							GROUP BY
								ntt.NoteTonnageId
						) transferReusedSum ON transferReusedSum.NoteTonnageId = nt.Id
					WHERE 
						nt.NoteId = n.Id
					GROUP BY
						nt.NoteId,
						nt.CategoryId,
						nt.Reused,
						transferReusedSum.TransferReused
					) AS t
					PIVOT
					(   AVG(t.Reused) FOR Category IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12], [13], [14], [15])
					) pvt
				) reusedCat 
		WHERE
			n.NoteType = 1
GO
