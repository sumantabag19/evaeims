CREATE EXTERNAL TABLE [dbo].[tblOrg] (
    [OrgId] NVARCHAR (60) NOT NULL,
    [Name] NVARCHAR (100) NOT NULL,
    [Description] NVARCHAR (160) NULL,
    [IsDeleted] BIT NULL
)
    WITH (
    DATA_SOURCE = [RefIMSValUat]
    );

