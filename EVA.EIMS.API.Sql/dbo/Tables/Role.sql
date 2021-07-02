CREATE TABLE [dbo].[Role] (
    [RoleId]            INT              IDENTITY (1, 1) NOT NULL,
    [RoleName]          NVARCHAR (150)   NOT NULL,
    [Description]       NVARCHAR (200)   NULL,
    [MultipleOrgAccess] BIT              CONSTRAINT [DF_Role_MultipleOrgAccess] DEFAULT ((0)) NULL,
    [IsActive]          BIT              NOT NULL,
    [CreatedBy]         UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]         DATETIME         NOT NULL,
    [ModifiedBy]        UNIQUEIDENTIFIER NULL,
    [ModifiedOn]        DATETIME         NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([RoleId] ASC)
);

