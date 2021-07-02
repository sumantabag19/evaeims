-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[proc_GetAllApplicationByUserId]
    -- Add the parameters for the stored procedure here
    @p_UserId uniqueidentifier,     
	@p_role nvarchar(50),
	@p_UserName nvarchar(50)
AS
BEGIN
	
	
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
	if (@p_role<>'SuperAdmin') 
	begin 
		if (@p_UserId <> '00000000-0000-0000-0000-000000000000')
		 begin	
			
			if OBJECT_ID('tempDB..#userApplicationDetails') is not null begin drop table #userApplicationDetails end
			select u.userid,u.username,a.appid,a.appname,a.Description into #userApplicationDetails from [user] u
				inner join applicationusermapping aum on u.userid=aum.userid 
				inner join [application] a on a.appid=aum.appid 			
				where u.userid = @p_UserId and u.orgid in (select OrgId from [user] where UserName=@p_UserName)

			select uad1.UserId,uad1.UserName,
			(select STUFF(( SELECT  N',' + convert(varchar ,uad2.AppId)
							FROM  #userApplicationDetails  uad2
							WHERE (uad1.UserId = uad2.UserId)
							order by AppId
							FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')) as AppId,
			(select STUFF(( SELECT  N',' + convert(varchar ,uad2.appname)
							FROM  #userApplicationDetails  uad2
							WHERE (uad1.UserId = uad2.UserId)
							order by AppId
							FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')) as AppName,
			(select STUFF(( SELECT  N',' + convert(varchar ,uad2.Description)
							FROM  #userApplicationDetails  uad2
							WHERE (uad1.UserId = uad2.UserId)
							order by AppId
							FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')) as AppDescription	
	
			from #userApplicationDetails uad1	
			group by uad1.UserId,uad1.username
		end
		else
		begin
			if OBJECT_ID('tempDB..#userApplicationDetails1') is not null begin drop table #userApplicationDetails1 end
		select u.userid,u.username,a.appid,a.appname,a.Description into #userApplicationDetails1 from [user] u
			inner join applicationusermapping aum on u.userid=aum.userid 
			inner join [application] a on a.appid=aum.appid 	
			where u.orgid in (select OrgId from [user] where UserName=@p_UserName)
		select uad1.UserId,uad1.UserName,
		(select STUFF(( SELECT  N',' + convert(varchar ,uad2.AppId)
						FROM  #userApplicationDetails1  uad2
						WHERE (uad1.UserId = uad2.UserId)
						order by AppId
						FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')) as AppId,
		(select STUFF(( SELECT  N',' + convert(varchar ,uad2.appname)
						FROM  #userApplicationDetails1  uad2
						WHERE (uad1.UserId = uad2.UserId)
						order by AppId
						FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')) as AppName,
		(select STUFF(( SELECT  N',' + convert(varchar ,uad2.Description)
						FROM  #userApplicationDetails1  uad2
						WHERE (uad1.UserId = uad2.UserId)
						order by AppId
						FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')) as AppDescription	
	
		from #userApplicationDetails1 uad1	
		group by uad1.UserId,uad1.username
		end
	end
	else
	begin
		if (@p_UserId <> '00000000-0000-0000-0000-000000000000')
		 begin	
			
			if OBJECT_ID('tempDB..#userApplicationDetails2') is not null begin drop table #userApplicationDetails2 end
			select u.userid,u.username,a.appid,a.appname,a.Description into #userApplicationDetails2 from [user] u
				inner join applicationusermapping aum on u.userid=aum.userid 
				inner join [application] a on a.appid=aum.appid 			
				where u.userid = @p_UserId 

			select uad1.UserId,uad1.UserName,
			(select STUFF(( SELECT  N',' + convert(varchar ,uad2.AppId)
							FROM  #userApplicationDetails2  uad2
							WHERE (uad1.UserId = uad2.UserId)
							order by AppId
							FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')) as AppId,
			(select STUFF(( SELECT  N',' + convert(varchar ,uad2.appname)
							FROM  #userApplicationDetails2  uad2
							WHERE (uad1.UserId = uad2.UserId)
							order by AppId
							FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')) as AppName,
			(select STUFF(( SELECT  N',' + convert(varchar ,uad2.Description)
							FROM  #userApplicationDetails2  uad2
							WHERE (uad1.UserId = uad2.UserId)
							order by AppId
							FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')) as AppDescription	
	
			from #userApplicationDetails2 uad1	
			group by uad1.UserId,uad1.username
		end
		else
		begin
			if OBJECT_ID('tempDB..#userApplicationDetails3') is not null begin drop table #userApplicationDetails3 end
		select u.userid,u.username,a.appid,a.appname,a.Description into #userApplicationDetails3 from [user] u
			inner join applicationusermapping aum on u.userid=aum.userid 
			inner join [application] a on a.appid=aum.appid 				
		select uad1.UserId,uad1.UserName,
		(select STUFF(( SELECT  N',' + convert(varchar ,uad2.AppId)
						FROM  #userApplicationDetails3  uad2
						WHERE (uad1.UserId = uad2.UserId)
						order by AppId
						FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')) as AppId,
		(select STUFF(( SELECT  N',' + convert(varchar ,uad2.appname)
						FROM  #userApplicationDetails3  uad2
						WHERE (uad1.UserId = uad2.UserId)
						order by AppId
						FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')) as AppName,
		(select STUFF(( SELECT  N',' + convert(varchar ,uad2.Description)
						FROM  #userApplicationDetails3  uad2
						WHERE (uad1.UserId = uad2.UserId)
						order by AppId
						FOR XML PATH(''),TYPE).value('text()[1]','nvarchar(max)'),1,1,N'')) as AppDescription	
	
		from #userApplicationDetails3 uad1	
		group by uad1.UserId,uad1.username
		end
	end
	
END