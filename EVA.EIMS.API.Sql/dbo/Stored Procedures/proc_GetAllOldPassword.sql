-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[proc_GetAllOldPassword]
(
  @p_UserId nvarchar(50),
  @p_PwdCount int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets FROM
    -- interfering with SELECT statements.
    SET NOCOUNT ON

		DELETE FROM passwordhistory WHERE lastpasswordchangedate not in (SELECT TOP (@p_PwdCount - 1) lastpasswordchangedate FROM PasswordHistory WHERE UserId = @p_UserId ORDER BY LastPasswordChangeDate DESC) and UserId = @p_UserId;
END;