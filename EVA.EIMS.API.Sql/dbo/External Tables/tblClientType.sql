CREATE EXTERNAL TABLE [dbo].[tblClientType] (
    [ClientTypeId] INT NOT NULL,
    [ClientType] NVARCHAR (100) NOT NULL,
    [Description] NVARCHAR (160) NULL
)
    WITH (
    DATA_SOURCE = [RefIMSValUat]
    );

