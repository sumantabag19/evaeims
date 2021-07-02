-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[proc_ResetLockOfUser]

@p_UserName nvarchar(50),
@p_LockType int
AS
BEGIN
  
    SET NOCOUNT ON
	If exists (Select 1 from [dbo].[User] as u inner join LockAccount as l on u.UserId = l.UserId where u.UserName = @p_UserName and l.LockTypeId = @p_LockType and u.IsActive = 1)
		Begin 
			Delete from LockAccount where UserId = (select UserId from [dbo].[User] u where u.UserName = @p_UserName and u.IsActive = 1) and LockTypeId = @p_LockType;
		end
END