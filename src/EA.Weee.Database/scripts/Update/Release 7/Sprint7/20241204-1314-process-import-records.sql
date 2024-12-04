IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.WEEERegistrations') AND name = 'Error')
BEGIN
    ALTER TABLE [dbo].[WEEERegistrations] ADD Error NVARCHAR(MAX)
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.WEEERegistrations') AND name = 'OrganisationId')
BEGIN
    ALTER TABLE [dbo].[WEEERegistrations] ADD OrganisationId UNIQUEIDENTIFIER
END
GO
BEGIN TRANSACTION

-- Declare variables

DECLARE @Name_in_WEEE nvarchar(255),
        @PRN_NPWD nvarchar(255),
        @Company_name_NPWD nvarchar(255),
        @Trading_name nvarchar(255),
        @Companies_house_number_NPWD nvarchar(255),
        @Company_type_NPWD nvarchar(255),
        @newOrganisationId uniqueidentifier,
        @CompanyType int = NULL;

DECLARE WEEECursor CURSOR FOR 
SELECT Name_in_WEEE,
       NULLIF(NULLIF(PRN_NPWD, ''), ' '),
       NULLIF(NULLIF(Company_name_NPWD, ''), ' '),
       NULLIF(NULLIF(Trading_name, ''), ' '),
       NULLIF(NULLIF(Companies_house_number_NPWD, ''), ' '),
       Company_type_NPWD
FROM WEEERegistrations;

OPEN WEEECursor;
FETCH NEXT FROM WEEECursor INTO 
    @Name_in_WEEE, @PRN_NPWD, @Company_name_NPWD, 
    @Trading_name, @Companies_house_number_NPWD, @Company_type_NPWD;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @newOrganisationId = NEWID();
    SET @CompanyType = CASE @Company_type_NPWD
        WHEN 'Individual Company' THEN 3
        WHEN 'Sole Trader' THEN 3
        WHEN 'Partnership' THEN 2
        ELSE 1
    END;

    -- Set company registration number to NULL if length <= 4
    IF LEN(@Companies_house_number_NPWD) <= 4 OR LEN(@Companies_house_number_NPWD) > 30
    BEGIN
        SET @Companies_house_number_NPWD = NULL;
    END

    BEGIN TRY
        -- Clear any previous error
        UPDATE WEEERegistrations 
        SET Error = NULL
        WHERE PRN_NPWD = @PRN_NPWD;

        INSERT INTO [Organisation].Organisation (
            Id, [Name], OrganisationType, 
            OrganisationStatus, TradingName, IsRepresentingCompany, CompanyRegistrationNumber, NPWDMigrated
        )
        VALUES (
            @newOrganisationId, 
            @Company_name_NPWD,
            @CompanyType,
            2, 
            @Trading_name,
            0,
            @Companies_house_number_NPWD,
            1
        );
		
		INSERT INTO [Producer].DirectRegistrant (Id, OrganisationId, ProducerRegistrationNumber)
		values (NEWID(), @newOrganisationId, @PRN_NPWD)

        UPDATE WEEERegistrations 
        SET OrganisationId = @newOrganisationId
        WHERE PRN_NPWD = @PRN_NPWD;
    END TRY
    BEGIN CATCH
        UPDATE WEEERegistrations 
        SET Error = ERROR_MESSAGE()
        WHERE PRN_NPWD = @PRN_NPWD;

        PRINT 'Error inserting organisation for PRN_NPWD: ' + @PRN_NPWD + 
              ' Error: ' + ERROR_MESSAGE();
    END CATCH

    FETCH NEXT FROM WEEECursor INTO 
        @Name_in_WEEE, @PRN_NPWD, @Company_name_NPWD,
        @Trading_name, @Companies_house_number_NPWD, @Company_type_NPWD;
END;

CLOSE WEEECursor;
DEALLOCATE WEEECursor;

-- Verify results
--SELECT * FROM [Organisation].Organisation;
--SELECT * FROM WEEERegistrations WHERE OrganisationId IS NULL AND Error IS NOT NULL;

COMMIT TRANSACTION;