

CREATE PROCEDURE [dbo].[proc_GetAllUsersByRoleForUI]
@p_Roles nvarchar(50),
@p_OrgId nvarchar(50),
@p_ClientType nvarchar(100),
@p_UserName nvarchar(50)
as
begin

DROP TABLE IF EXISTS #ClientTypeList;
SELECT value as 'ClientType'  INTO #ClientTypeList
FROM STRING_SPLIT(@p_ClientType, ',')  
WHERE RTRIM(value) <> ''; 

declare @Users as UDTUserForUI;

IF((SELECT 1 FROM dbo.[Role] WHERE RoleName = @p_Roles AND MultipleOrgAccess = 1)>0)
BEGIN
	DROP TABLE IF EXISTS #InternalUserRole;
	SELECT u.UserId, r.RoleName,ct.ClientTypeName INTO #InternalUserRole FROM dbo.[User] u 
							INNER JOIN dbo.[UserRoleMapping] urm on urm.UserId = u.UserId AND urm.IsActive = 1
							INNER JOIN dbo.[Role] r on urm.RoleID = r.RoleId AND r.IsActive = 1
							INNER JOIN dbo.[UserClientTypeMapping] ctm on ctm.UserId = u.UserId 
							INNER JOIN dbo.[ClientType] ct on ctm.ClientTypeId = ct.ClientTypeId AND ct.IsActive = 1
							WHERE u.IsActive =1;
					
	 INSERT INTO @Users SELECT distinct u.*,ud.Roles,ud.ClientType, o.OrgName FROM (SELECT  UserId,
		Roles=STUFF(( SELECT N',' + CONVERT(Varchar, f2.RoleName)
			FROM  #InternalUserRole  f2
			WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),			
		ClientType=STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.ClientTypeName)
			FROM  #InternalUserRole  f2
			WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')
	 		FROM  #InternalUserRole f1
		group by f1.UserId) ud
	 INNER JOIN dbo.[User] u on ud.UserId = u.UserId
	 INNER JOIN dbo.UserOrganizationMapping uom ON u.OrgId = uom.OrgId 
	 INNER JOIN dbo.[Organization] o on o.OrgId = u.OrgId AND o.IsActive = 1
	 WHERE u.IsActive = 1 AND uom.UserId = (SELECT UserId FROM dbo.[User] WHERE UserName = @p_UserName );
