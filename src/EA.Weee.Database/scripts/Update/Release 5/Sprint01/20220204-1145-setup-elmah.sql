 
/****** Object:  Schema [Logging]    Script Date: 03/02/2022 15:07:58 ******/
CREATE SCHEMA [Logging]
GO


/****** Object:  Synonym [dbo].[ELMAH_Error]    Script Date: 03/02/2022 15:10:19 ******/
CREATE SYNONYM [dbo].[ELMAH_Error] FOR [Logging].[ELMAH_Error]
GO


CREATE SYNONYM [dbo].[ELMAH_GetErrorsXml] FOR [Logging].[ELMAH_GetErrorsXml]
GO

CREATE SYNONYM [dbo].[ELMAH_GetErrorXml] FOR [Logging].[ELMAH_GetErrorXml]
GO

/****** Object:  Synonym [dbo].[ELMAH_LogError]    Script Date: 03/02/2022 15:10:52 ******/
CREATE SYNONYM [dbo].[ELMAH_LogError] FOR [Logging].[ELMAH_LogError]
GO

/****** Object:  Table [Logging].[ELMAH_Error]    Script Date: 03/02/2022 15:05:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Logging].[ELMAH_Error](
	[ErrorId] [uniqueidentifier] NOT NULL,
	[Application] [nvarchar](60) NOT NULL,
	[Host] [nvarchar](50) NOT NULL,
	[Type] [nvarchar](100) NOT NULL,
	[Source] [nvarchar](60) NOT NULL,
	[Message] [nvarchar](500) NOT NULL,
	[User] [nvarchar](50) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[TimeUtc] [datetime] NOT NULL,
	[Sequence] [int] IDENTITY(1,1) NOT NULL,
	[AllXml] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ELMAH_Error] PRIMARY KEY NONCLUSTERED 
(
	[ErrorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [Logging].[ELMAH_Error] ADD  CONSTRAINT [DF_ELMAH_Error_ErrorId]  DEFAULT (newid()) FOR [ErrorId]
GO


/****** Object:  StoredProcedure [Logging].[ELMAH_GetErrorsXml]    Script Date: 04/02/2022 11:03:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Logging].[ELMAH_GetErrorsXml]
(
    @Application NVARCHAR(60),
    @PageIndex INT = 0,
    @PageSize INT = 15,
    @TotalCount INT OUTPUT
)
AS
BEGIN

    SET NOCOUNT ON

    DECLARE @FirstTimeUTC DATETIME
    DECLARE @FirstSequence INT
    DECLARE @StartRow INT
    DECLARE @StartRowIndex INT

    SELECT 
        @TotalCount = COUNT(1) 
    FROM 
        [Logging].[ELMAH_Error]
    WHERE 
        [Application] = @Application

    -- Get the ID of the first error for the requested page

    SET @StartRowIndex = @PageIndex * @PageSize + 1

    IF @StartRowIndex <= @TotalCount
    BEGIN

        SET ROWCOUNT @StartRowIndex

        SELECT  
            @FirstTimeUTC = [TimeUtc],
            @FirstSequence = [Sequence]
        FROM 
            [Logging].[ELMAH_Error]
        WHERE   
            [Application] = @Application
        ORDER BY 
            [TimeUtc] DESC, 
            [Sequence] DESC

    END
    ELSE
    BEGIN

        SET @PageSize = 0

    END

    -- Now set the row count to the requested page size and get
    -- all records below it for the pertaining application.

    SET ROWCOUNT @PageSize

    SELECT 
        errorId     = [ErrorId], 
        application = [Application],
        host        = [Host], 
        type        = [Type],
        source      = [Source],
        message     = [Message],
        [user]      = [User],
        statusCode  = [StatusCode], 
        time        = CONVERT(VARCHAR(50), [TimeUtc], 126) + 'Z'
    FROM 
        [Logging].[ELMAH_Error] error
    WHERE
        [Application] = @Application
    AND
        [TimeUtc] <= @FirstTimeUTC
    AND 
        [Sequence] <= @FirstSequence
    ORDER BY
        [TimeUtc] DESC, 
        [Sequence] DESC
    FOR
        XML AUTO

END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Logging].[ELMAH_GetErrorXml]
(
    @Application NVARCHAR(60),
    @ErrorId UNIQUEIDENTIFIER
)
AS
BEGIN

    SET NOCOUNT ON

    SELECT 
        [AllXml]
    FROM 
        [Logging].[ELMAH_Error]
    WHERE
        [ErrorId] = @ErrorId
    AND
        [Application] = @Application

END
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Logging].[ELMAH_LogError]
(
    @ErrorId UNIQUEIDENTIFIER,
    @Application NVARCHAR(60),
    @Host NVARCHAR(30),
    @Type NVARCHAR(100),
    @Source NVARCHAR(60),
    @Message NVARCHAR(500),
    @User NVARCHAR(50),
    @AllXml NVARCHAR(MAX),
    @StatusCode INT,
    @TimeUtc DATETIME
)
AS
BEGIN

    SET NOCOUNT ON

    INSERT
    INTO
        [Logging].[ELMAH_Error]
        (
            [ErrorId],
            [Application],
            [Host],
            [Type],
            [Source],
            [Message],
            [User],
            [AllXml],
            [StatusCode],
            [TimeUtc]
        )
    VALUES
        (
            @ErrorId,
            @Application,
            @Host,
            @Type,
            @Source,
            @Message,
            @User,
            @AllXml,
            @StatusCode,
            @TimeUtc
        )

END
GO


IF NOT EXISTS (SELECT [Name] FROM sys.synonyms WHERE [Name] = 'ELMAH_Error')
    CREATE SYNONYM [ELMAH_Error] FOR [Logging].[ELMAH_Error];
GO
IF NOT EXISTS (SELECT [Name] FROM sys.synonyms WHERE [Name] = 'ELMAH_GetErrorsXml')
    CREATE SYNONYM [ELMAH_GetErrorsXml] FOR [Logging].[ELMAH_GetErrorsXml];
GO
IF NOT EXISTS (SELECT [Name] FROM sys.synonyms WHERE [Name] = 'ELMAH_GetErrorXml')
    CREATE SYNONYM [ELMAH_GetErrorXml] FOR [Logging].[ELMAH_GetErrorXml];
GO
IF NOT EXISTS (SELECT [Name] FROM sys.synonyms WHERE [Name] = 'ELMAH_LogError')
    CREATE SYNONYM [ELMAH_LogError] FOR [Logging].[ELMAH_LogError];

GO
