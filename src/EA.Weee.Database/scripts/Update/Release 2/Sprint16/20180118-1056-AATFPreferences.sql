GO
PRINT N'Creating [AATF].[ReportOnQuestion]...';




GO
CREATE TABLE [AATF].[ReportOnQuestion] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
	[Question]					  NVARCHAR (300) NOT NULL,
	[Description]				  NVARCHAR (300) NOT NULL,
	[ParentId]					  INT NOT NULL


    CONSTRAINT [PK_ReportOnQuestion_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
PRINT N'Creating [AATF].[AATFReturnReportOn]...';




GO
CREATE TABLE [AATF].[AATFReturnReportOn] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
	[AATFReturnId]				  UNIQUEIDENTIFIER NOT NULL,
	[ReportOnQuestionId]		  UNIQUEIDENTIFIER NOT NULL


    CONSTRAINT [PK_AATFReturnReportOn_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);



GO
PRINT N'Creating FK_AATFReturnReportOn_ReportOnQuestion...';



GO
ALTER TABLE [AATF].[AATFReturnReportOn] WITH NOCHECK
    ADD CONSTRAINT [FK_AATFReturnReportOn_ReportOnQuestion] FOREIGN KEY ([ReportOnQuestionId]) REFERENCES [AATF].[ReportOnQuestion] ([Id]);



GO
PRINT N'Creating FK_AATFReturnReportOn_AATFReturn...';



GO
ALTER TABLE [AATF].[AATFReturnReportOn] WITH NOCHECK
    ADD CONSTRAINT [FK_AATFReturnReportOn_AATFReturn] FOREIGN KEY ([AATFReturnId]) REFERENCES [AATF].[AATFReturn] ([Id]);




GO
PRINT N'Creating [AATF].[AATFReturnScheme]...';


GO
CREATE TABLE [AATF].[AATFReturnScheme] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
    [SchemeId]					  UNIQUEIDENTIFIER NOT NULL,
	[AATFReturnId]				  UNIQUEIDENTIFIER NOT NULL

    CONSTRAINT [PK_AATFReturnScheme_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);



GO
PRINT N'Creating FK_AATFReturn_Scheme...';



GO
ALTER TABLE [AATF].[AATFReturnScheme] WITH NOCHECK
    ADD CONSTRAINT [FK_AATFReturnScheme_Scheme] FOREIGN KEY ([SchemeId]) REFERENCES [PCS].[SCHEME] ([Id]);


	GO
PRINT N'Creating FK_AATFReturn_AATFReturn...';



GO
ALTER TABLE [AATF].[AATFReturnScheme] WITH NOCHECK
    ADD CONSTRAINT [FK_AATFReturnScheme_AATFReturn] FOREIGN KEY ([AATFReturnId]) REFERENCES [AATF].[AATFReturn] ([Id]);
