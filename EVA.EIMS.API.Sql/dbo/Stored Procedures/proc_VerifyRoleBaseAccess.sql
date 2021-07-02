

CREATE PROCEDURE [dbo].[proc_VerifyRoleBaseAccess]
@p_Role nvarchar(50),
@p_ModuleName nvarchar(50),
@p_ActionName nvarchar(50)
as
begin
DECLARE @accessType nvarchar(100);
DECLARE @authorized int;
DECLARE @ModuleID int;

SELECT @ModuleID=ModuleId from dbo.Module where ModuleName = @p_ModuleName and IsActive=1;

DROP TABLE IF EXISTS #RoleModuleAccessTable;

SELECT RoleAccessId,ModuleId,ReadAccess,WriteAccess,EditAccess,DeleteAccess into #RoleModuleAccessTable from RoleModuleAccess WHERE (RoleId = (SELECT RoleId from dbo.Role where RoleName = @p_Role and IsActive = 1) and ModuleId = @ModuleID)

IF((select 1 from #RoleModuleAccessTable) > 0)
	BEGIN
		SELECT @accessType = AccessTypeName from dbo.AccessType WHERE AccessTypeId = (SELECT AccessTypeId from dbo.AccessType where AccessTypeId = 
			(SELECT AccessTypeId from dbo.Actions where ActionName = @p_ActionName and IsActive = 1 and ModuleId = @ModuleID));

			SELECT @authorized =
				CASE 
					WHEN @accessType='Read' and ReadAccess = 1  THEN 1

					WHEN @accessType='Write' and WriteAccess=1  THEN 1

					WHEN @accessType='Edit' and EditAccess=1  THEN 1

					WHEN @accessType='Delete' and DeleteAccess=1  THEN 1

					ELSE 0
				END 
			FROM #RoleModuleAccessTable;
	END
	ELSE BEGIN
		SELECT @authorized = 0;
	END
	DROP TABLE IF EXISTS #AccessPermission;
SELECT @authorized as Authorized into #AccessPermission ;
select * from #AccessPermission;

END