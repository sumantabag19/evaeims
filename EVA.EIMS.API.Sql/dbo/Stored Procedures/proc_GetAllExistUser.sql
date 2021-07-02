

CREATE PROCEDURE [dbo].[proc_GetAllExistUser]
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


IF((SELECT 1 FROM dbo.[Role] WHERE RoleName = @p_Roles AND MultipleOrgAccess = 1)>0)
BEGIN				
	SELECT u.* from dbo.[User] u
	 INNER JOIN dbo.UserOrganizationMapping uom ON u.OrgId = uom.OrgId 
	 WHERE uom.UserId = (SELECT UserId FROM dbo.[User] WHERE UserName = @p_UserName );
END ELSE
BEGIN
	IF((SELECT 1 WHERE @p_Roles = 'SiteUser') >0)
	BEGIN
		IF((SELECT distinct 1 FROM [User] WHERE UserName = @p_UserName)>0)
		BEGIN
					
			 SELECT * FROM dbo.[User] WHERE UserName = @p_UserName;
								
		END 
	END
	ELSE BEGIN

		IF  @p_OrgId = 'eims.eva.com' AND @p_Roles = 'SuperAdmin' AND exists (SELECT * FROM #ClientTypeList WHERE ClientType ='SecurityApiClient' ) 
		BEGIN 	
					
			 SELECT * FROM dbo.[User] ;

		END
		ELSE IF  @p_OrgId = 'eims.eva.com' AND @p_Roles = 'Siteadmin' AND exists (SELECT * FROM #ClientTypeList WHERE ClientType ='UiWebClient' ) 
		BEGIN 	
					
			 SELECT A.* FROM dbo.[User] A
			 INNER JOIN UserRoleMapping B ON A.UserId = B.UserId
			 INNER JOIN Role C ON B.RoleId = C.RoleId
			 WHERE C.RoleName = 'Support';

		END
		ELSE BEGIN
			IF( (SELECT distinct 1 FROM dbo.[User] as u
								INNER JOIN Organization o on u.OrgId = o.OrgId AND o.OrgName=@p_OrgId
                                WHERE 'SiteAdmin' = @p_Roles)>0 )
			BEGIN

				SELECT u.*
					FROM dbo.[User] u 
					INNER JOIN dbo.[Organization] o on u.OrgId=o.OrgId AND o.IsActive = 1
					WHERE o.OrgName = @p_OrgId;
                
			END

		END 
	END
END
END;