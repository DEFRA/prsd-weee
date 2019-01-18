PRINT N'Creating [AATF]...';


GO
CREATE SCHEMA [AATF]
    AUTHORIZATION [dbo];


GO
PRINT N'Creating [AATF].[Operator]...';


GO
CREATE TABLE [AATF].[Operator] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
    [OrganisationId]              UNIQUEIDENTIFIER NOT NULL

    CONSTRAINT [PK_Operator_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);



GO
PRINT N'Creating FK_Operator_Organisation...';



GO
ALTER TABLE [AATF].[Operator] WITH NOCHECK
    ADD CONSTRAINT [FK_Operator_Organisation] FOREIGN KEY ([OrganisationId]) REFERENCES [Organisation].[Organisation] ([Id]);



GO
PRINT N'Creating [AATF].[Return]...';



GO
CREATE TABLE [AATF].[Return] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
    [OperatorId]				  UNIQUEIDENTIFIER NOT NULL,
	[ComplianceYear]			  INT NOT NULL,	
	[Period]					  INT NOT NULL,
	[Status]					  INT NOT NULL

    CONSTRAINT [PK_Return_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);



GO
PRINT N'Creating FK_Return_Operator...';



GO
ALTER TABLE [AATF].[Return] WITH NOCHECK
    ADD CONSTRAINT [FK_Return_Operator] FOREIGN KEY ([OperatorId]) REFERENCES [AATF].[Operator] ([Id]);




GO
PRINT N'Creating [AATF].[NonObligatedWeee]...';



GO
CREATE TABLE [AATF].[NonObligatedWeee] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
    [ReturnId]				      UNIQUEIDENTIFIER NOT NULL,
	[CategoryId]				  UNIQUEIDENTIFIER NOT NULL,	
	[Dcf]					      BIT NOT NULL,
	[Tonnage]					  DECIMAL(28, 3)

    CONSTRAINT [PK_NonObligatedWeee_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);



GO
PRINT N'Creating FK_NonObligatedWeee_Return...';



GO
ALTER TABLE [AATF].[NonObligatedWeee] WITH NOCHECK
    ADD CONSTRAINT [FK_NonObligatedWeee_Return] FOREIGN KEY ([ReturnId]) REFERENCES [AATF].[Return] ([Id]);




GO
PRINT N'Creating [AATF].[AATF]...';



