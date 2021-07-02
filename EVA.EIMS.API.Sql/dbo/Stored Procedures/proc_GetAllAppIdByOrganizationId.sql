-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[proc_GetAllAppIdByOrganizationId]
	-- Add the parameters for the stored procedure here
	@p_OrgId int
AS
BEGIN
	select distinct f1.OrgId,  AppId=STUFF(( select distinct N',' + CONVERT(Varchar, f2.AppId)
					FROM  OrganizationApplicationMapping  f2 
					WHERE f1.OrgId = f2.OrgId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'') from organizationapplicationmapping f1  where f1.OrgId = @p_OrgId ;
END