CREATE TABLE [dbo].[LockAccount] (
    [UserId]          UNIQUEIDENTIFIER NOT NULL,
    [LockTypeId]      INT              NOT NULL,
    [FailedLockCount] INT              NOT NULL,
    [FailedLockDate]  DATETIME         NOT NULL,
    CONSTRAINT [PK_LockAccount] PRIMARY KEY CLUSTERED ([UserId] ASC, [LockTypeId] ASC)
);

