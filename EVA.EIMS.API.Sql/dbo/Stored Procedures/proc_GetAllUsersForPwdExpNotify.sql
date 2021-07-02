CREATE PROCEDURE [dbo].[proc_GetAllUsersForPwdExpNotify]
AS
BEGIN

SELECT u.UserName, u.EmailId, u.[Name], a.AppName,U.PasswordExpiration,-- DATEDIFF(DAY,GETDATE(),U.PasswordExpiration),
EmailSubject,
EmailBody = 
REPLACE(
	REPLACE(
		REPLACE(
			REPLACE(
				REPLACE(
					REPLACE(
						REPLACE(ET.EmailBody,'[$#;FirstName$#;]',U.[Name]),
						'[$#;UserName$#;]',U.UserName),
					'[$#;ExpiryDays$#;]',DATEDIFF(DAY,GETDATE(),U.PasswordExpiration)),
				'[$#;AppName$#;]',CASE WHEN a.AppName IN('IMS') THEN 'EVA' ELSE a.AppName END),
			'[$#;AppUrl$#;]',a.AppUrl),
		'[$#;TenantNote$#;]',CASE WHEN a.AppUrl LIKE '%.com%' AND a.AppName IN('EVA','IMS') THEN '' ELSE '' END),
	'[$#;AppDescription$#;]',a.[Description]) + EmailConfidentialMsg + EmailFooter
FROM [dbo].[Application] a
INNER JOIN [dbo].[ApplicationUserMapping] aum ON aum.AppId = a.AppId
INNER JOIN [dbo].[User] u ON aum.UserId = u.UserId
INNER JOIN [dbo].[AuthProviderMaster] apm ON apm.ProviderId=u.ProviderId
INNER JOIN [dbo].[Organization] O ON O.orgid = u.orgid
INNER JOIN [dbo].[EmailTemplate] ET ON A.AppId = ET.AppId
WHERE u.UserId = aum.UserId
AND aum.IsActive = 1
AND a.IsPwdExpNotify = 1
AND u.IsActive = 1
AND u.IsAccLock = 0
AND apm.providername='IMS'
AND O.IsActive = 1
AND ET.EmailTemplateName = 'PasswordExpiration'
AND CAST(u.PasswordExpiration AS DATE) > Cast(GETDATE() AS DATE)
GROUP BY u.[Name],u.UserName, u.EmailId, u.PasswordExpiration, a.AppName,a.[Description],a.AppUrl,ET.EmailSubject,ET.EmailBody,ET.EmailConfidentialMsg, ET.EmailFooter
HAVING DATEADD(DAY, -Max(a.PwdExpNotifyDays), CAST(u.PasswordExpiration AS DATE)) <= CAST(GETDATE() AS DATE)
ORDER BY u.UserName

END
