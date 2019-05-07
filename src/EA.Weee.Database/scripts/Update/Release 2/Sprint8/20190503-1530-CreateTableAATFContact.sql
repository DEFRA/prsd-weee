CREATE TABLE [AATF].[Contact](
	[Id] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](35) NOT NULL,
	[LastName] [nvarchar](35) NOT NULL,
	[AddressId] [uniqueidentifier] NOT NULL,
	[Position] [nvarchar](35) NOT NULL,
	[Telephone] [nvarchar](20) NOT NULL,
	[Email] [nvarchar](256) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Contact_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

ALTER TABLE [AATF].[Contact] ADD CONSTRAINT FK_Contact_Address_AddressId FOREIGN KEY (AddressId) REFERENCES [AATF].[Address](Id);