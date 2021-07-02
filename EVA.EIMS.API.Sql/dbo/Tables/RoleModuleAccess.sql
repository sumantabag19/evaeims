CREATE TABLE [dbo].[RoleModuleAccess] (
    [RoleAccessId] INT              IDENTITY (1, 1) NOT NULL,
    [RoleId]       INT              NOT NULL,
    [ModuleId]     INT              NOT NULL,
    [ReadAccess]   BIT              NOT NULL,
    [WriteAccess]  BIT              NOT NULL,
    [EditAccess]   BIT              NOT NULL,
    [DeleteAccess] BIT              NOT NULL,
    [IsActive]     BIT              NOT NULL,
    [CreatedBy]    UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]    DATETIME         NOT NULL,
    [ModifiedBy]   UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]   DATETIME         NOT NULL,
    CONSTRAINT [PK_RoleModuleMapping] PRIMARY KEY CLUSTERED ([RoleAccessId] ASC),
    CONSTRAINT [FK_RoleModuleAccess_Module] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[Module] ([ModuleId]),
    CONSTRAINT [FK_RoleModulesAccess_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([RoleId])
);

