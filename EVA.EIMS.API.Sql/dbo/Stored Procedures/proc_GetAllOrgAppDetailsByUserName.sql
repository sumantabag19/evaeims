-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[proc_GetAllOrgAppDetailsByUserName]
(
    -- Add the parameters for the stored procedure here
    @p_UserName nvarchar(50)
)
AS
BEGIN
    DROP TABLE IF EXISTS #SupportUserData;
Select distinct uorg.OrgId, aum.AppId, a.AppName INTO #SupportUserData
from dbo.[User] u 
inner join UserOrganizationMapping uorg on u.UserId = uorg.UserId
inner join ApplicationUserMapping aum  on u.UserId = aum.UserId
inner join OrganizationApplicationMapping oam on uorg.OrgId = oam.OrgId
inner join dbo.[Application]a on aum.AppId = a.AppId
WHERE u.UserName = @p_UserName
select f1.AppId, f1.AppName, OrgId = STUFF(( SELECT distinct N',' + CONVERT(Varchar, f2.OrgId)
					FROM  #SupportUserData  f2
					WHERE f1.AppId = f2.AppId FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')
					from
 #SupportUserData f1
 group by f1.AppId, f1.AppName
 
END