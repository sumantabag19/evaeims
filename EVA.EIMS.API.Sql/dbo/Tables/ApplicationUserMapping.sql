CREATE TABLE [dbo].[ApplicationUserMapping] (
    [MappingId]  INT              IDENTITY (1, 1) NOT NULL,
    [UserId]     UNIQUEIDENTIFIER NOT NULL,
    [AppId]      INT              NOT NULL,
    [IsActive]   BIT              NOT NULL,
    [CreatedBy]  UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]  DATETIME         NOT NULL,
    [ModifiedBy] UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn] DATETIME         NOT NULL,
    CONSTRAINT [PK_applicationusermapping_MappingId] PRIMARY KEY CLUSTERED ([MappingId] ASC),
    CONSTRAINT [FK_ApplicationUserMapping_Application] FOREIGN KEY ([AppId]) REFERENCES [dbo].[Application] ([AppId]),
    CONSTRAINT [FK_ApplicationUserMapping_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([UserId])
);


GO

CREATE INDEX [IX_UserId] ON [dbo].[ApplicationUserMapping] ([UserId]) INCLUDE ([AppId]);
