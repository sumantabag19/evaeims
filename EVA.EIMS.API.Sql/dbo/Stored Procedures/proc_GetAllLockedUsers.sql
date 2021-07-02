CREATE PROCEDURE [dbo].[proc_GetAllLockedUsers]
(
@p_Roles nvarchar(50),
@p_OrgId nvarchar(50),
@p_ClientType  NVARCHAR(400),
@p_UserName nvarchar(50)
)
AS
BEGIN
DROP TABLE IF EXISTS #ClientTypes;
SELECT value as 'ClientTypeName'  INTO #ClientTypes
FROM STRING_SPLIT(@p_ClientType, ',')  
WHERE RTRIM(value) <> '';  

DROP TABLE IF EXISTS #Users;

--DROP TABLE IF EXISTS #Users;
Create table #Users
(
	[UserId] [uniqueidentIFier] NOT NULL,
	[UserName] [nvarchar](100) NULL,
	[Name] [nvarchar](100) NULL,
	[LockAccountDate] [datetime] NULL,
	[LockTypeId] [int] null
);

IF((SELECT 1 FROM dbo.[Role] WHERE RoleName = @p_Roles and MultipleOrgAccess = 1)>0)
BEGIN
			DROP TABLE IF EXISTS #InternalUserRole;
			SELECT u.UserId,u.UserName, u.Name,u.LockAccountDate,u.LockTypeId INTO #InternalUserRole FROM dbo.[User] u 
									INNER JOIN dbo.[UserRoleMapping] urm ON urm.UserId = u.UserId and urm.IsActive = 1
									INNER JOIN dbo.[Role] r ON urm.RoleID = r.RoleId and r.IsActive = 1
									INNER JOIN dbo.[UserClientTypeMapping] ctm ON ctm.UserId = u.UserId 
									INNER JOIN dbo.[ClientType] ct ON ctm.ClientTypeId = ct.ClientTypeId and ct.IsActive = 1
									WHERE u.IsActive =1;	
					
			Insert INTO #Users SELECT distinct u.UserId, u.UserName,u.Name,u.LockAccountDate,u.LockTypeID
			 	FROM  #InternalUserRole ud
					INNER JOIN dbo.[User] u ON ud.UserId = u.UserId and u.IsAccLock = 1
					INNER JOIN dbo.UserOrganizationMapping uom ON u.OrgId = uom.OrgId 
				WHERE u.IsActive = 1 and uom.UserId = (SELECT UserId FROM dbo.[User] WHERE UserName = @p_UserName );
				
			SELECT * FROM #Users;

END ELSE
BEGIN

	IF (@p_OrgId = 'eims.eva.com' and exists (SELECT * FROM #ClientTypes WHERE ClientTypeName ='SecurityApiClient' ))
	BEGIN
	    
		DROP TABLE IF EXISTS #SuperAdminData;
		SELECT u.UserId,u.UserName, u.Name,u.LockAccountDate,u.LockTypeId INTO #SuperAdminData FROM dbo.[User] u 
					INNER JOIN dbo.[UserRoleMapping] urm ON urm.UserId = u.UserId and urm.IsActive = 1
					INNER JOIN dbo.[Role] r ON urm.RoleID = r.RoleId and r.IsActive = 1
					INNER JOIN dbo.[UserClientTypeMapping] ctm ON ctm.UserId = u.UserId 
					INNER JOIN dbo.[ClientType] ct ON ctm.ClientTypeId = ct.ClientTypeId and ct.IsActive = 1
					WHERE u.IsActive =1;	
					
		Insert INTO #Users SELECT distinct u.UserId, u.UserName,u.Name,u.LockAccountDate,u.LockTypeID
			 FROM  #SuperAdminData ud
			INNER JOIN dbo.[User] u ON ud.UserId = u.UserId and u.IsActive = 1 and u.IsAccLock = 1;
			SELECT * FROM #Users;
	END
	ELSE BEGIN
		IF( (SELECT distinct 1 FROM dbo.[User] as u
							INNER JOIN Organization o ON u.OrgId = o.OrgId and o.OrgName=@p_OrgId
                               WHERE 'SiteAdmin' = @p_Roles)>0 )
		BEGIN

			drop table IF exists #SiteAdminData
			
			SELECT u.OrgId , a.AppId,ctm.ClientTypeId  INTO #SiteAdminData FROM dbo.[User] u
					INNER JOIN dbo.[ApplicationUserMapping] a ON u.UserId = a.UserId
					INNER JOIN dbo.[UserClientTypeMapping] ctm ON ctm.UserId = u.UserId 
					INNER JOIN dbo.[ClientType] ct ON ctm.ClientTypeId = ct.ClientTypeId and ct.IsActive = 1
				WHERE u.IsActive =1 and u.UserName = @p_UserName;

			insert INTO #Users SELECT distinct u.UserId, u.UserName,u.Name,u.LockAccountDate,u.LockTypeId FROM #SiteAdminData sa
					INNER JOIN dbo.[ApplicationUserMapping] aum ON aum.AppId = sa.AppId 
					INNER JOIN dbo.[User] u ON sa.OrgId=u.OrgId and u.UserId = aum.UserId and u.IsActive = 1 and u.IsAccLock = 1
					INNER JOIN dbo.[UserRoleMapping] urm  ON urm.UserId = u.UserId 
					INNER JOIN [dbo].[Role] r ON r.RoleId = urm.RoleId
				WHERE r.RoleName ='SiteUser'
	
			SELECT * FROM #Users;
                
		END
			
	END
	
END
END