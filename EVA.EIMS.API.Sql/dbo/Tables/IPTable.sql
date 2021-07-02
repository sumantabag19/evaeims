CREATE TABLE [dbo].[IPTable] (
    [IPAddressId]     INT              IDENTITY (1, 1) NOT NULL,
    [GatewayDeviceId] INT              NULL,
    [OrgId]           INT              NOT NULL,
    [AppId]           INT              NOT NULL,
    [IPStartAddress]  NVARCHAR (25)    NOT NULL,
    [IPEndAddress]    NVARCHAR (25)    NOT NULL,
    [IPProxyAddress]  NVARCHAR (25)    NOT NULL,
    [PortNo]          INT              NULL,
    [IsProxyEnabled]  BIT              NOT NULL,
    [IsIPAllowed]     BIT              NOT NULL,
    [CreatedBy]       UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]       DATETIME         NOT NULL,
    [ModifiedBy]      UNIQUEIDENTIFIER NULL,
    [ModifiedOn]      DATETIME         NULL,
    [IsActive]        BIT              CONSTRAINT [DF_IPTable_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_IPTable] PRIMARY KEY CLUSTERED ([IPAddressId] ASC),
    CONSTRAINT [FK_IPTable_Application] FOREIGN KEY ([AppId]) REFERENCES [dbo].[Application] ([AppId]),
    CONSTRAINT [FK_IPTable_Organization] FOREIGN KEY ([OrgId]) REFERENCES [dbo].[Organization] ([OrgId])
);

