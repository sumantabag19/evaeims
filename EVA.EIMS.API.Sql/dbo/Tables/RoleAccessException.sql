CREATE TABLE [dbo].[RoleAccessException] (
    [AccessExceptionId] INT              IDENTITY (1, 1) NOT NULL,
    [RoleId]            INT              NOT NULL,
    [ModuleId]          INT              NOT NULL,
    [ActionId]          INT              NOT NULL,
    [CreatedBy]         UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]         DATETIME         NOT NULL,
    CONSTRAINT [PK_RoleAccessException] PRIMARY KEY CLUSTERED ([AccessExceptionId] ASC),
    CONSTRAINT [FK_RoleAccessException_Actions] FOREIGN KEY ([ActionId]) REFERENCES [dbo].[Actions] ([ActionId]),
    CONSTRAINT [FK_RoleAccessException_Module] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[Module] ([ModuleId]),
    CONSTRAINT [FK_RoleAccessException_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([RoleId])
);

