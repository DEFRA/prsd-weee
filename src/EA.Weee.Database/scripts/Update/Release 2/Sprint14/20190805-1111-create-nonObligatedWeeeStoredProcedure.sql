IF OBJECT_ID('[AATF].getUkNonObligatedWeeeReceivedByComplianceYear', 'P') IS NOT NULL BEGIN
	DROP PROCEDURE [AATF].[getUkNonObligatedWeeeReceivedByComplianceYear]
END
IF OBJECT_ID('[AATF].UkNonObligatedWeeeReceivedByComplianceYear', 'P') IS NOT NULL BEGIN
	DROP PROCEDURE [AATF].[UkNonObligatedWeeeReceivedByComplianceYear]
END
GO
/****** Object:  StoredProcedure [AATF].[UkNonObligatedWeeeReceivedByComplianceYear]    Script Date: 26/07/2019 16:48:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Create date: <18-07-2019>
-- =============================================
CREATE PROCEDURE [AATF].[getUkNonObligatedWeeeReceivedByComplianceYear] 
	@ComplianceYear INT
AS
BEGIN

-- This table will be all the returns we want to get weee data for
CREATE TABLE #FinalReturns(
 Id uniqueidentifier,
 [Quarter] int,
 [Year] int)

-- Insert all returns we want to try an use into a temp table
Select *
Into   #Returns
From   [Aatf].[Return]
WHERE FacilityType = 1
AND ComplianceYear = @ComplianceYear
AND ReturnStatus = 2

DECLARE @Id uniqueidentifier
DECLARE @parentId uniqueidentifier

-- Loop through the temp table, if there is no parent id, see if that row is the parent of another row, then repeat until we find the youngest child row. 
-- We then insert the youngest child row into a temp table so that we can get that return data
WHILE EXISTS(SELECT * FROM #Returns)
BEGIN

    SELECT TOP 1 @Id = Id, @parentId = ParentId FROM #Returns

    WHILE EXISTS(SELECT * FROM #Returns WHERE ParentId = @Id)
	BEGIN
		DELETE #Returns WHERE Id = @Id
		SELECT @Id = Id FROM #Returns WHERE ParentId = @Id
	END
	
	INSERT INTO #FinalReturns (Id, [Quarter], [Year]) SELECT Id, [Quarter], ComplianceYear from #Returns WHERE Id = @Id

    DELETE #Returns WHERE Id = @Id

END

  -- Weee category

 CREATE TABLE #Results(
 [Quarter] nvarchar(5),
 [Category] nvarchar(100),
 [TotalNonObligatedWeeeReceived] decimal(23,3),
 [TotalNonObligatedWeeeReceivedFromDcf] decimal(23,3))

 -- Insert Quarter data
 INSERT INTO #Results
 SELECT CONCAT('Q',fr.Quarter) ,
 CONCAT(w.Id, '. ', w.Name), 
 CONVERT(decimal(28,3), CASE WHEN SUM(CASE WHEN non.Dcf = 0 THEN non.Tonnage ELSE 0 END) IS NULL THEN 0 ELSE SUM(CASE WHEN non.Dcf = 0 THEN non.Tonnage ELSE 0 END) END), 
 CONVERT(decimal(28,3), CASE WHEN SUM(CASE WHEN non.Dcf = 1 THEN non.Tonnage ELSE 0 END) IS NULL THEN 0 ELSE SUM(CASE WHEN non.Dcf = 1 THEN non.Tonnage ELSE 0 END) END)
 FROM [Lookup].WeeeCategory w, [AATF].[NonObligatedWeee] non 
 INNER JOIN #FinalReturns fr ON non.ReturnId = fr.Id 
 WHERE w.Id = non.CategoryId
 GROUP BY w.ID, fr.[Quarter], w.Name

 -- Insert year data
 INSERT INTO #Results
 SELECT @ComplianceYear,
 CONCAT(w.Id, '. ', w.Name), 
 CONVERT(decimal(28,3), CASE WHEN SUM(CASE WHEN non.Dcf = 0 THEN non.Tonnage ELSE 0 END) IS NULL THEN 0 ELSE SUM(CASE WHEN non.Dcf = 0 THEN non.Tonnage ELSE 0 END) END), 
 CONVERT(decimal(28,3), CASE WHEN SUM(CASE WHEN non.Dcf = 1 THEN non.Tonnage ELSE 0 END) IS NULL THEN 0 ELSE SUM(CASE WHEN non.Dcf = 1 THEN non.Tonnage ELSE 0 END) END)
 FROM [Lookup].WeeeCategory w, [AATF].[NonObligatedWeee] non 
 INNER JOIN #FinalReturns fr ON non.ReturnId = fr.Id 
 WHERE w.Id = non.CategoryId
 GROUP BY  W.ID, fr.[Year], w.Name

 
SELECT * from #Results

DROP TABLE #Results
DROP TABLE #Returns
DROP TABLE #FinalReturns
END