CREATE TABLE [dbo].[ApplicationRoleMapping] (
    [ApplicationRoleId] INT              IDENTITY (1, 1) NOT NULL,
    [RoleId]            INT              NOT NULL,
    [AppId]             INT              NOT NULL,
    [IsActive]          BIT              CONSTRAINT [DF_ApplicationRoleMapping_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedBy]         UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]         DATETIME         NOT NULL,
    [ModifiedBy]        UNIQUEIDENTIFIER NULL,
    [ModifiedOn]        DATETIME         NULL,
    CONSTRAINT [PK_RoleApplicationMapping] PRIMARY KEY CLUSTERED ([ApplicationRoleId] ASC),
    CONSTRAINT [FK_RoleApplicationMapping_Application] FOREIGN KEY ([AppId]) REFERENCES [dbo].[Application] ([AppId]),
    CONSTRAINT [FK_RoleApplicationMapping_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([RoleId])
);

