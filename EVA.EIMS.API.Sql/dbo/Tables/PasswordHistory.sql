CREATE TABLE [dbo].[PasswordHistory] (
    [PasswordHistoryId]      INT              IDENTITY (1, 1) NOT NULL,
    [UserId]                 UNIQUEIDENTIFIER NOT NULL,
    [PasswordHash]           NVARCHAR (250)   NOT NULL,
    [LastPasswordChangeDate] DATETIME         NOT NULL,
    [CreateBy]               UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_PasswordHistory] PRIMARY KEY CLUSTERED ([PasswordHistoryId] ASC),
    CONSTRAINT [FK_PasswordHistory_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([UserId])
);

