-- Add new Email column
ALTER TABLE [Lookup].[CompetentAuthority]
ADD [Email] NVARCHAR(255) NULL
GO

-- Populate the column with email addresses
UPDATE [Lookup].[CompetentAuthority]
SET [Email] =	CASE [Id]
				WHEN 'A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8' THEN 'weee@environment-agency.gov.uk' 
				WHEN '78F37814-364B-4FAE-BEB5-DB0439CBF177' THEN 'producer.responsibility@sepa.org.uk'
				WHEN '4EEE5942-01B2-4A4D-855A-34DEE1BBBF26' THEN 'weee@doeni.gov.uk'
				WHEN '44C2F368-AA66-48F0-BBC9-A0ED34AD0951' THEN 'weee@naturalresourceswales.gov.uk'
				ELSE [Email]
				END
GO

-- Set Email as not nullable
ALTER TABLE [Lookup].[CompetentAuthority]
ALTER COLUMN [Email] NVARCHAR(255) NOT NULL
GO
