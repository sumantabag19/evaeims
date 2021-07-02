INSERT INTO EmailTemplate
SELECT 'UserRegistration','info@eva.com','User creation request for user [$#;UserName$#;]','<html><head></head><body><div style=font - size:13px; font - family:Calibri; ><p style=line - height:10px; >Hi [$#;FirstName$#;],</p><p style=line - height:10px;>The user [$#;UserName$#;]   with email ID : [$#;EmailId$#;] has submitted a request to create user in EVA application. </ p ><p style = line - height:10px; >Thanks, </ p ><p style = line - height:10px; >EVA Management.</ p ></ div > <p style = line - height:7px; > This is an automated email. Please don’t reply to this email.</ p ></ body ></ html >',
'','','BB6D7810-F539-40FC-ABDA-F461076129CE',2,'22E4DF29-D0E6-4D60-A3EB-13152B8496C4',GETDATE(),'22E4DF29-D0E6-4D60-A3EB-13152B8496C4',GETDATE()
GO


UPDATE EmailTemplate
SET EmailSubject = 'Account Activation',
EmailBody = '<html><head></head><body><div style="font-size:13px;font-family:Calibri;"><p style="line-height:10px;">Hello [$#;FirstName$#;],</p> <br/><p style="line-height:10px;">Please click the following link to activate your account<br/><a href = ''[$#;AppURL$#;]Activation/[$#;UserId$#;]''>Click here to activate your account.</a><br /></p> <br/><p style="line-height:10px;">Thanks,</p><p style="line-height:10px;">EVA Management.</p></div><p style="line-height:7px;">This is an automated email. Please don’t reply to this email.</p></body></html>'
WHERE EmailTemplateName = 'UserRegistration'

GO

UPDATE Application
SET AppUrl = 'http://localhost:4200/'
WHERE AppName = 'EVAUI'
GO