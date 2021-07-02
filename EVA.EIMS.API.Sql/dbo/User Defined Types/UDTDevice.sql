CREATE TYPE [dbo].[UDTDevice] AS TABLE (
    [OrgName]         NVARCHAR (150)   NOT NULL,
    [AppName]         NVARCHAR (50)    NOT NULL,
    [DeviceId]        NVARCHAR (50)    NOT NULL,
    [Subject]         UNIQUEIDENTIFIER NOT NULL,
    [SerialKey]       UNIQUEIDENTIFIER NOT NULL,
    [PrimaryKey]      NVARCHAR (MAX)   NOT NULL,
    [OrgId]           INT              NOT NULL,
    [ClientTypeId]    INT              NOT NULL,
    [AppId]           INT              NOT NULL,
    [IsActive]        BIT              NOT NULL,
    [CreatedBy]       UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]       DATETIME         NOT NULL,
    [ModifiedBy]      UNIQUEIDENTIFIER NULL,
    [ModifiedOn]      DATETIME         NULL,
    [IsUsed]          BIT              NOT NULL,
    [GatewayDeviceId] INT              NULL);

