BEGIN TRANSACTION

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'HasAnnualCharge'
          AND Object_ID = Object_ID(N'[PCS].[MemberUpload]'))
BEGIN
    ALTER TABLE [PCS].[MemberUpload]
	ADD HasAnnualCharge bit NOT NULL DEFAULT 0
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'AnnualChargeAmount'
          AND Object_ID = Object_ID(N'[Lookup].[CompetentAuthority]'))
BEGIN
    ALTER TABLE [Lookup].[CompetentAuthority]	
	ADD AnnualChargeAmount [decimal](18, 2)
END
GO

UPDATE [Lookup].[CompetentAuthority]
SET AnnualChargeAmount = 12500.00
WHERE Id = 'A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8'

COMMIT TRANSACTION