CREATE TABLE [dbo].[ClientType] (
    [ClientTypeId]   INT              IDENTITY (1, 1) NOT NULL,
    [ClientTypeName] NVARCHAR (50)    NOT NULL,
    [Description]    NVARCHAR (150)   NULL,
    [IsActive]       BIT              CONSTRAINT [DF_ClientType_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]      DATETIME         NOT NULL,
    [ModifiedBy]     UNIQUEIDENTIFIER NULL,
    [ModifiedOn]     DATETIME         NULL,
    CONSTRAINT [PK_ClientType] PRIMARY KEY CLUSTERED ([ClientTypeId] ASC)
);

