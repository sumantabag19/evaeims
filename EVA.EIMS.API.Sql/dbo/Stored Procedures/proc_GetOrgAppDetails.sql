
CREATE PROCEDURE [dbo].[proc_GetOrgAppDetails]
(
    -- Add the parameters for the stored procedure here
    @TenantId NVARCHAR(100),
	@AzureAppId NVARCHAR(100)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    
SELECT OTM.OrgId,O.OrgName,OTM.TenantId,A.AppId,A.AppName,A.AzureAppId
FROM OrganizationTenantMapping OTM
INNER JOIN OrganizationApplicationMapping OAM ON OTM.OrgId = OAM.OrgId
INNER JOIN Organization O ON OTM.OrgId = O.OrgId
INNER JOIN Application A ON OAM.AppId = A.AppId
WHERE OTM.TenantId = @TenantId
AND A.AzureAppId = @AzureAppId
END
GO
