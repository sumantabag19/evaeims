CREATE EXTERNAL TABLE [dbo].[tblUserRole] (
    [UserRoleId] UNIQUEIDENTIFIER NOT NULL,
    [RoleId] NVARCHAR (60) NULL,
    [UserId] UNIQUEIDENTIFIER NULL,
    [IsDeleted] BIT NULL
)
    WITH (
    DATA_SOURCE = [RefIMSValUat]
    );

