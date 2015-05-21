

GO
PRINT N'Creating [Notification]...';


GO
CREATE SCHEMA [Notification]
    AUTHORIZATION [dbo];


GO
PRINT N'Creating [Notification].[EaNotificationNumber]...';


GO
CREATE SEQUENCE [Notification].[EaNotificationNumber]
    AS INT
    INCREMENT BY 1
    MINVALUE 5000
    MAXVALUE 999999
    NO CACHE;


GO
PRINT N'Creating [Notification].[NieaNotificationNumber]...';


GO
CREATE SEQUENCE [Notification].[NieaNotificationNumber]
    AS INT
    INCREMENT BY 1
    MINVALUE 1000
    MAXVALUE 999999
    NO CACHE;


GO
PRINT N'Creating [Notification].[NrwNotificationNumber]...';


GO
CREATE SEQUENCE [Notification].[NrwNotificationNumber]
    AS INT
    INCREMENT BY 1
    MINVALUE 100
    MAXVALUE 999999
    NO CACHE;


GO
PRINT N'Creating [Notification].[SepaNotificationNumber]...';


GO
CREATE SEQUENCE [Notification].[SepaNotificationNumber]
    AS INT
    INCREMENT BY 1
    MINVALUE 500
    MAXVALUE 999999
    NO CACHE;


GO
PRINT N'Update complete.';


GO
