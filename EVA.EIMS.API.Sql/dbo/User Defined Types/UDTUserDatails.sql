﻿CREATE TYPE [dbo].[UDTUserDatails] AS TABLE (
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
    [TwoFactorEnabled]         BIT              NULL,
    [LastPasswordChangeOn]     DATETIME         NULL,
    [IsActive]                 BIT              NOT NULL,
    [LockAccountDate]          DATETIME         NULL,
    [PasswordActivationPeriod] INT              NULL,
    [CreatedBy]                UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]                DATETIME         NOT NULL,
    [ModifiedBy]               UNIQUEIDENTIFIER NULL,
    [ModifiedOn]               DATETIME         NULL,
    [IsFirstTimeLogin]         BIT              NOT NULL,
    [RequiredSecurityQuestion] BIT              NOT NULL,
    [LockTypeID]               INT              NULL,
    [UnlockAccountDate]        DATETIME         NULL,
    [ProviderId]               INT              NULL,
    [Roles]                    NVARCHAR (200)   NULL,
    [ClientType]               NVARCHAR (200)   NULL);

