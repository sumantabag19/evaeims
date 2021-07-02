CREATE PROCEDURE [dbo].[proc_GetRoleModuleDetails]
@p_RoleId int
AS
BEGIN

DROP TABLE IF EXISTS #RoleModuleDetailTable;
SELECT m.ModuleId,m.ModuleName,ISNULL(rma.ReadAccess, 0) AS ReadAccess,ISNULL(rma.WriteAccess, 0) AS WriteAccess,ISNULL(rma.EditAccess, 0) AS EditAccess,ISNULL(rma.DeleteAccess, 0) AS DeleteAccess,ISNULL(rma.IsActive, 0) AS IsActive INTO #RoleModuleDetailTable FROM  [dbo].Module m
			left join [dbo].RoleModuleAccess rma ON m.ModuleId=rma.ModuleId where rma.RoleId = @p_RoleId;
			SELECT * FROM  #RoleModuleDetailTable;

END