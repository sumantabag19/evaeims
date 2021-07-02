CREATE PROCEDURE [dbo].[proc_GetAllUsersByRole]
@p_Roles NVARCHAR(50),
@p_OrgId NVARCHAR(50),
@p_ClientType NVARCHAR(100),
@p_UserName NVARCHAR(50)
AS
BEGIN

DECLARE @Users AS UDTUSERDATAILS;

DROP TABLE IF EXISTS #clienttypelist;
DROP TABLE IF EXISTS #TEMP;

SELECT value AS 'ClientType'
INTO #clienttypelist
FROM String_split(@p_ClientType, ',')
WHERE Rtrim(value) <> '';

SELECT u.userid, r.rolename, ct.clienttypename
INTO #TEMP
FROM dbo.[User] u
INNER JOIN dbo.[UserRoleMapping] urm ON urm.userid = u.userid AND urm.isactive = 1
INNER JOIN dbo.[Role] r ON urm.roleid = r.roleid AND r.isactive = 1
INNER JOIN dbo.[UserClientTypeMapping] ctm ON ctm.userid = u.userid
INNER JOIN dbo.[ClientType] ct ON ctm.clienttypeid = ct.clienttypeid
AND ct.isactive = 1
WHERE u.isactive = 1;

IF((SELECT 1 FROM dbo.[Role] WHERE rolename = @p_Roles AND multipleorgaccess = 1) > 0)
BEGIN
	INSERT INTO @Users
	SELECT DISTINCT u.*, ud.roles, ud.clienttype
	FROM (SELECT userid, roles=Stuff((SELECT N',' + CONVERT(VARCHAR, f2.rolename) FROM #TEMP f2 WHERE f1.userid = f2.userid FOR xml path(''), type).value('text()[1]','nvarchar(max)'),1,1,N''),
				clienttype=Stuff((SELECT DISTINCT N',' + CONVERT(VARCHAR, f2.clienttypename) FROM #TEMP f2 WHERE f1.userid = f2.userid FOR xml path(''), type).value('text()[1]','nvarchar(max)'),1,1,N'')
			FROM #TEMP f1
			GROUP BY f1.userid) ud
	INNER JOIN dbo.[User] u ON ud.userid = u.userid
	INNER JOIN dbo.userorganizationmapping uom ON u.orgid = uom.orgid
	WHERE u.isactive = 1
	AND uom.userid = (SELECT userid FROM dbo.[User] WHERE username = @p_UserName);
END
ELSE
BEGIN
	--IF((SELECT 1 WHERE @p_Roles = 'SiteUser') >0)
	IF(((SELECT 1 WHERE @p_Roles = 'SiteUser') >0) OR (@p_OrgId != 'eims.eva.com' AND @p_Roles = 'SuperAdmin'))
	BEGIN
		IF((SELECT DISTINCT 1 FROM [User] WHERE username = @p_UserName) > 0)
		BEGIN
			--SELECT "SiteUser" INTO returnValue;

			INSERT INTO @Users
			SELECT u.*,ud.roles,ud.clienttype
			FROM (SELECT userid, roles=Stuff((SELECT DISTINCT N',' + CONVERT(VARCHAR, f2.rolename) FROM #TEMP f2 WHERE f1.userid = f2.userid FOR xml path(''), type).value('text()[1]','nvarchar(max)'),1,1,N''),
						clienttype=Stuff((SELECT DISTINCT N',' + CONVERT(VARCHAR, f2.clienttypename) FROM #TEMP f2 WHERE f1.userid = f2.userid FOR xml path(''), type).value('text()[1]','nvarchar(max)'),1,1,N'')
					FROM #TEMP f1
					GROUP BY f1.userid) ud
			INNER JOIN dbo.[User] u ON ud.userid = u.userid AND u.isactive = 1
			WHERE u.username = @p_UserName;
		END
		--ELSE BEGIN
		--SELECT "UserName Not EXISTS" INTO returnValue;
		--DROP TABLE IF EXISTS @Users;
		--Insert INTO @Users SELECT u.* FROM dbo.[User] as u WHERE 1=2 ;
		--END
	END
	ELSE
	BEGIN
		IF @p_OrgId = 'eims.eva.com' AND @p_Roles = 'SuperAdmin' AND EXISTS (SELECT * FROM #clienttypelist WHERE clienttype ='SecurityApiClient' )
		BEGIN
			INSERT INTO @Users
			SELECT u.*, ud.roles, ud.clienttype
			FROM (SELECT userid, roles=Stuff((SELECT DISTINCT N',' + CONVERT(VARCHAR, f2.rolename) FROM #TEMP f2 WHERE f1.userid = f2.userid FOR xml path(''),type).value('text()[1]','nvarchar(max)'),1,1,N''),
						clienttype=Stuff((SELECT DISTINCT N',' + CONVERT(VARCHAR, f2.clienttypename) FROM #TEMP f2 WHERE f1.userid = f2.userid FOR xml path(''),type).value('text()[1]','nvarchar(max)'),1,1,N'')
					FROM #TEMP f1
					GROUP BY f1.userid) ud
			INNER JOIN dbo.[User] u ON ud.userid = u.userid AND u.isactive = 1;
		END
		ELSE
		BEGIN
			IF((SELECT DISTINCT 1 FROM dbo.[User] AS u
				INNER JOIN organization o ON u.orgid = o.orgid
				WHERE 'SiteAdmin' = @p_Roles AND o.orgname = @p_OrgId) > 0)
			BEGIN
				--SELECT "Organization" INTO returnValue;
				INSERT INTO @Users
				SELECT u.*, ud.roles, ud.clienttype
				FROM (SELECT userid, roles = Stuff((SELECT DISTINCT N',' + CONVERT(VARCHAR, f2.rolename) FROM #TEMP f2 WHERE f1.userid = f2.userid FOR xml path(''), type).value('text()[1]','nvarchar(max)'),1,1,N''),
							clienttype = Stuff((SELECT DISTINCT N',' + CONVERT(VARCHAR, f2.clienttypename) FROM #TEMP f2 WHERE f1.userid = f2.userid FOR xml path(''), type).value('text()[1]','nvarchar(max)'),1,1,N'')
						FROM #TEMP f1
						GROUP BY f1.userid) ud
				INNER JOIN dbo.[User] u ON ud.userid = u.userid AND u.isactive = 1
				INNER JOIN dbo.[Organization] o ON u.orgid=o.orgid AND o.isactive = 1
				WHERE o.orgname = @p_OrgId;
			END
		END
	END
END

SELECT * FROM @Users;

END;