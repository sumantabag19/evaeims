CREATE TABLE [dbo].[Device] (
    [DeviceId]        NVARCHAR (50)    NOT NULL,
    [Subject]         UNIQUEIDENTIFIER NOT NULL,
    [SerialKey]       UNIQUEIDENTIFIER NOT NULL,
    [PrimaryKey]      NVARCHAR (MAX)   NOT NULL,
    [OrgId]           INT              NOT NULL,
    [ClientTypeId]    INT              NOT NULL,
    [AppId]           INT              NOT NULL,
    [IsActive]        BIT              CONSTRAINT [DF_Device_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedBy]       UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]       DATETIME         NOT NULL,
    [ModifiedBy]      UNIQUEIDENTIFIER NULL,
    [ModifiedOn]      DATETIME         NULL,
    [IsUsed]          BIT              NOT NULL,
    [GatewayDeviceId] INT              NULL,
    CONSTRAINT [PK_Device_1] PRIMARY KEY CLUSTERED ([DeviceId] ASC),
    CONSTRAINT [FK_Device_Application] FOREIGN KEY ([AppId]) REFERENCES [dbo].[Application] ([AppId]),
    CONSTRAINT [FK_Device_ClientType] FOREIGN KEY ([ClientTypeId]) REFERENCES [dbo].[ClientType] ([ClientTypeId]),
    CONSTRAINT [FK_Device_Organization] FOREIGN KEY ([OrgId]) REFERENCES [dbo].[Organization] ([OrgId])
);

