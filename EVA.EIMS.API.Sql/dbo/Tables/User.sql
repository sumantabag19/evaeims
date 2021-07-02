CREATE TABLE [dbo].[User] (
    [UserId]                   UNIQUEIDENTIFIER NOT NULL,
    [Subject]                  UNIQUEIDENTIFIER NOT NULL,
    [Name]                     NVARCHAR (100)   NULL,
    [FamilyName]               NVARCHAR (100)   NULL,
    [UserName]                 NVARCHAR (100)   NULL,
    [EmailId]                  NVARCHAR (100)   NULL,
    [PhoneNumber]              NVARCHAR (60)    NULL,
    [PhoneNumberConfirmed]     BIT              NOT NULL,
    [PasswordHash]             NVARCHAR (250)   NOT NULL,
    [OrgId]                    INT              NOT NULL,
    [IsAccLock]                BIT              NOT NULL,
    [PasswordExpiration]       DATETIME         NOT NULL,
    [IsPasswordReset]          BIT              NOT NULL,
    [TwoFactorEnabled]         BIT              CONSTRAINT [DF_User_TwoFactorEnabled] DEFAULT ((1)) NOT NULL,
    [LastPasswordChangeOn]     DATETIME         NULL,
    [IsActive]                 BIT              NOT NULL,
    [LockAccountDate]          DATETIME         NULL,
    [PasswordActivationPeriod] INT              CONSTRAINT [DF_User_PasswordActivationPeriod] DEFAULT ((1)) NULL,
    [CreatedBy]                UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]                DATETIME         NOT NULL,
    [ModifiedBy]               UNIQUEIDENTIFIER NULL,
    [ModifiedOn]               DATETIME         NULL,
    [IsFirstTimeLogin]         BIT              CONSTRAINT [DF_User_IsFirstTimeLogin] DEFAULT ((1)) NOT NULL,
    [RequiredSecurityQuestion] BIT              CONSTRAINT [DF_User_RequiredSecurityQuestion] DEFAULT ((1)) NOT NULL,
    [LockTypeID]               INT              NULL,
    [UnlockAccountDate]        DATETIME         NULL,
    [ProviderId]               INT              NULL,
    [MobileLoginEnabled] BIT DEFAULT(1),
    [EmailVerified] BIT NULL DEFAULT (0), 
    [AppUserId] BIGINT NULL, 
    [AppOrgId] BIGINT NULL, 
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [FK_User_Organization] FOREIGN KEY ([OrgId]) REFERENCES [dbo].[Organization] ([OrgId]),
    CONSTRAINT [FK_User_Provider_ProviderID] FOREIGN KEY ([ProviderId]) REFERENCES [dbo].[AuthProviderMaster] ([ProviderId])
);

GO

CREATE NONCLUSTERED INDEX [IX_UserName] ON [dbo].[User] ([UserName],[IsActive]);

GO

CREATE NONCLUSTERED INDEX [IX_OrgId] ON [dbo].[User] ([OrgId]);

GO
