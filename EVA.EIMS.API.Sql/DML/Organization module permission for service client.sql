--Added organization module access permission for service client
DECLARE @ModuleId INT
SELECT @ModuleId = ModuleId FROM Module WHERE ModuleName= 'Organization'
IF NOT EXISTS (SELECT 1 FROM ClientTypeModuleAccess WHERE ClientTypeId = 4 AND ModuleId = @ModuleId)
INSERT INTO ClientTypeModuleAccess
SELECT 4,2,1,1,1,1,1,'02E94960-F394-4571-88ED-2140DA9745E8',GETDATE(),'02E94960-F394-4571-88ED-2140DA9745E8',GETDATE()
