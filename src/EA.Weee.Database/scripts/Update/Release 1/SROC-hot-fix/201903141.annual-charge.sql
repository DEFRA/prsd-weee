

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
