CREATE TABLE [dbo].[Organization] (
    [OrgId]       INT              IDENTITY (1, 1) NOT NULL,
    [OrgName]     NVARCHAR (150)   NOT NULL,
    [Description] NVARCHAR (200)   NULL,
    [MobileLoginEnabled] BIT DEFAULT(1),
    [TenantDB] VARCHAR(200) NULL, 
    [IsActive]    BIT              CONSTRAINT [DF_Organization_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]   DATETIME         NOT NULL,
    [ModifiedBy]  UNIQUEIDENTIFIER NULL,
    [ModifiedOn]  DATETIME         NULL,
    CONSTRAINT [PK_Organization] PRIMARY KEY CLUSTERED ([OrgId] ASC)
);

