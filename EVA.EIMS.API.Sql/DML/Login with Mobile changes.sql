--OTPTypeId = 3
INSERT INTO OTPType
SELECT 'Mobile Login'

GO
ALTER TABLE Organization
ADD [MobileLoginEnabled] BIT DEFAULT(1)
GO
ALTER TABLE [User]
ADD [MobileLoginEnabled] BIT DEFAULT(1)
GO

--proc_GetAllUserDetailsByMobile