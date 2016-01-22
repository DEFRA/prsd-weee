IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority')
DROP PROCEDURE Producer.spgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority;
GO