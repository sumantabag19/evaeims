CREATE TABLE [dbo].[AuditUserLogin] (
    [LoginHistoryId]    INT              IDENTITY (1, 1) NOT NULL,
    [UserId]            UNIQUEIDENTIFIER NOT NULL,
    [LastLoginDatetime] DATETIME         NULL,
    CONSTRAINT [PK_AuditUserLogin] PRIMARY KEY CLUSTERED ([LoginHistoryId] ASC)
);

