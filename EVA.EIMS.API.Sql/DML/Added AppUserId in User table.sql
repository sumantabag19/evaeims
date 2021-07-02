ALTER TABLE [User]
ADD [AppUserId] BIGINT NULL
GO
ALTER TABLE [User]
ADD [AppOrgId] BIGINT NULL
GO

UPDATE A
SET AppUserId = B.UserId
FROM [User] A
INNER JOIN  [EVA-Catalog-DB].dbo.Users B ON A.UserId = B.IMSUserId
WHERE A.AppUserId IS NULL
GO
UPDATE A
SET AppOrgId = B.OrgId
FROM [User] A
INNER JOIN  [EVA-Catalog-DB].dbo.Users B ON A.UserId = B.IMSUserId
WHERE A.AppOrgId IS NULL