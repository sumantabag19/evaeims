-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[proc_GetAllExceptionRolesByApplicationName]
(
   @p_appName nvarchar(max)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON	
	select r.RoleId,r.RoleName,r.Description from Role as r where r.RoleId not in (select ro.RoleId from ApplicationRoleMapping as ro inner join Application as ap on ro.AppId = ap.AppId where ap.AppName = @p_appName);
END