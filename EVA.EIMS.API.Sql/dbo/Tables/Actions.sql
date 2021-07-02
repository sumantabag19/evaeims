CREATE TABLE [dbo].[Actions] (
    [ActionId]     INT              IDENTITY (1, 1) NOT NULL,
    [ActionName]   NVARCHAR (100)   NOT NULL,
    [Description]  NVARCHAR (100)   NULL,
    [ModuleId]     INT              NOT NULL,
    [AccessTypeId] INT              NOT NULL,
    [IsActive]     BIT              CONSTRAINT [DF_Action_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedBy]    UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]    DATETIME         NOT NULL,
    [ModifiedBy]   UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]   DATETIME         NOT NULL,
    CONSTRAINT [PK_Action] PRIMARY KEY CLUSTERED ([ActionId] ASC),
    CONSTRAINT [FK_Actions_AccessType] FOREIGN KEY ([AccessTypeId]) REFERENCES [dbo].[AccessType] ([AccessTypeId]),
    CONSTRAINT [FK_Actions_Module] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[Module] ([ModuleId])
);

