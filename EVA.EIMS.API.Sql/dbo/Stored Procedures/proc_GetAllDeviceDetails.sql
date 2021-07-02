-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[proc_GetAllDeviceDetails]
 @DeviceID varchar(100) = null
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
    -- Insert statements for procedure here
	if (@DeviceID ='')
	begin
    SELECT d.DeviceId,d.SerialKey,d.PrimaryKey,o.OrgId,o.OrgName,a.AppId,a.AppName,d.IsActive,d.IsUsed,d.gatewaydeviceid,d.subject,d.clienttypeid,d.CreatedBy,d.CreatedOn,d.ModifiedBy,d.ModifiedOn from device d
	inner join Organization o on d.OrgId=o.orgid
	inner join application a on d.appid=a.appid
	end
	else
	begin
	 SELECT d.DeviceId,d.SerialKey,d.PrimaryKey,o.OrgId,o.OrgName,a.AppId,a.AppName,d.IsActive,d.IsUsed,d.gatewaydeviceid,d.subject,d.clienttypeid,d.CreatedBy,d.CreatedOn,d.ModifiedBy,d.ModifiedOn from device d
	inner join Organization o on d.OrgId=o.orgid
	inner join application a on d.appid=a.appid
	where d.DeviceId=@DeviceID
	end

	
END