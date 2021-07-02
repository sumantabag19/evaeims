CREATE PROCEDURE [dbo].[proc_GetClientTypeModuleDetails]
@p_ClientTypeId int
AS
BEGIN

DROP TABLE IF EXISTS #ClientTypeModuleDetailTable;
SELECT m.ModuleId,m.ModuleName,ISNULL(rma.ReadAccess, 0) AS ReadAccess,ISNULL(rma.WriteAccess, 0) AS WriteAccess,ISNULL(rma.EditAccess, 0) AS EditAccess,ISNULL(rma.DeleteAccess, 0) AS DeleteAccess,ISNULL(rma.IsActive, 0) AS IsActive INTO #ClientTypeModuleDetailTable FROM  [dbo].Module m
			left join [dbo].ClientTypeModuleAccess rma ON m.ModuleId=rma.ModuleId and rma.ClientTypeId = @p_ClientTypeId;
			SELECT * FROM  #ClientTypeModuleDetailTable;

END