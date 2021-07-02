-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[proc_GetAllOrgWithAppDetails]

AS
BEGIN
-- SET NOCOUNT ON added to prevent extra result sets from
-- interfering with SELECT statements.
SET NOCOUNT OFF

-- Insert statements for procedure here
    
if OBJECT_ID('tempDB..#OrganizationApplicationDetails') is not null begin drop table #OrganizationApplicationDetails end
select distinct o.OrgId,o.OrgName,o.[Description] as OrgDescription,o.IsActive as OrgIsActive,a.AppId,a.AppName,a.[Description]as AppDescription,a.AppUrl,a.IsActive as AppIsActive,oam.CanAccessAllUsers
into #OrganizationApplicationDetails 
from Organization o 
inner join organizationapplicationmapping oam on o.OrgId=oam.OrgId
inner join [Application] a on a.appid=oam.AppId

select oad1.OrgId,oad1.OrgName,
(select STUFF(( SELECT  N',' + convert(varchar ,oad2.AppId)
					FROM  #OrganizationApplicationDetails  oad2
					WHERE (oad1.Orgid = oad2.Orgid)
					order by AppId
					FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')) as AppId,
 (select STUFF(( SELECT  N',' + convert(varchar ,oad2.appname)
					FROM  #OrganizationApplicationDetails  oad2
					WHERE (oad1.Orgid = oad2.Orgid)
					order by AppId
					FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')) as AppName,
(select STUFF(( SELECT  N',' + convert(varchar ,oad2.AppDescription)
					FROM  #OrganizationApplicationDetails  oad2
					WHERE (oad1.Orgid = oad2.Orgid)
					order by AppId
					FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')) as AppDescription,
(SELECT STUFF(( SELECT N',' + CONVERT(VARCHAR ,oad2.CanAccessAllUsers)
					FROM #OrganizationApplicationDetails oad2
					WHERE (oad1.Orgid = oad2.Orgid)
					ORDER by AppId
					FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')) AS CanAccessAllUser
from #OrganizationApplicationDetails oad1
group by oad1.Orgid,oad1.OrgName 

END