IF OBJECT_ID('[AATF].getReturnObligatedCsvData', 'P') IS NOT NULL BEGIN
	DROP PROCEDURE [AATF].[getReturnObligatedCsvData]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Description:	This stored procedure is used to provide the data for the admin report of obligatde data
--				that have/haven't submitted a data return within
--				the limits of the specified parameters.Get the latest submitted return

-- =============================================
CREATE PROCEDURE [AATF].[getReturnObligatedCsvData]
	@ReturnId UNIQUEIDENTIFIER
AS
BEGIN

SET NOCOUNT ON;




END