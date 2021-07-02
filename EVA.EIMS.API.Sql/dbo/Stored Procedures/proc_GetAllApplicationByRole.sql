-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[proc_GetAllApplicationByRole]
    -- Add the parameters for the stored procedure here       
	@p_role nvarchar(50),
	@p_UserName nvarchar(50)
AS
BEGIN
	
	
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
	if (@p_role<>'SuperAdmin') 
	begin			
			select distinct a.*  from [application] a
				inner join applicationusermapping aum on a.AppId=aum.appid 
				inner join [user] u  on aum.UserId=u.UserId 			
				where  u.orgid in (select OrgId from [user] where UserName=@p_UserName)
				and a.IsActive=1
	end
	else
	begin
		select *  from [application] where IsActive=1
	end
	
END