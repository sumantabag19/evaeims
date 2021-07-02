-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[proc_GetAllOrgNameAppNameMapping]
(
  @p_OrgAppMappingId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
	if(@p_OrgAppMappingId is null)
	begin
	select oa.OrganizationApplicationId, o.OrgName, a.AppName, oa.IsActive from OrganizationApplicationMapping as oa inner join Application as a on oa.AppId = a.AppId inner join Organization as o on oa.OrgId = o.OrgId
	end
	else
	begin
	select oa.OrganizationApplicationId, o.OrgName, a.AppName, oa.IsActive from OrganizationApplicationMapping as oa inner join Application as a on oa.AppId = a.AppId inner join Organization as o on oa.OrgId = o.OrgId where oa.OrganizationApplicationId = @p_OrgAppMappingId
	end

END