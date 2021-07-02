

CREATE PROCEDURE [dbo].[proc_GetAllUserByUserId]

@p_UserId uniqueidentifier
AS
BEGIN

DECLARE @UserByOrg AS UDTUserDatails;

			DROP TABLE IF EXISTS #UserData;
			SELECT u.UserId, r.RoleName,ct.ClientTypeName INTO #UserData FROM dbo.[User] u 
									inner join dbo.[UserRoleMapping] urm ON urm.UserId = u.UserId AND urm.IsActive = 1 AND u.UserId=@p_UserId
									inner join dbo.[Role] r ON urm.RoleID = r.RoleId AND r.IsActive = 1
									inner join dbo.[UserClientTypeMapping] ctm ON ctm.UserId = u.UserId 
									inner join dbo.[ClientType] ct ON ctm.ClientTypeId = ct.ClientTypeId AND ct.IsActive = 1;
					--SELECT * FROM #UserData;
					
			 Insert INTO @UserByOrg SELECT u.*,ud.Roles,ud.ClientType FROM (SELECT  UserId,
				Roles=STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.RoleName)
					FROM  #UserData  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),			
				ClientType=STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.ClientTypeName)
					FROM  #UserData  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')
			 		FROM  #UserData f1
				group by f1.UserId) ud
			 inner join dbo.[User] u ON ud.UserId = u.UserId AND u.IsActive = 1;

			 SELECT * FROM @UserByOrg;
	END;