GO
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'DirectRegistrantCharge' AND type = 'U')
BEGIN
    CREATE TABLE [Lookup].[DirectRegistrantCharge] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL,
    [ComplianceYear]    INT                 NOT NULL,
    [EffectiveFrom]     DATE                NOT NULL,
    [ChargeAmount]      DECIMAL(10,2)       NOT NULL);
    PRINT 'Table "DirectRegistrantCharge" created successfully.';
END
ELSE
BEGIN
    PRINT 'Table "DirectRegistrantCharge" already exists.';
END

GO
INSERT INTO [Lookup].[DirectRegistrantCharge]
            ([Id]
            ,[ComplianceYear]
            ,[EffectiveFrom]
            ,[ChargeAmount])
     VALUES
           (NEWID()
           ,2025
           ,'01-01-2025',
           '30.00');

INSERT INTO [Lookup].[DirectRegistrantCharge]
            ([Id]
            ,[ComplianceYear]
            ,[EffectiveFrom]
            ,[ChargeAmount])
    VALUES
           (NEWID()
           ,2026
           ,'01-01-2026',
           '32.00');