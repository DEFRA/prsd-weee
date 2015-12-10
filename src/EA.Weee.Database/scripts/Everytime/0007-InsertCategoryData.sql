GO
PRINT N'Altering [Lookup].[Category]...';

DECLARE @tblTempCategoryTable TABLE (
[Id]            UNIQUEIDENTIFIER NOT NULL,
[Name]          NVARCHAR(255) NOT NULL
)

INSERT INTO @tblTempCategoryTable([Id], [Name])
VALUES
('9080C0D8-D164-44C3-A6D4-997CA7D66F84', 'Large Household Appliances'),
('5F0A9047-82D6-418A-A95D-68851F6C2D76', 'Small Household Appliances'),
('121975E8-D168-4A8F-802B-DC7F68551CF9', 'IT and Telecomms Equipment'),
('ACFD5B5D-AE31-46AC-93DE-853ECDFB35EB', 'Consumer Equipment'),
('568BA399-72FB-4897-BD1B-170BF7D78D26', 'Lighting Equipment'),
('91624FB0-E5D4-4BB4-86DD-365C67BD3A12', 'Electrical and Electronic Tools'),
('1F2BB2DA-4242-4F3C-9595-C67B10B13CDF', 'Toys Leisure and Sports'),
('EA26CE67-154A-409A-B80A-B03A5A5910D6', 'Medical Devices'),
('23497C88-2371-4C3E-8A94-681B4484CBF2', 'Monitoring and Control Instruments'),
('5B8C8B6D-888A-49A5-995A-5D3C660FB0D7', 'Automatic Dispensers'),
('4D13450B-80E9-45AB-85D3-6AD662C9387D', 'Display Equipment'),
('29FD4480-7385-4A6A-AE38-1AA47A5F555B', 'Cooling Appliances Containing Refrigerants'),
('1552F07A-2F0A-4276-9D49-C42A24FAA2BF', 'Gas Discharge Lamps and LED light sources'),
('0F0A5653-24A7-4DE0-BE0A-5EF0FC81D11E', 'Photovoltaic Panels');

INSERT INTO [Lookup].[Category]([Id], [Name])
SELECT tmp.[Id], tmp.[Name]
FROM @tblTempCategoryTable tmp
LEFT JOIN [Lookup].[Category] tbl ON tbl.[Id] = tmp.[Id]
WHERE tbl.[Id] IS NULL

GO
PRINT N'Update complete.';

GO