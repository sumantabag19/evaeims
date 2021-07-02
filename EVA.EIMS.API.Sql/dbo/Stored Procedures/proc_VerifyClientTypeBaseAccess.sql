

CREATE PROCEDURE [dbo].[proc_VerifyClientTypeBaseAccess]
@p_ClientType nvarchar(50),
@p_ModuleName nvarchar(50),
@p_ActionName nvarchar(50)
as
begin
DECLARE @accessType nvarchar(100);
DECLARE @authorized int;
DECLARE @ModuleID int;

SELECT @ModuleID=ModuleId from dbo.Module where ModuleName = @p_ModuleName and IsActive=1;

DROP TABLE IF EXISTS #ClientTypeModuleAccessTable;

SELECT ClientTypeAccessId,ModuleId,ReadAccess,WriteAccess,EditAccess,DeleteAccess into #ClientTypeModuleAccessTable from ClientTypeModuleAccess WHERE (ClientTypeId = (SELECT ClientTypeId from dbo.ClientType where ClientTypeName = @p_ClientType) and ModuleId = @ModuleID)

IF((select 1 from #ClientTypeModuleAccessTable) > 0)
	BEGIN
		SELECT @accessType = AccessTypeName from dbo.AccessType WHERE AccessTypeId = (SELECT AccessTypeId from dbo.AccessType where AccessTypeId = 
			(SELECT AccessTypeId from dbo.Actions where ActionName = @p_ActionName and ModuleId = @ModuleID));

			SELECT @authorized =
				CASE 
					WHEN @accessType='Read' and ReadAccess = 1  THEN 1

					WHEN @accessType='Write' and WriteAccess=1  THEN 1

					WHEN @accessType='Edit' and EditAccess=1  THEN 1

					WHEN @accessType='Delete' and DeleteAccess=1  THEN 1

					ELSE 0
				END 
			FROM #ClientTypeModuleAccessTable;
	END
	ELSE BEGIN
		SELECT @authorized = 0;
	END
	DROP TABLE IF EXISTS #AccessPermission;
SELECT @authorized as Authorized into #AccessPermission ;
select * from #AccessPermission;

END