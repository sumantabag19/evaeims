CREATE PROCEDURE [dbo].[GetSiteAdminDetails]
	@orgId int
AS
	SELECT U.* FROM [User] U
INNER JOIN UserRoleMapping URM ON U.UserId = URM.UserId
INNER JOIN [Role] R ON URM.RoleId = R.RoleId
WHERE RoleName = 'SiteAdmin'
AND U.IsActive = 1
AND URM.IsActive = 1
AND OrgId = @orgId

RETURN 0
