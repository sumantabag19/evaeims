-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[proc_ValidateUserApplicationAccess]
(
    -- Add the parameters for the stored procedure here
   @UserId uniqueidentifier, @AppId int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    

    
 Select u.* from [User] u 
INNER JOIN Organization o ON o.OrgId = u.OrgId
INNER JOIN dbo.[OrganizationApplicationMapping] oam ON oam.OrgId = o.OrgId
INNER JOIN [dbo].[Application] a ON a.AppId = oam.AppId
AND  o.IsActive = 1
AND oam.IsActive = 1
and a.IsActive = 1
where u.UserId = @UserId and a.AppId = @AppId
END