CREATE TABLE [dbo].[ClientTypeAccessException] (
    [AccessExceptionId] INT              IDENTITY (1, 1) NOT NULL,
    [ClientTypeId]      INT              NOT NULL,
    [ModuleId]          INT              NOT NULL,
    [ActionId]          INT              NOT NULL,
    [CreatedBy]         UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]         DATETIME         NOT NULL,
    CONSTRAINT [PK_ClientTypeAccessException] PRIMARY KEY CLUSTERED ([AccessExceptionId] ASC),
    CONSTRAINT [FK_ClientTypeAccessException_Actions] FOREIGN KEY ([ActionId]) REFERENCES [dbo].[Actions] ([ActionId]),
    CONSTRAINT [FK_ClientTypeAccessException_ClientType] FOREIGN KEY ([ClientTypeId]) REFERENCES [dbo].[ClientType] ([ClientTypeId]),
    CONSTRAINT [FK_ClientTypeAccessException_Module] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[Module] ([ModuleId])
);

