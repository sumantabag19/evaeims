CREATE TABLE [dbo].[Application] (
    [AppId]            INT              IDENTITY (9, 1) NOT NULL,
    [AppName]          NVARCHAR (50)    NOT NULL,
    [Description]      NVARCHAR (150)   CONSTRAINT [DF__Applicati__Descr__32E0915F] DEFAULT (NULL) NULL,
    [AppUrl]           NVARCHAR (255)   CONSTRAINT [DF__Applicati__AppUr__33D4B598] DEFAULT (NULL) NOT NULL,
    [IsActive]         BIT              CONSTRAINT [DF_Application_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedBy]        UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]        DATETIME         NOT NULL,
    [ModifiedBy]       UNIQUEIDENTIFIER CONSTRAINT [DF__Applicati__Modif__34C8D9D1] DEFAULT (NULL) NOT NULL,
    [ModifiedOn]       DATETIME         CONSTRAINT [DF__Applicati__Modif__35BCFE0A] DEFAULT (NULL) NOT NULL,
    [IsPwdExpNotify]   BIT              CONSTRAINT [DF_Application_IsSendPasswordExpireNotification] DEFAULT ((1)) NOT NULL,
    [PwdExpNotifyDays] INT              CONSTRAINT [DF_Application_PasswordExpireNotificationDays] DEFAULT ((7)) NOT NULL,
    [AzureAppId]       UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_application_AppId] PRIMARY KEY CLUSTERED ([AppId] ASC)
);

