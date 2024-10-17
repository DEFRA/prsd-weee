CREATE FUNCTION dbo.GetObligationType
(
    @EeeOutputReturnVersionId UNIQUEIDENTIFIER
)
RETURNS NVARCHAR(10)
AS
BEGIN
    DECLARE @ObligationType NVARCHAR(10)

    IF EXISTS (
        SELECT 1
        FROM (
            SELECT COUNT(DISTINCT eoa.ObligationType) AS DistinctCount
            FROM [PCS].[EeeOutputReturnVersionAmount] eorva
            INNER JOIN [PCS].[EeeOutputAmount] eoa ON eoa.Id = eorva.EeeOuputAmountId
            WHERE eorva.EeeOutputReturnVersionId = @EeeOutputReturnVersionId
        ) AS CountQuery
        WHERE CountQuery.DistinctCount = 2
    )
    SET @ObligationType = 'Both'
    ELSE IF EXISTS (
        SELECT 1
        FROM [PCS].[EeeOutputReturnVersionAmount] eorva
        INNER JOIN [PCS].[EeeOutputAmount] eoa ON eoa.Id = eorva.EeeOuputAmountId
        WHERE eorva.EeeOutputReturnVersionId = @EeeOutputReturnVersionId AND eoa.ObligationType = 'B2B'
    )
    SET @ObligationType = 'B2B'
    ELSE IF EXISTS (
        SELECT 1
        FROM [PCS].[EeeOutputReturnVersionAmount] eorva
        INNER JOIN [PCS].[EeeOutputAmount] eoa ON eoa.Id = eorva.EeeOuputAmountId
        WHERE eorva.EeeOutputReturnVersionId = @EeeOutputReturnVersionId AND eoa.ObligationType = 'B2C'
    )
    SET @ObligationType = 'B2C'
    ELSE
    SET @ObligationType = 'Unknown'

    RETURN @ObligationType
END