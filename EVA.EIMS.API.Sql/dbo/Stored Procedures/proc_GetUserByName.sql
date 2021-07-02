-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[proc_GetUserByName]
    -- Add the parameters for the stored procedure here       
	@p_role nvarchar(50),
	@p_TokenUserName nvarchar(50),
	@p_UserName nvarchar(50)
AS
BEGIN
	
	
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
	if (@p_role<>'SuperAdmin') 
	begin			
	--select distinct u.*  from [user] u					
	--			where  u.orgid in (select OrgId from [user] where UserName=@p_TokenUserName)
	--			and u.UserName = @p_UserName
	--			and u.IsActive=1
			DROP TABLE IF EXISTS #SiteUserData;
			SELECT distinct u.UserId, r.RoleName,ct.ClientTypeName, ct.ClientTypeId, au.AppId,ap.AppName,aum.ProviderName,org.OrgName INTO #SiteUserData 
			from dbo.[User] u 
									left JOIN dbo.[UserRoleMapping] urm ON urm.UserId = u.UserId AND urm.IsActive = 1
									left JOIN dbo.[Role] r ON urm.RoleID = r.RoleId AND r.IsActive = 1
									left JOIN dbo.[UserClientTypeMapping] ctm ON ctm.UserId = u.UserId 
									left JOIN dbo.[ClientType] ct ON ctm.ClientTypeId = ct.ClientTypeId AND ct.IsActive = 1
									left JOIN dbo.[ApplicationUserMapping] au ON au.UserId = u.UserId
									left join dbo.[Application] ap ON ap.AppId = au.AppId
									left join dbo.AuthProviderMaster aum on u.ProviderId=aum.ProviderId
									left join dbo.Organization org on org.OrgId = u.OrgId
									WHERE u.orgid in (select OrgId from [user] where UserName=@p_TokenUserName)
									and u.UserName = @p_UserName
									and u.IsActive=1	
					
			 SELECT u.*,ud.Roles,ud.ClientType,ud.ClientTypeId,ud.AppId,ud.AppName,ProviderName,OrgName  from (SELECT  UserId,
				ProviderName=STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.ProviderName)
					FROM  #SiteUserData  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),	
				Roles=STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.RoleName)
					FROM  #SiteUserData  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),
				AppName=STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.AppName)
					FROM  #SiteUserData  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),			
				ClientType=STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.ClientTypeName)
					FROM  #SiteUserData  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),
				ClientTypeId = STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.ClientTypeId)
					FROM  #SiteUserData  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),
				AppId = STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.AppId)
					FROM  #SiteUserData  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),
				OrgName=STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.OrgName)
					FROM  #SiteUserData  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')
			 		from  #SiteUserData f1
				group by f1.UserId) ud
			 INNER JOIN dbo.[User] u ON ud.UserId = u.UserId AND u.IsActive = 1
			 WHERE u.UserName = @p_UserName;
	end	
	else
		begin
			DROP TABLE IF EXISTS #SiteUserData1;
			SELECT distinct u.UserId, r.RoleName,ct.ClientTypeName, ct.ClientTypeId, au.AppId,ap.AppName,aum.ProviderName,org.OrgName INTO #SiteUserData1 
			from dbo.[User] u 
									left JOIN dbo.[UserRoleMapping] urm ON urm.UserId = u.UserId AND urm.IsActive = 1
									left JOIN dbo.[Role] r ON urm.RoleID = r.RoleId AND r.IsActive = 1
									left JOIN dbo.[UserClientTypeMapping] ctm ON ctm.UserId = u.UserId 
									left JOIN dbo.[ClientType] ct ON ctm.ClientTypeId = ct.ClientTypeId AND ct.IsActive = 1
									left JOIN dbo.[ApplicationUserMapping] au ON au.UserId = u.UserId
									left join dbo.[Application] ap ON ap.AppId = au.AppId
									left join dbo.AuthProviderMaster aum on u.ProviderId=aum.ProviderId
									left join dbo.Organization org on org.OrgId = u.OrgId
									WHERE u.UserName = @p_UserName
									and u.IsActive=1	
					
			 SELECT u.*,ud.Roles,ud.ClientType,ud.ClientTypeId,ud.AppId,ud.AppName,ProviderName,OrgName  from (SELECT  UserId,
				ProviderName=STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.ProviderName)
					FROM  #SiteUserData1  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),	
				Roles=STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.RoleName)
					FROM  #SiteUserData1  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),
				AppName=STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.AppName)
					FROM  #SiteUserData1  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),			
				ClientType=STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.ClientTypeName)
					FROM  #SiteUserData1  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),
				ClientTypeId = STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.ClientTypeId)
					FROM  #SiteUserData1  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),
				AppId = STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.AppId)
					FROM  #SiteUserData1  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),
				OrgName=STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.OrgName)
					FROM  #SiteUserData1  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')
			 		from  #SiteUserData1 f1
				group by f1.UserId) ud
			 INNER JOIN dbo.[User] u ON ud.UserId = u.UserId AND u.IsActive = 1
			 WHERE u.UserName = @p_UserName;
		end

END