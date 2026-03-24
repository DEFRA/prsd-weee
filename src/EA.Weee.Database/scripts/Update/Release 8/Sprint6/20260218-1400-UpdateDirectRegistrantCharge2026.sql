/*
 * Update Direct Registrant charge for 2026 (England + Non-UK)
 * Fee Change: £33.00 -> £33.48
 * Record ID: D24429EB-50A0-43AB-8D50-FDBC6085AE92
 */

UPDATE [Lookup].[DirectRegistrantCharge]
SET [ChargeAmount] = 33.48
WHERE [Id] = 'D24429EB-50A0-43AB-8D50-FDBC6085AE92'
  AND [ComplianceYear] = 2026
  AND [IsNonUk] = 1;
GO