END ELSE
BEGIN
	IF((SELECT 1 WHERE @p_Roles = 'SiteUser') >0)
	BEGIN
		IF((SELECT distinct 1 FROM [User] WHERE UserName = @p_UserName)>0)
		BEGIN
			--SELECT "SiteUser" INTO returnValue;
			DROP TABLE IF EXISTS #SiteUserData;
			SELECT u.UserId, r.RoleName,ct.ClientTypeName INTO #SiteUserData FROM dbo.[User] u 
									INNER JOIN dbo.[UserRoleMapping] urm on urm.UserId = u.UserId AND urm.IsActive = 1
									INNER JOIN dbo.[Role] r on urm.RoleID = r.RoleId AND r.IsActive = 1
									INNER JOIN dbo.[UserClientTypeMapping] ctm on ctm.UserId = u.UserId 
									INNER JOIN dbo.[ClientType] ct on ctm.ClientTypeId = ct.ClientTypeId AND ct.IsActive = 1
									WHERE u.IsActive =1;	
					--SELECT * FROM #SiteUserData;
					
			INSERT INTO @Users SELECT u.*,ud.Roles,ud.ClientType, o.OrgName FROM (SELECT  UserId,
				Roles=STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.RoleName)
					FROM  #SiteUserData  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),			
				ClientType=STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.ClientTypeName)
					FROM  #SiteUserData  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')
			 		FROM  #SiteUserData f1
				group by f1.UserId) ud
			 INNER JOIN dbo.[User] u on ud.UserId = u.UserId AND u.IsActive = 1
			 INNER JOIN dbo.[Organization] o on o.OrgId = u.OrgId AND o.IsActive = 1
			 WHERE u.UserName = @p_UserName;
								
		END 
		--ELSE BEGIN
            --SELECT "UserName Not Exists" INTO returnValue;
			--DROP TABLE IF EXISTS @Users;
            --Insert INTO @Users  SELECT u.*  FROM dbo.[User] as u WHERE 1=2 ;
		--END 
        
	END
	ELSE BEGIN
		IF  @p_OrgId = 'eims.eva.com' AND @p_Roles = 'SuperAdmin' AND exists (SELECT * FROM #ClientTypeList WHERE ClientType ='SecurityApiClient' ) 
		BEGIN 

			DROP TABLE IF EXISTS #SuperAdminData;
			SELECT u.UserId, r.RoleName,ct.ClientTypeName INTO #SuperAdminData FROM dbo.[User] u 
									INNER JOIN dbo.[UserRoleMapping] urm on urm.UserId = u.UserId AND urm.IsActive = 1
									INNER JOIN dbo.[Role] r on urm.RoleID = r.RoleId AND r.IsActive = 1
									INNER JOIN dbo.[UserClientTypeMapping] ctm on ctm.UserId = u.UserId 
									INNER JOIN dbo.[ClientType] ct on ct.ClientTypeId = ctm.ClientTypeId AND ct.IsActive = 1
									WHERE u.IsActive =1;	
					
			SELECT u.*,ud.Roles,ud.ClientType,o.OrgName FROM (SELECT  UserId,
				Roles=STUFF(( SELECT  distinct N',' + CONVERT(Varchar, f2.RoleName)
					FROM  #SuperAdminData  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),			
				ClientType=STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.ClientTypeName)
					FROM  #SuperAdminData  f2
					WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')
			 		FROM  #SuperAdminData f1
				group by f1.UserId) ud
				INNER JOIN dbo.[User] u on ud.UserId = u.UserId AND u.IsActive = 1
				INNER JOIN dbo.[Organization] o on o.OrgId = u.OrgId AND o.IsActive = 1;

		END
		ELSE BEGIN
			IF( (SELECT distinct 1 FROM dbo.[User] as u
								INNER JOIN Organization o on u.OrgId = o.OrgId AND o.OrgName=@p_OrgId
                                WHERE 'SiteAdmin' = @p_Roles)>0 )
			BEGIN
			
				--SELECT "Organization" INTO returnValue;
          		DROP TABLE IF EXISTS #SiteAdminData;
				SELECT u.UserId, r.RoleName,ct.ClientTypeName INTO #SiteAdminData FROM dbo.[User] u 
									INNER JOIN dbo.[UserRoleMapping] urm on urm.UserId = u.UserId AND urm.IsActive = 1
									INNER JOIN dbo.[Role] r on urm.RoleID = r.RoleId AND r.IsActive = 1
									INNER JOIN dbo.[UserClientTypeMapping] ctm on ctm.UserId = u.UserId 
									INNER JOIN dbo.[ClientType] ct on ctm.ClientTypeId = ct.ClientTypeId AND ct.IsActive = 1
									WHERE u.IsActive =1;	

			INSERT INTO @Users SELECT u.*,ud.Roles, ud.ClientType, o.OrgName FROM (SELECT  UserId,
					Roles=STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.RoleName)
						FROM  #SiteAdminData  f2
						WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N''),			
					ClientType=STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.ClientTypeName)
						FROM  #SiteAdminData  f2
						WHERE f1.UserId = f2.UserId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')
			 			FROM  #SiteAdminData f1
					group by f1.UserId) ud
					INNER JOIN dbo.[User] u on ud.UserId = u.UserId AND u.IsActive = 1
					INNER JOIN dbo.[Organization] o on u.OrgId=o.OrgId AND o.IsActive = 1
					WHERE o.OrgName = @p_OrgId;
                
			END

		END 
	END
END
  
SELECT * FROM @Users;
END;