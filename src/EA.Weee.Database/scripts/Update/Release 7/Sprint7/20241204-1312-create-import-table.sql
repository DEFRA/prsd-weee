IF OBJECT_ID('WEEERegistrations', 'U') IS NOT NULL
    DROP TABLE WEEERegistrations;

CREATE TABLE WEEERegistrations (
    Name_in_WEEE nvarchar(255),
    PRN_NPWD nvarchar(255),
    Company_name_NPWD nvarchar(255),
    Trading_name nvarchar(255),
    Companies_house_number_NPWD nvarchar(255),
    Company_type_NPWD nvarchar(255)
);
