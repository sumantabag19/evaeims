

CREATE PROCEDURE [dbo].[proc_VerifyRoleAccessExceptions]
@p_Role nvarchar(50),
@p_ModuleName nvarchar(50),
@p_ActionName nvarchar(50)
AS
BEGIN
	DECLARE @authorized int;
	DECLARE @ModuleID int;


	IF((SELECT distinct 1 FROM [dbo].RoleAccessException WHERE 
			(RoleId = (SELECT RoleId FROM dbo.Role WHERE RoleName = @p_Role and IsActive = 1) and ModuleId = (SELECT ModuleId FROM dbo.Module WHERE ModuleName = @p_ModuleName and IsActive=1) and ActionId = (SELECT ActionId FROM dbo.Actions WHERE ActionName = @p_ActionName and IsActive=1) )) >=1)
	BEGIN

		SELECT 1 AS Authorized;
	END ELSE
	BEGIN

		SELECT 0 AS Authorized;
	END
END;