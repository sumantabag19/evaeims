-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[proc_GetOrganizationByTenant]
@TenantId UNIQUEIDENTIFIER
AS
BEGIN
SET NOCOUNT ON

SELECT OTM.*,O.OrgName
FROM OrganizationTenantMapping OTM
INNER JOIN Organization O ON OTM.OrgId = O.OrgId
WHERE OTM.TenantId = @TenantId

END

