CREATE TABLE [dbo].[UserClientTypeMapping] (
    [UserClientId] INT              IDENTITY (1, 1) NOT NULL,
    [ClientTypeId] INT              NOT NULL,
    [UserId]       UNIQUEIDENTIFIER NOT NULL,
    [IsActive]     BIT              CONSTRAINT [DF_UserClientTypeMapping_IsActive] DEFAULT ((1)) NULL,
    CONSTRAINT [PK_UserClientTypeMapping] PRIMARY KEY CLUSTERED ([UserClientId] ASC),
    CONSTRAINT [FK_UserClientTypeMapping_ClientType] FOREIGN KEY ([ClientTypeId]) REFERENCES [dbo].[ClientType] ([ClientTypeId]),
    CONSTRAINT [FK_UserClientTypeMapping_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([UserId])
);


GO

CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[UserClientTypeMapping] (UserId);

GO
