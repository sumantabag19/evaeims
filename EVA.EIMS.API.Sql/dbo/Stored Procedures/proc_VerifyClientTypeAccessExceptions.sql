

CREATE PROCEDURE [dbo].[proc_VerifyClientTypeAccessExceptions]
@p_ClientType nvarchar(50),
@p_ModuleName nvarchar(50),
@p_ActionName nvarchar(50)
AS
BEGIN
	DECLARE @authorized int;
	DECLARE @ModuleID int;


	IF((SELECT distinct 1 FROM [dbo].ClientTypeAccessException WHERE 
			(ClientTypeId = (SELECT ClientTypeId FROM dbo.ClientType WHERE ClientTypeName = @p_ClientType and IsActive = 1) and ModuleId = (SELECT ModuleId FROM dbo.Module WHERE ModuleName = @p_ModuleName and IsActive=1) and ActionId = (SELECT ActionId FROM dbo.Actions WHERE ActionName = @p_ActionName and IsActive=1) )) >=1)
	BEGIN

		SELECT 1 AS Authorized;
	END ELSE
	BEGIN

		SELECT 0 AS Authorized;
	END
END;