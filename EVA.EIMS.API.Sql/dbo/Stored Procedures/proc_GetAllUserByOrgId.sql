

CREATE PROCEDURE [dbo].[proc_GetAllUserByOrgId]

@p_OrgId nvarchar(50)
as
BEGIN

DECLARE @UserByOrg AS UDTUserDatails;

			DROP TABLE IF EXISTS #UserData;
			SELECT u.UserId, r.RoleName,ct.ClientTypeName INTO #UserData FROM dbo.[User] u 
									inner join dbo.[Organization] o on o.OrgId=u.OrgId AND o.OrgName=@p_OrgId AND o.IsActive = 1 AND u.IsActive=1
									inner join dbo.[UserRoleMapping] urm on urm.UserId = u.UserId AND urm.IsActive = 1
									inner join dbo.[Role] r on urm.RoleID = r.RoleId AND r.IsActive = 1
									inner join dbo.[UserClientTypeMapping] ctm on ctm.UserId = u.UserId 
									inner join dbo.[ClientType] ct on ctm.ClientTypeId = ct.ClientTypeId AND ct.IsActive = 1;
					--SELECT * FROM #UserData;
					
			 INSERT INTO @UserByOrg SELECT u.*,ud.Roles,ud.ClientType FROM (SELECT  UserId,
				Roles=STUFF(( SELECT DISTINCT N',' + CONVERT(Varchar, f2.RoleName)
					FROM  #UserData  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),			
				ClientType=STUFF(( SELECT DISTINCT N',' + CONVERT(Varchar, f2.ClientTypeName)
					FROM  #UserData  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')
			 		FROM  #UserData f1
				group by f1.UserId) ud
			 inner join dbo.[User] u on ud.UserId = u.UserId AND u.IsActive = 1;

			 SELECT * FROM @UserByOrg;
	END;