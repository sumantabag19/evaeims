CREATE EXTERNAL TABLE [dbo].[tblClient] (
    [ClientId] NVARCHAR (160) NOT NULL,
    [ClientName] NVARCHAR (200) NOT NULL,
    [ClientSecret] NVARCHAR (200) NOT NULL,
    [Flow] NVARCHAR (60) NOT NULL,
    [AllowedScopes] NVARCHAR (160) NOT NULL,
    [IsDeleted] BIT NULL,
    [ClientType] INT NOT NULL
)
    WITH (
    DATA_SOURCE = [RefIMSValUat]
    );

