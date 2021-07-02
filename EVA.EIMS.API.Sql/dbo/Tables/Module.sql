CREATE TABLE [dbo].[Module] (
    [ModuleId]    INT              IDENTITY (1, 1) NOT NULL,
    [ModuleName]  NCHAR (100)      NOT NULL,
    [Description] NCHAR (200)      NULL,
    [IsActive]    BIT              CONSTRAINT [DF_Table_1_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]   DATETIME         NOT NULL,
    [ModifiedBy]  UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]  DATETIME         NOT NULL,
    CONSTRAINT [PK_Table_1] PRIMARY KEY CLUSTERED ([ModuleId] ASC)
);

