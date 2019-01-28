/****** Object:  Table [Auditing].[AuditLog]    Script Date: 10/08/2015 21:37:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Auditing].[AuditEvent](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EventDate] [datetime2](7) NOT NULL,
	[Scope] [nvarchar](256) NOT NULL,
	[EventId] [int] NOT NULL,
	[EventName] [nvarchar](256) NOT NULL,
	[UserId] [nvarchar](256) NULL,
	[Data] [nvarchar](max) NULL,
 CONSTRAINT [PK_AuditEvent] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO