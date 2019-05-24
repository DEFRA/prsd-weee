ALTER TABLE [Organisation].Address ALTER COLUMN Address1 nvarchar(60) NOT NULL;
ALTER TABLE [Organisation].Address ALTER COLUMN Address2 nvarchar(60);

ALTER TABLE [AATF].Address ALTER COLUMN Address1 nvarchar(60) NOT NULL;
ALTER TABLE [AATF].Address ALTER COLUMN Address2 nvarchar(60);

ALTER TABLE [AATF].Contact ALTER COLUMN Address1 nvarchar(60) NOT NULL;
ALTER TABLE [AATF].Contact ALTER COLUMN Address2 nvarchar(60);