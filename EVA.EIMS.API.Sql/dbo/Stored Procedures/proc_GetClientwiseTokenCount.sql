CREATE PROCEDURE proc_GetClientwiseTokenCount
AS
BEGIN

SELECT A.ClientId,A.RequestThreshold, TokenCount = Count(1) FROM OauthClient A
INNER JOIN RefreshToken B ON A.ClientId = B.ClientId
WHERE ISNULL(A.RequestThreshold,0) > 0
GROUP BY A.ClientId,A.RequestThreshold
HAVING COUNT(1) > A.RequestThreshold
ORDER BY Count(1) DESC

END
