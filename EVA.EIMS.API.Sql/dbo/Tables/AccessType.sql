CREATE TABLE [dbo].[AccessType] (
    [AccessTypeId]   INT              IDENTITY (1, 1) NOT NULL,
    [AccessTypeName] NVARCHAR (100)   NOT NULL,
    [Description]    NVARCHAR (200)   NULL,
    [IsActive]       BIT              CONSTRAINT [DF_AccessType_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]      DATETIME         NOT NULL,
    [ModifiedBy]     UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]     DATETIME         NOT NULL,
    CONSTRAINT [PK_AccessType] PRIMARY KEY CLUSTERED ([AccessTypeId] ASC)
);

