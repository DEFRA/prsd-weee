GO
PRINT N'Creating [Notification].[Exporter]...';


GO
CREATE TABLE [Notification].[Exporter](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
	[Type] [nvarchar](64) NOT NULL,
	[RegistrationNumber1] [nvarchar](64) NULL,
	[RegistrationNumber2] [nvarchar](64) NULL,
	[CompanyHouseNumber] [nvarchar](64) NULL,
	[AddressId] [uniqueidentifier] NULL,
	[ContactId] [uniqueidentifier] NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Exporter] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Notification].[Exporter]  WITH CHECK ADD  CONSTRAINT [FK_Exporter_Address] FOREIGN KEY([AddressId])
REFERENCES [Business].[Address] ([Id])
GO

ALTER TABLE [Notification].[Exporter] CHECK CONSTRAINT [FK_Exporter_Address]
GO

ALTER TABLE [Notification].[Exporter]  WITH CHECK ADD  CONSTRAINT [FK_Exporter_Contact] FOREIGN KEY([ContactId])
REFERENCES [Business].[Contact] ([Id])
GO

ALTER TABLE [Notification].[Exporter] CHECK CONSTRAINT [FK_Exporter_Contact]
GO


GO
PRINT N'Update complete.';

GO