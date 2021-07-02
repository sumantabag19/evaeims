CREATE TABLE [dbo].[UserOrganizationMapping] (
    [UserOrgId] INT              IDENTITY (1, 1) NOT NULL,
    [UserId]    UNIQUEIDENTIFIER NOT NULL,
    [OrgId]     INT              NOT NULL,
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn] DATETIME         NOT NULL,
    CONSTRAINT [PK_UserOrganizationMapping] PRIMARY KEY CLUSTERED ([UserOrgId] ASC),
    CONSTRAINT [FK_UserOrganizationMapping_Organization] FOREIGN KEY ([OrgId]) REFERENCES [dbo].[Organization] ([OrgId]),
    CONSTRAINT [FK_UserOrganizationMapping_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([UserId])
);

