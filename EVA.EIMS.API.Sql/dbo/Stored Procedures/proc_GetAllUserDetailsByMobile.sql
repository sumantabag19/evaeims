CREATE PROCEDURE [dbo].[proc_GetAllUserDetailsByMobile]
@p_Mobile nvarchar(50)
AS
BEGIN

--SELECT "SiteUser" INTO returnValue;
DROP TABLE IF EXISTS #SiteUserData;

SELECT u.UserId,u.AppUserId,u.AppOrgId, r.RoleName,ct.ClientTypeName, ct.ClientTypeId, au.AppId,ap.AppName,aum.ProviderName,org.OrgName, r.MultipleOrgAccess,u.TwoFactorEnabled,
MobileLoginEnabled = ISNULL(U.MobileLoginEnabled,0)
INTO #SiteUserData
FROM dbo.[User] u
INNER JOIN dbo.[UserRoleMapping] urm ON urm.UserId = u.UserId AND urm.IsActive = 1
INNER JOIN dbo.[Role] r ON urm.RoleID = r.RoleId AND r.IsActive = 1
INNER JOIN dbo.[UserClientTypeMapping] ctm ON ctm.UserId = u.UserId
INNER JOIN dbo.[ClientType] ct ON ctm.ClientTypeId = ct.ClientTypeId AND ct.IsActive = 1
LEFT JOIN dbo.[ApplicationUserMapping] au ON au.UserId = u.UserId
LEFT JOIN dbo.[Application] ap ON ap.AppId = au.AppId
INNER JOIN dbo.AuthProviderMaster aum ON u.ProviderId=aum.ProviderId
INNER JOIN dbo.Organization org ON org.OrgId = u.OrgId
WHERE u.IsActive =1
AND org.MobileLoginEnabled = 1
AND u.PhoneNumber = @p_Mobile;

SELECT u.*,ud.Roles,ud.ClientType,ud.ClientTypeId,ud.AppId,ud.AppName,ProviderName,OrgName,MultipleOrgAccess
FROM (SELECT UserId,
		ProviderName=STUFF(( SELECT DISTINCT N',' + CONVERT(VARCHAR, f2.ProviderName)
								FROM #SiteUserData f2
								WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),
		Roles=STUFF(( SELECT DISTINCT N',' + CONVERT(VARCHAR, f2.RoleName)
								FROM #SiteUserData f2
								WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),
		AppName=STUFF(( SELECT DISTINCT N',' + CONVERT(VARCHAR, f2.AppName)
								FROM #SiteUserData f2
								WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),
		ClientType=STUFF(( SELECT DISTINCT N',' + CONVERT(VARCHAR, f2.ClientTypeName)
								FROM #SiteUserData f2
								WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),
		ClientTypeId = STUFF(( SELECT DISTINCT N',' + CONVERT(VARCHAR, f2.ClientTypeId)
								FROM #SiteUserData f2
								WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),
		AppId = STUFF(( SELECT DISTINCT N',' + CONVERT(VARCHAR, f2.AppId)
								FROM #SiteUserData f2
								WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),
		OrgName=STUFF(( SELECT DISTINCT N',' + CONVERT(nvarchar(max), f2.OrgName)
								FROM #SiteUserData f2
								WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),
		MultipleOrgAccess = (SELECT DISTINCT f2.MultipleOrgAccess
								FROM #SiteUserData f2
								WHERE f1.UserId = f2.UserId),
		MobileLoginEnabled = f1.MobileLoginEnabled
		FROM #SiteUserData f1
		GROUP BY f1.UserId,f1.MobileLoginEnabled) ud
INNER JOIN dbo.[User] u ON ud.UserId = u.UserId AND u.IsActive = 1
WHERE u.PhoneNumber = @p_Mobile;

END;
