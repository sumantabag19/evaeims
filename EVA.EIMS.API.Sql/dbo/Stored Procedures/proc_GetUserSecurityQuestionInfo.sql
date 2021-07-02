-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[proc_GetUserSecurityQuestionInfo]
(
    -- Add the parameters for the stored procedure here
    @UserId uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
   Select ua.QuestionId, ua.UserAnswerText, q.Question, ua.UserId FROM [dbo].[UserAnswer] ua
INNER JOIN [dbo].[SecurityQuestion ] q ON q.QuestionId = ua.QuestionId
Where ua.UserId = @UserId
END