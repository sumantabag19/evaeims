-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[proc_GetAllApplicationRoleMapping]
(
	@p_AppRoleId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	if @p_AppRoleId is null
	begin
		select ar.ApplicationRoleId,a.AppName,r.RoleName,ar.IsActive from ApplicationRoleMapping as ar inner join Application as a on ar.AppId = a.AppId inner join Role as r on ar.RoleId = r.RoleId;
	end
	else
	begin
		select ar.ApplicationRoleId,a.AppName,r.RoleName,ar.IsActive from ApplicationRoleMapping as ar inner join Application as a on ar.AppId = a.AppId inner join Role as r on ar.RoleId = r.RoleId 
		where ar.ApplicationRoleId = @p_AppRoleId;
	end


END