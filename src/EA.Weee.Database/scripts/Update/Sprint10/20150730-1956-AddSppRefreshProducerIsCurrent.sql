﻿GO
PRINT N'Creating [Producer].[sppRefreshProducerIsCurrent]...';

GO
-- =============================================
-- Author:		Graham Alexander-Thomson; gt@main.sfwltd.co.uk
-- Create date: 2015-07-30T18:21:00+08:00
-- Description:	Updates the [IsCurrentForComplianceYear] column
--				for all rows in [Producer].[Producer].
-- =============================================
CREATE PROCEDURE [Producer].[sppRefreshProducerIsCurrent]
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE
		[Producer].[Producer]
	SET
		[IsCurrentForComplianceYear] =
			CASE X.[RowNumber]
				WHEN 1 THEN 1
				ELSE 0
			END
	FROM
		[Producer].[Producer] P
	LEFT JOIN
	(
		SELECT
			P.[Id],
			ROW_NUMBER() OVER
			(
				PARTITION BY
					MU.[ComplianceYear],
					P.[SchemeId],
					P.[RegistrationNumber]
				ORDER BY
					P.[LastSubmitted] DESC
			) AS RowNumber
		FROM
			[Producer].[Producer] P
		INNER JOIN
			[PCS].[MemberUpload] MU
				ON P.[MemberUploadId] = MU.[Id]
		WHERE
			MU.[IsSubmitted] = 1
	) X
		ON P.[Id] = X.[Id]
END

GO    
PRINT N'Executing [Producer].[sppRefreshProducerIsCurrent]...';

GO
EXEC [Producer].[sppRefreshProducerIsCurrent]

GO
PRINT N'Update complete.';

GO
