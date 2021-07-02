

CREATE PROCEDURE [dbo].[proc_GetAllOrganization]
@p_Roles nvarchar(50),
@p_OrgId nvarchar(50),
@p_ClientType nvarchar(100),
@p_UserName nvarchar(50),
@p_SearchOrg nvarchar(50) = NULL 

as
BEGIN

DROP TABLE IF EXISTS #ClientTypeList;
SELECT value as 'ClientType'  INTO #ClientTypeList
FROM STRING_SPLIT(@p_ClientType, ',')  
WHERE RTRIM(value) <> ''; 

declare @TempOrganization as UDTOrganization;
IF((SELECT 1 FROM dbo.[Role] WHERE RoleName = @p_Roles AND MultipleOrgAccess = 1)>0)
BEGIN
	IF(@p_SearchOrg IS NULL)
	BEGIN
	DROP TABLE IF EXISTS #TempOrganization;
	INSERT INTO @TempOrganization SELECT o.* FROM dbo.Organization o 
			INNER JOIN dbo.UserOrganizationMapping uom ON o.OrgId = uom.OrgId 
			WHERE  uom.UserId = (SELECT UserId FROM dbo.[User] WHERE UserName = @p_UserName and IsActive = 1 );
	END ELSE
	INSERT INTO @TempOrganization SELECT o.* FROM dbo.Organization o 
			INNER JOIN dbo.UserOrganizationMapping uom ON o.OrgId = uom.OrgId 
			WHERE  o.OrgName = @p_SearchOrg AND uom.UserId = (SELECT UserId FROM dbo.[User] WHERE UserName = @p_UserName and IsActive = 1 );
END ELSE
BEGIN
	--IF((SELECT 1 WHERE @p_Roles = 'SiteUser') >0)
	--BEGIN
      -- SiteUser Section
        
	--END
	--ELSE BEGIN
	IF  'eims.eva.com' = @p_OrgId AND 'SuperAdmin' = @p_Roles AND exists (SELECT * FROM #ClientTypeList WHERE ClientType ='SecurityApiClient' ) 
	BEGIN 
			IF(@p_SearchOrg IS NULL)
			BEGIN
				SELECT * FROM dbo.Organization;
			END ELSE
				INSERT INTO @TempOrganization SELECT * FROM dbo.Organization WHERE OrgName = @p_SearchOrg;
			END
	ELSE BEGIN
		IF( ((SELECT distinct 1 FROM dbo.[User] as u
							INNER JOIN Organization o on u.OrgId = o.OrgId AND o.OrgName=@p_OrgId
                               WHERE 'SiteAdmin' = @p_Roles)>0 ) AND (@p_SearchOrg IS NULL OR @p_OrgId = @p_SearchOrg))
			BEGIN
          		INSERT INTO @TempOrganization SELECT * FROM dbo.Organization WHERE OrgName = @p_OrgId;
			END
	END
END 
SELECT * FROM @TempOrganization;
END;