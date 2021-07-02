-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
Create PROCEDURE [dbo].[proc_GetAllUserWithAppDetails]

AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT OFF

    -- Insert statements for procedure here

  
if OBJECT_ID('tempDB..#UserApplicationDetails') is not null begin drop table #UserApplicationDetails end
select distinct u.userid,u.username,a.appid,a.appname into #UserApplicationDetails from [user] u 
inner join ApplicationUserMapping aum on u.UserId=aum.UserId
inner join [Application] a on a.AppId=aum.AppId

select uad1.userId,uad1.userName,
(select STUFF(( SELECT  N',' + convert(varchar ,uad2.AppId)
					FROM  #UserApplicationDetails  uad2
					WHERE (uad1.UserId = uad2.UserId)
					order by AppId
					FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')) as AppId,
 (select STUFF(( SELECT  N',' + convert(varchar ,uad2.appname)
					FROM  #UserApplicationDetails  uad2
					WHERE (uad1.userid = uad2.userid)
					order by AppId
					FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')) as AppName
				
from #UserApplicationDetails uad1
group by uad1.userid,uad1.userName 

End