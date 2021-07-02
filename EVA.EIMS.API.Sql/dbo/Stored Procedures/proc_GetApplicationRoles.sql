

CREATE PROCEDURE [dbo].[proc_GetApplicationRoles]

@p_AppId int

as
begin
DROP TABLE IF EXISTS #Users;


DROP TABLE IF EXISTS #Users;

create table #Users
(
	[RoleId] [int] ,
	[RoleName] [nvarchar](150),
	[Description] [nvarchar](200)
);

select r.RoleId,r.RoleName,r.Description from [dbo].Role  r
Inner join [dbo].ApplicationRoleMapping arm on r.RoleId = arm.RoleId and  AppId= @p_AppId;
	
	END