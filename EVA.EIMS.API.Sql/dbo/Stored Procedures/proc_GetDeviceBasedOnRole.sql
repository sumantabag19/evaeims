-- =============================================
-- Author:      Muskan Sethia
-- Create Date: 20th March, 2019
-- Description: EXEC	@return_value = [dbo].[proc_GetDeviceBasedOnRole]
--		@p_Roles = N'SuperAdmin',
--		@p_OrgName = N'eims.eva.com',
--		@p_UserName = N'superAdmin',
--		@p_SearchDevice = N''
-- =============================================
CREATE PROCEDURE [dbo].[proc_GetDeviceBasedOnRole]
(
    -- Add the parameters for the stored procedure here
@p_Roles nvarchar(50),
@p_OrgName nvarchar(50),
@p_UserName nvarchar(50),
@p_SearchDevice nvarchar(50) = NULL 
)
AS
BEGIN

declare @TempDeviceTable as UDTDevice;
-- For Users with MultiOrgAccess
IF((SELECT 1 FROM dbo.[Role] WHERE RoleName = @p_Roles AND MultipleOrgAccess = 1)>0)
BEGIN
-- For Get All Devices
	IF(@p_SearchDevice = '')
	BEGIN
	INSERT INTO @TempDeviceTable SELECT o.OrgName,a.AppName, d.* from device d	
	inner join Organization o ON d.OrgId = o.OrgId
	inner join application a on d.appid= a.appid
    inner join [dbo].[UserOrganizationMapping] uorg ON o.OrgId = uorg.OrgId
	inner join OrganizationApplicationMapping oap ON d.OrgId = oap.OrgId
    WHERE d.IsActive = 1 and uorg.UserId = (SELECT UserId FROM dbo.[User] WHERE UserName = @p_UserName and IsActive = 1 );
	END 
-- For Get Device by Device ID
	ELSE
	INSERT INTO @TempDeviceTable SELECT o.OrgName,a.AppName, d.* from device d		
	inner join Organization o ON d.OrgId = o.OrgId
	inner join application a on d.appid= a.appid
    inner join [dbo].[UserOrganizationMapping] uorg ON o.OrgId = uorg.OrgId
	inner join OrganizationApplicationMapping oap ON d.OrgId = oap.OrgId
    WHERE  d.DeviceId = @p_SearchDevice AND d.IsActive = 1 AND uorg.UserId = (SELECT UserId FROM dbo.[User] WHERE UserName = @p_UserName and IsActive = 1 );
END

ELSE
-- For SuperAdmin
BEGIN
IF  'eims.eva.com' = @p_OrgName AND 'SuperAdmin' = @p_Roles
	BEGIN 
-- For Get All Devices
			IF(@p_SearchDevice = '')
			BEGIN
				INSERT INTO @TempDeviceTable SELECT o.OrgName,a.AppName, d.* from device d
	inner join Organization o on d.OrgId=o.orgid
	inner join application a on d.appid=a.appid
	WHERE d.IsActive = 1;
			END 
-- For Get Specific Device
			ELSE
				INSERT INTO @TempDeviceTable SELECT o.OrgName,a.AppName, d.* from device d
	inner join Organization o on d.OrgId=o.orgid
	inner join application a on d.appid=a.appid
	WHERE d.IsActive = 1 and d.deviceId = @p_SearchDevice;
	END
			
	ELSE 
-- For SiteAdmin
	BEGIN

	IF('SiteAdmin' = @p_Roles)
	BEGIN
	-- For Get All Devices
	IF(@p_SearchDevice = '')
			BEGIN
				INSERT INTO @TempDeviceTable SELECT o.OrgName,a.AppName, d.* from device d
	inner join Organization o on d.OrgId=o.orgid
	inner join application a on d.appid=a.appid
	where d.IsActive = 1 and d.orgId = (SELECT OrgId FROM dbo.[Organization] WHERE OrgName = @p_OrgName and IsActive = 1 );
			END

	ELSE
				INSERT INTO @TempDeviceTable SELECT o.OrgName,a.AppName, d.* from device d
	inner join Organization o on d.OrgId=o.orgid
	inner join application a on d.appid=a.appid
	WHERE d.IsActive = 1 and d.deviceId = @p_SearchDevice and d.orgId = (SELECT OrgId FROM dbo.[Organization] WHERE OrgName = @p_OrgName and IsActive = 1 );

	END

			END
	END
SELECT * FROM @TempDeviceTable;
END;