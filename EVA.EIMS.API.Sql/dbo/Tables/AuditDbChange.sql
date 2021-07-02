CREATE TABLE [dbo].[AuditDbChange] (
    [DBChangeId]      INT            IDENTITY (1, 1) NOT NULL,
    [ChangeType]      SMALLINT       NULL,
    [ChangeDatetime]  DATETIME       NULL,
    [ChangeQuery]     NVARCHAR (MAX) NULL,
    [ChangeRequestIP] NVARCHAR (50)  NULL,
    CONSTRAINT [PK_AuditDBChange] PRIMARY KEY CLUSTERED ([DBChangeId] ASC)
);

