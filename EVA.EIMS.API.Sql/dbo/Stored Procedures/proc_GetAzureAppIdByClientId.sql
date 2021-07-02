CREATE PROCEDURE [dbo].[proc_GetAzureAppIdByClientId]
	@ClientId NVARCHAR(100)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    
SELECT ClientId,A.AppId,AppName,AzureAppId,C.RedirectURL,C.DebugURL
FROM OauthClient C
INNER JOIN Application A ON C.AppId = A.AppId
WHERE C.ClientId= @ClientId
END
GO