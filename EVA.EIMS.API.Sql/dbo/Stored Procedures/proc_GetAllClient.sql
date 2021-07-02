-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[proc_GetAllClient]
@p_OauthClientId int
AS
BEGIN
    SET NOCOUNT ON
	If(@p_OauthClientId is null)
	Begin
    SELECT o.OauthClientId,o.ClientId,o.ClientName,o.Flow,o.AllowedScopes,c.ClientTypeName,a.AppName,o.IsActive,o.DeleteRefreshToken,o.TokenValidationPeriod,o.ClientValidationPeriod,o.ClientExpireOn 
	from OauthClient as o inner join Application as a on o.AppId = a.AppId  inner join ClientType as c on o.ClientTypeId = c.ClientTypeId
	end

	Else
	Begin
	SELECT o.OauthClientId,o.ClientId,o.ClientName,o.Flow,o.AllowedScopes,c.ClientTypeName,a.AppName,o.IsActive,o.DeleteRefreshToken,o.TokenValidationPeriod,o.ClientValidationPeriod,o.ClientExpireOn 
	from OauthClient as o inner join Application as a on o.AppId = a.AppId  inner join ClientType as c on o.ClientTypeId = c.ClientTypeId where o.OauthClientId = @p_OauthClientId
	End
END