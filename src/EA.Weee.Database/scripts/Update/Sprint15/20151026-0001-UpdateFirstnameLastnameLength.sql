Update [Identity].[AspNetUsers] set FirstName =  left(FirstName, 35), Surname = left(Surname, 35)

ALTER TABLE [Identity].[AspNetUsers]
  ALTER COLUMN FirstName NVARCHAR (35) NOT NULL

ALTER TABLE [Identity].[AspNetUsers]
  ALTER COLUMN Surname NVARCHAR (35) NOT NULL