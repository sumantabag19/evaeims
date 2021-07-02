CREATE TABLE [dbo].[OrganizationApplicationMapping] (
    [OrganizationApplicationId] INT              IDENTITY (1, 1) NOT NULL,
    [OrgId]                     INT              NOT NULL,
    [AppId]                     INT              NOT NULL,
	[CanAccessAllUsers]			BIT				 CONSTRAINT [DF_OrganizationApplicationMapping_CanAccessAllUsers] DEFAULT ((0)),
    [IsActive]                  BIT              CONSTRAINT [DF_OrganizationApplicationMapping_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedBy]                 UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]                 DATETIME         NOT NULL,
    [ModiefiedBy]               UNIQUEIDENTIFIER NOT NULL,
    [ModiefiedOn]               DATETIME         NOT NULL,
    CONSTRAINT [PK__Organiza__A2D8FC8AA368FFAE] PRIMARY KEY CLUSTERED ([OrganizationApplicationId] ASC),
    CONSTRAINT [FK__Organizat__AppId__30C33EC3] FOREIGN KEY ([AppId]) REFERENCES [dbo].[Application] ([AppId]),
    CONSTRAINT [FK__Organizat__OrgId__2FCF1A8A] FOREIGN KEY ([OrgId]) REFERENCES [dbo].[Organization] ([OrgId])
);


GO

CREATE NONCLUSTERED INDEX [IX_AppId] ON [dbo].[OrganizationApplicationMapping] (AppId,OrgId);
