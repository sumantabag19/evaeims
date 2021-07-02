CREATE EXTERNAL TABLE [dbo].[tblDevice] (
    [Subject] UNIQUEIDENTIFIER NOT NULL,
    [DeviceId] NVARCHAR (160) NOT NULL,
    [PrimaryKey] NVARCHAR (250) NULL,
    [OrgId] NVARCHAR (60) NOT NULL,
    [ClientType] INT NOT NULL,
    [IsDeleted] BIT NOT NULL,
    [SerialKey] UNIQUEIDENTIFIER NULL,
    [IsUsed] BIT NOT NULL
)
    WITH (
    DATA_SOURCE = [RefIMSValUat]
    );

