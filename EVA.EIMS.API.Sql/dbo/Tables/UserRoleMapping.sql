CREATE TABLE [dbo].[UserRoleMapping] (
    [UserRoleId] INT              IDENTITY (1, 1) NOT NULL,
    [UserId]     UNIQUEIDENTIFIER NOT NULL,
    [RoleId]     INT              NOT NULL,
    [IsActive]   BIT              NOT NULL,
    [CreatedBy]  UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]  DATETIME         NOT NULL,
    [ModifiedBy] UNIQUEIDENTIFIER NULL,
    [ModifiedOn] DATETIME         NULL,
    CONSTRAINT [PK_UserRoleMapping] PRIMARY KEY CLUSTERED ([UserRoleId] ASC),
    CONSTRAINT [FK_UserRoleMapping_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([RoleId]),
    CONSTRAINT [FK_UserRoleMapping_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([UserId])
);

GO

CREATE INDEX [IX_UserId] ON [dbo].[UserRoleMapping] ([UserId],[IsActive]);
