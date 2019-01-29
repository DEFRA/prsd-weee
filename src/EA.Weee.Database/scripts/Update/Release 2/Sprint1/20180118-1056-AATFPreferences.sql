GO
PRINT N'Creating [AATF].[ReportOnQuestion]...';




GO
CREATE TABLE [AATF].[ReportOnQuestion] (
    [Id]                          INT NOT NULL,
	[Question]					  NVARCHAR (300) NOT NULL,
	[Description]				  NVARCHAR (1000) NOT NULL,
	[ParentId]					  INT


    CONSTRAINT [PK_ReportOnQuestion_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
PRINT N'Adding questions to [AATF].[ReportOnQuestion]...';




GO
INSERT [AATF].[ReportOnQuestion]
VALUES
('1', 'PCS', 'Obligated WEEE received on behalf of producer compliance schemes (PCSs)', NULL),
('2', 'SentToAnotherATF', 'WEEE sent to another AATF or ATF for treatment', NULL),
('3', 'WEEEReused', 'WEEE reused as a whole appliance', NULL),
('4', 'NonObligated', 'Non-obligated WEEE', NULL),
('5', 'NonObligatedDCF', 'Was any of this non-obligated WEEE received on behalf of a DCF', '4');




GO
PRINT N'Creating [AATF].[AATFReturnReportOn]...';




GO
CREATE TABLE [AATF].[AATFReturnReportOn] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
	[AATFReturnId]				  UNIQUEIDENTIFIER NOT NULL,
	[ReportOnQuestionId]		  INT NOT NULL


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
