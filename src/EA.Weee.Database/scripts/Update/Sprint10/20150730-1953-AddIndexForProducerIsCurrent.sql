GO
PRINT N'Creating [IX_Producer_IsCurrentOnly]...';

GO
CREATE NONCLUSTERED INDEX [IX_Producer_IsCurrentOnly] ON [Producer].[Producer] 
(
	[MemberUploadId] ASC,
	[SchemeId] ASC
)
INCLUDE
(
	[RegistrationNumber]
) 
WHERE
	[IsCurrentForComplianceYear] = 1

GO    
PRINT N'Update complete.';

GO