GO
CREATE TABLE [AATF].[AATF] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
    [Name]						  NVARCHAR (50) NOT NULL,
	[CompetentAuthorityId]		  UNIQUEIDENTIFIER NOT NULL,	
	[ApprovalNumber]			  NCHAR (10) NOT NULL,
	[Status]					  NCHAR (10) NOT NULL

    CONSTRAINT [PK_AATF_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
PRINT N'Creating [AATF].[NonObligatedWeee]...';



GO
CREATE TABLE [AATF].[OperatorAATF] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
    [OperatorId]				  UNIQUEIDENTIFIER NOT NULL,
	[AATFId]					  UNIQUEIDENTIFIER NOT NULL,	

    CONSTRAINT [PK_OperatorAATF_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [AATF].[AATFReturn]...';



GO
CREATE TABLE [AATF].[AATFReturn] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
    [ReturnId]				      UNIQUEIDENTIFIER NOT NULL,
	[AATFId]					  UNIQUEIDENTIFIER NOT NULL,	


    CONSTRAINT [PK_AATFReturn_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);



GO
PRINT N'Creating FK_AATFReturn_Return...';



GO
ALTER TABLE [AATF].[AATFReturn] WITH NOCHECK
    ADD CONSTRAINT [FK_AATFReturn_Return] FOREIGN KEY ([ReturnId]) REFERENCES [AATF].[Return] ([Id]);



GO
PRINT N'Creating FK_AATFReturn_AATF...';



GO
ALTER TABLE [AATF].[AATFReturn] WITH NOCHECK
    ADD CONSTRAINT [FK_AATFReturn_AATF] FOREIGN KEY ([AATFId]) REFERENCES [AATF].[AATF] ([Id]);



GO
PRINT N'Creating FK_OperatorAATF_AATFId...';



GO
ALTER TABLE [AATF].[OperatorAATF] WITH NOCHECK
    ADD CONSTRAINT [FK_OperatorAATF_AATFId] FOREIGN KEY ([AATFId]) REFERENCES [AATF].[AATF] ([Id]);



GO
PRINT N'Creating FK_OperatorAATF_OperatorId...';



GO
ALTER TABLE [AATF].[OperatorAATF] WITH NOCHECK
    ADD CONSTRAINT [FK_OperatorAATF_OperatorId] FOREIGN KEY ([OperatorId]) REFERENCES [AATF].[Operator] ([Id]);




GO
PRINT N'Creating [AATF].[WeeeReused]...';



GO
CREATE TABLE [AATF].[WeeeReused] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
    [AATFReturnId]				  UNIQUEIDENTIFIER NOT NULL,


    CONSTRAINT [PK_WeeeReused_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
PRINT N'Creating FK_WeeeReused_AATFReturn...';



GO
ALTER TABLE [AATF].[WeeeReused] WITH NOCHECK
    ADD CONSTRAINT [FK_WeeeReused_AATFReturn] FOREIGN KEY ([AATFReturnId]) REFERENCES [AATF].[AATFReturn] ([Id]);



GO
PRINT N'Creating [AATF].[WeeeReusedAmount]...';




GO
CREATE TABLE [AATF].[WeeeReusedAmount] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
    [WeeeReusedId]				  UNIQUEIDENTIFIER NOT NULL,
	[CategoryId]				  UNIQUEIDENTIFIER NOT NULL,
	[ObligationType]			  INT NOT NULL,
	[Tonnage]					  DECIMAL(28, 3)


    CONSTRAINT [PK_WeeeReusedAmount_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
PRINT N'Creating FK_WeeeReusedAmount_WeeeReused...';



GO
ALTER TABLE [AATF].[WeeeReusedAmount] WITH NOCHECK
    ADD CONSTRAINT [FK_WeeeReusedAmount_WeeeReused] FOREIGN KEY ([WeeeReusedId]) REFERENCES [AATF].[WeeeReused] ([Id]);




GO
PRINT N'Creating [AATF].[WeeeReusedSite]...';




GO
CREATE TABLE [AATF].[WeeeReusedSite] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
    [WeeeReusedId]				  UNIQUEIDENTIFIER NOT NULL


    CONSTRAINT [PK_WeeeReusedSite_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
PRINT N'Creating FK_WeeeReusedSite_WeeeReused...';



GO
ALTER TABLE [AATF].[WeeeReusedSite] WITH NOCHECK
    ADD CONSTRAINT [FK_WeeeReusedSite_WeeeReused] FOREIGN KEY ([WeeeReusedId]) REFERENCES [AATF].[WeeeReused] ([Id]);




GO
PRINT N'Creating [AATF].[WeeeSentOn]...';




GO
CREATE TABLE [AATF].[WeeeSentOn] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
    [AATFReturnId]				  UNIQUEIDENTIFIER NOT NULL,
	[SiteAddressId]				  UNIQUEIDENTIFIER NOT NULL


    CONSTRAINT [PK_WeeeSentOn_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
PRINT N'Creating FK_WeeeSentOn_AATFReturn...';



GO
ALTER TABLE [AATF].[WeeeSentOn] WITH NOCHECK
    ADD CONSTRAINT [FK_WeeeSentOn_AATFReturn] FOREIGN KEY ([AATFReturnId]) REFERENCES [AATF].[AATFReturn] ([Id]);




GO
PRINT N'Creating [AATF].[WeeeSentOnAmount]...';




GO
CREATE TABLE [AATF].[WeeeSentOnAmount] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
    [WeeeSentOnId]				  UNIQUEIDENTIFIER NOT NULL,
	[ObligationType]			  INT NOT NULL,
	[CategoryId]				  UNIQUEIDENTIFIER NOT NULL,
	[Tonnage]					  DECIMAL(28, 3)


    CONSTRAINT [PK_WeeeSentOnAmount_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
PRINT N'Creating FK_WeeeSentOnAmount_WeeeSentOn...';



GO
ALTER TABLE [AATF].[WeeeSentOnAmount] WITH NOCHECK
    ADD CONSTRAINT [FK_WeeeSentOnAmount_WeeeSentOn] FOREIGN KEY ([WeeeSentOnId]) REFERENCES [AATF].[WeeeSentOn] ([Id]);




GO
PRINT N'Creating [AATF].[WeeeReceived]...';




GO
CREATE TABLE [AATF].[WeeeReceived] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
	[SchemeId]					  UNIQUEIDENTIFIER NOT NULL,
	[AATFReturnId]				  UNIQUEIDENTIFIER NOT NULL


    CONSTRAINT [PK_WeeeReceived_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
PRINT N'Creating FK_WeeeReceived_AATFReturn...';



GO
ALTER TABLE [AATF].[WeeeReceived] WITH NOCHECK
    ADD CONSTRAINT [FK_WeeeReceived_AATFReturn] FOREIGN KEY ([AATFReturnId]) REFERENCES [AATF].[AATFReturn] ([Id]);




GO
PRINT N'Creating FK_WeeeReceived_Scheme...';



GO
ALTER TABLE [AATF].[WeeeReceived] WITH NOCHECK
    ADD CONSTRAINT [FK_WeeeReceived_Scheme] FOREIGN KEY ([SchemeId]) REFERENCES [PCS].[Scheme] ([Id]);




GO
PRINT N'Creating [AATF].[WeeeReceivedAmount]...';




GO
CREATE TABLE [AATF].[WeeeReceivedAmount] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
	[WeeeReceivedId]			  UNIQUEIDENTIFIER NOT NULL,
	[ObligationType]			  INT NOT NULL,
	[CategoryId]				  UNIQUEIDENTIFIER NOT NULL,
	[Tonnage]					  DECIMAL(28, 3)


    CONSTRAINT [PK_WeeeReceivedAmount_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
PRINT N'Creating FK_WeeeReceived_WeeeReceived...';



GO
ALTER TABLE [AATF].[WeeeReceivedAmount] WITH NOCHECK
    ADD CONSTRAINT [FK_WeeeReceivedAmount_AATFReturn] FOREIGN KEY ([WeeeReceivedId]) REFERENCES [AATF].[WeeeReceived] ([Id]);