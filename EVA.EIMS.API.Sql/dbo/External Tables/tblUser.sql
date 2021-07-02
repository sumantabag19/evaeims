CREATE EXTERNAL TABLE [dbo].[tblUser] (
    [Subject] UNIQUEIDENTIFIER NOT NULL,
    [Name] VARCHAR (60) NULL,
    [FamilyName] VARCHAR (60) NULL,
    [OrgId] NVARCHAR (60) NULL,
    [Id] UNIQUEIDENTIFIER NULL,
    [UserName] NVARCHAR (100) NOT NULL,
    [ProtectedPassword] NVARCHAR (MAX) NULL,
    [EmailId] VARCHAR (100) NULL,
    [IsDeleted] BIT NULL,
    [ClientType] INT NOT NULL
)
    WITH (
    DATA_SOURCE = [RefIMSValUat]
    );

