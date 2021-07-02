-- =============================================
CREATE PROCEDURE [dbo].[proc_GetAllAuthorizedIPByAppId]
(
    -- Add the parameters for the stored procedure here
  @AppId nvarchar(100),
   @OrgId int
)
AS
BEGIN
    DROP TABLE IF EXISTS #AppIds  

SELECT value as AppId into #AppIds
FROM STRING_SPLIT(@AppId, ',')  
WHERE RTRIM(value) <> ''; 

select i.IPStartAddress, i.IPEndAddress from [dbo].[IPTable] i
inner join #AppIds a on i.AppId = a.AppId
where orgId = @OrgId

END