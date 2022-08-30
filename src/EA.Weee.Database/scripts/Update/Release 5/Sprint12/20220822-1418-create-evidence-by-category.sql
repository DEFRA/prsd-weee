GO
IF OBJECT_ID('Evidence.vwEvidenceByCategory', 'V') IS NOT NULL
	DROP VIEW Evidence.vwEvidenceByCategory;
GO


CREATE VIEW [Evidence].vwEvidenceByCategory AS
SELECT
		n.Id,
		receivedCat.[1] AS Received1, 
		receivedCat.[2] AS Received2,
		receivedCat.[3] AS Received3,
		receivedCat.[4] AS Received4,
		receivedCat.[5] AS Received5,
		receivedCat.[6] AS Received6,
		receivedCat.[7] AS Received7,
		receivedCat.[8] AS Received8,
		receivedCat.[9] AS Received9,
		receivedCat.[10] AS Received10,
		receivedCat.[11] AS Received11,
		receivedCat.[12] AS Received12,
		receivedCat.[13] AS Received13,
		receivedCat.[14] AS Received14,
		receivedCat.[1] + receivedCat.[2] + receivedCat.[3]+ receivedCat.[4] + receivedCat.[5] + receivedCat.[6] + receivedCat.[7] + receivedCat.[8] + receivedCat.[9] + receivedCat.[10] + receivedCat.[11]
			+ receivedCat.[12] + receivedCat.[13] + receivedCat.[14] AS ReceivedTotal,
		reusedCat.[1] AS Reused1, 
		reusedCat.[2] AS Reused2,
		reusedCat.[3] AS Reused3,
		reusedCat.[4] AS Reused4,
		reusedCat.[5] AS Reused5,
		reusedCat.[6] AS Reused6,
		reusedCat.[7] AS Reused7,
		reusedCat.[8] AS Reused8,
		reusedCat.[9] AS Reused9,
		reusedCat.[10] AS Reused10,
		reusedCat.[11] AS Reused11,
		reusedCat.[12] AS Reused12,
		reusedCat.[13] AS Reused13,
		reusedCat.[14] AS Reused14,
		reusedCat.[1] + reusedCat.[2] + reusedCat.[3] + reusedCat.[4] + reusedCat.[5] + reusedCat.[6] + reusedCat.[7] + reusedCat.[8] + reusedCat.[9] + reusedCat.[10] + reusedCat.[11]
			+ reusedCat.[12] + reusedCat.[13] + reusedCat.[14] AS ReusedTotal
FROM
	[Evidence].Note n
	CROSS APPLY
		(
		SELECT
			pvt.NoteId,
			[1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12], [13], [14]
		FROM
				(SELECT
					COALESCE(nt.Received, 0) AS Received,
					n1.Id AS NoteId,
					nt.CategoryId AS Category
				 FROM    
					[Evidence].NoteTonnage nt
					INNER JOIN [Evidence].Note n1 ON n1.Id = nt.NoteId AND n1.WasteType = 1 AND n1.Status = 3
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
				[1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12], [13], [14]
			FROM
					(SELECT
						COALESCE(nt.Reused, 0) AS Reused,
						n1.Id AS NoteId,
						nt.CategoryId AS Category
					FROM 
						[Evidence].NoteTonnage nt
						INNER JOIN [Evidence].Note n1 ON n1.Id = nt.NoteId AND n1.WasteType = 1 AND n1.Status = 3	
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