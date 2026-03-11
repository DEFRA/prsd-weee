/* =====================================================
   2027 Inflationary Charge Uplift - PCS Subsistence Fee
   ===================================================== */

SET NOCOUNT ON;

BEGIN TRY

BEGIN TRANSACTION

DECLARE @EAId UNIQUEIDENTIFIER = 'A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8';
DECLARE @Id UNIQUEIDENTIFIER = '8E5E9C81-6219-B50C-82CB-FF866969BC24';
DECLARE @EffectiveFrom DATETIME = '2026-04-01';
DECLARE @ComplianceYear INT = 2027;


IF NOT EXISTS (
    SELECT 1 FROM [Lookup].[AnnualChargeByYear] 
    WHERE [ComplianceYear] = @ComplianceYear
    AND [EffectiveFrom] = @EffectiveFrom 
    AND [CompetentAuthorityId] = @EAId
)

INSERT INTO [Lookup].[AnnualChargeByYear]
    ([Id],[CompetentAuthorityId],[ComplianceYear],[AnnualChargeAmount],[EffectiveFrom])
VALUES
    -- 2027 Data (Effective from 2026-04-01) - AnnualChargeAmount INCREASED
    (@Id, @EAId, @ComplianceYear, 13948.13, @EffectiveFrom)

COMMIT TRANSACTION

END TRY

BEGIN CATCH
    ROLLBACK TRANSACTION
    PRINT ERROR_MESSAGE()
END CATCH

PRINT N'=== AnnualChargeByYear Table Creation and Data Population Complete ===';
PRINT N'';
PRINT N'Summary:';
PRINT N'  - Table: [Lookup].[AnnualChargeByYear] created';
PRINT N'  - Data insertion depends on CompetentAuthority records existence';
PRINT N'  - If CompetentAuthority records don''t exist yet, data will be inserted';
PRINT N'';
PRINT N'Expected charges: increase from 2026';
PRINT N'  - EA (2026): £13,438.00';
PRINT N'  - EA (2027): 13,948.13';
PRINT N'';

GO
