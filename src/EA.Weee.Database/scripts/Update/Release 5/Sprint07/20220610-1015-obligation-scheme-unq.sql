ALTER TABLE [Pcs].[ObligationScheme]
ADD CONSTRAINT UNQ_ObligationScheme_Scheme_ComplianceYear UNIQUE (ComplianceYear, SchemeId);
GO