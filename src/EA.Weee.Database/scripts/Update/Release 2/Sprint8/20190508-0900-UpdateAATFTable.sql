BEGIN TRANSACTION
GO

ALTER TABLE [AATF].AATF ADD ContactId uniqueidentifier NULL;
ALTER TABLE [AATF].AATF ADD CONSTRAINT FK_Aatf_Contact_ContactId FOREIGN KEY (ContactId) REFERENCES [AATF].[Contact](Id);
GO

DECLARE @defaultContactId uniqueidentifier = NEWID();

INSERT INTO [AATF].[Contact]
           ([Id]
           ,[FirstName]
           ,[LastName]
           ,[Position]
           ,[Address1]
           ,[Address2]
           ,[TownOrCity]
           ,[CountyOrRegion]
           ,[Postcode]
           ,[CountryId]
           ,[Telephone]
           ,[Email])
     VALUES
           (@defaultContactId
           ,'First Name'
           ,'Last Name'
           ,'Director'
           ,'1 Address Lane'
           ,'Address Ward'
           ,'Town'
           ,'County'
           ,'PO5T COD3'
           ,(SELECT ID FROM Lookup.Country WHERE Name = 'UK - England')
           ,'0123456789'
           ,'email@email.com')

UPDATE [AATF].AATF SET ContactId = @defaultContactId;

ALTER TABLE [AATF].AATF ALTER COLUMN ContactId uniqueidentifier NOT NULL;
GO

COMMIT TRANSACTION