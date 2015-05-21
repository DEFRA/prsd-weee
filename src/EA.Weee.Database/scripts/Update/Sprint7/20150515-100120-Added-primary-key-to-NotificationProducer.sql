

GO
PRINT N'Dropping DF_NotificationProducerId...';


GO
ALTER TABLE [Business].[NotificationProducer] DROP CONSTRAINT [DF_NotificationProducerId];


GO
PRINT N'Dropping FK_NotificationProducer_Notification...';


GO
ALTER TABLE [Business].[NotificationProducer] DROP CONSTRAINT [FK_NotificationProducer_Notification];


GO
PRINT N'Dropping FK_NotificationProducer_Producer...';


GO
ALTER TABLE [Business].[NotificationProducer] DROP CONSTRAINT [FK_NotificationProducer_Producer];


GO
/*
The column [Business].[NotificationProducer].[Id] is being dropped, data loss could occur.
*/
GO
PRINT N'Starting rebuilding table [Business].[NotificationProducer]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [Business].[tmp_ms_xx_NotificationProducer] (
    [NotificationId] UNIQUEIDENTIFIER NOT NULL,
    [ProducerId]     UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_NotificationProducer] PRIMARY KEY CLUSTERED ([NotificationId] ASC, [ProducerId] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [Business].[NotificationProducer])
    BEGIN
        INSERT INTO [Business].[tmp_ms_xx_NotificationProducer] ([NotificationId], [ProducerId])
        SELECT   [NotificationId],
                 [ProducerId]
        FROM     [Business].[NotificationProducer]
        ORDER BY [NotificationId] ASC, [ProducerId] ASC;
    END

DROP TABLE [Business].[NotificationProducer];

EXECUTE sp_rename N'[Business].[tmp_ms_xx_NotificationProducer]', N'NotificationProducer';

EXECUTE sp_rename N'[Business].[tmp_ms_xx_constraint_PK_NotificationProducer]', N'PK_NotificationProducer', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating FK_NotificationProducer_Notification...';


GO
ALTER TABLE [Business].[NotificationProducer] WITH NOCHECK
    ADD CONSTRAINT [FK_NotificationProducer_Notification] FOREIGN KEY ([NotificationId]) REFERENCES [Notification].[Notification] ([Id]);


GO
PRINT N'Creating FK_NotificationProducer_Producer...';


GO
ALTER TABLE [Business].[NotificationProducer] WITH NOCHECK
    ADD CONSTRAINT [FK_NotificationProducer_Producer] FOREIGN KEY ([ProducerId]) REFERENCES [Business].[Producer] ([Id]);


GO
PRINT N'Checking existing data against newly created constraints';


GO


GO
ALTER TABLE [Business].[NotificationProducer] WITH CHECK CHECK CONSTRAINT [FK_NotificationProducer_Notification];

ALTER TABLE [Business].[NotificationProducer] WITH CHECK CHECK CONSTRAINT [FK_NotificationProducer_Producer];


GO
PRINT N'Update complete.';


GO
