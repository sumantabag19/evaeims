-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[proc_GetAllOrganizationByClientId]
(
    -- Add the parameters for the stored procedure here
	@p_clientid nvarchar(50)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    select o.* from Organization as o inner join OrganizationApplicationMapping as oa on o.OrgId = oa.OrgId inner join OauthClient as ou on oa.AppId = ou.AppId  where ou.ClientId = @p_clientid;
END