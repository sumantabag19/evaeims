-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[proc_GetUserMappingWithApplicationId]
-- Add the parameters for the stored procedure here
@p_UserId NVARCHAR(100)=null,
@p_ClientId NVARCHAR(100),
@p_ClientSecret NVARCHAR(250),
@p_OrgName NVARCHAR(150)
AS
BEGIN
IF @p_OrgName IS NULL 
    BEGIN
		DECLARE @CanAccessAllUsers BIT = 0, @OrgId INT
		SELECT @OrgId = OrgId FROM [User] WHERE UserId = @p_UserId

		SELECT TOP 1 @CanAccessAllUsers = OAM.CanAccessAllUsers FROM OauthClient OA
		INNER JOIN OrganizationApplicationMapping OAM ON OA.AppId = OAM.AppId
		WHERE OA.ClientId = @p_ClientId AND OA.ClientSecret = @p_ClientSecret AND (OAM.OrgId = @OrgId OR OAM.AppId IN (SELECT AppId FROM Application WHERE AppName IN('CPO')))
		ORDER BY OAM.CanAccessAllUsers DESC
			
		IF @CanAccessAllUsers = 1
		BEGIN 
			SELECT distinct oa.OauthClientId, oa.AppId FROM oauthclient oa
			INNER JOIN organizationapplicationmapping oap ON oa.AppId = oap.AppId
			WHERE oa.ClientId = @p_ClientId AND oa.ClientSecret = @p_ClientSecret;
		END
		ELSE
		BEGIN
			SELECT distinct oa.OauthClientId, oa.AppId FROM oauthclient oa
			INNER JOIN applicationusermapping appuser ON appuser.AppId = oa.AppId
			INNER JOIN [dbo].[user] u ON u.UserId = appuser.UserId
			INNER JOIN organizationapplicationmapping oap ON oap.OrgId = u.OrgId and appuser.AppId = oap.AppId
			WHERE u.UserId = @p_UserId AND oa.ClientId = @p_ClientId AND oa.ClientSecret = @p_ClientSecret;
		END
	END
ELSE
    BEGIN
		SELECT oa.OauthClientId, oa.AppId FROM oauthclient oa
		INNER JOIN organizationapplicationmapping oapp ON oapp.AppId = oa.AppId
        INNER JOIN organization o ON o.OrgId = oapp.OrgId
        WHERE o.OrgName = @p_OrgName AND oa.ClientId = @p_ClientId AND oa.ClientSecret = @p_ClientSecret;
    END
END
