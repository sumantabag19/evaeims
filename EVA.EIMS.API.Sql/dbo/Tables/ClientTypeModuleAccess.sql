CREATE TABLE [dbo].[ClientTypeModuleAccess] (
    [ClientTypeAccessId] INT              IDENTITY (1, 1) NOT NULL,
    [ClientTypeId]       INT              NOT NULL,
    [ModuleId]           INT              NOT NULL,
    [ReadAccess]         BIT              NOT NULL,
    [WriteAccess]        BIT              NOT NULL,
    [EditAccess]         BIT              NOT NULL,
    [DeleteAccess]       BIT              NOT NULL,
    [IsActive]           BIT              NOT NULL,
    [CreatedBy]          UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]          DATETIME         NOT NULL,
    [ModifiedBy]         UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]         DATETIME         NOT NULL,
    CONSTRAINT [PK_ClientModuleMapping] PRIMARY KEY CLUSTERED ([ClientTypeAccessId] ASC),
    CONSTRAINT [FK_ClientType_ClientTypeModuleMapping] FOREIGN KEY ([ClientTypeId]) REFERENCES [dbo].[ClientType] ([ClientTypeId]),
    CONSTRAINT [FK_ClientTypeModuleAccess_Module] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[Module] ([ModuleId])
);

