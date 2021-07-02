-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[proc_GetRandomSecurityQuestion]
(
    -- Add the parameters for the stored procedure here
   @UserId uniqueidentifier,
   @randomQuestionCount int
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
	SET ROWCOUNT @randomQuestionCount
   SELECT q.QuestionId, q.Question FROM [dbo].[SecurityQuestion ] q
INNER JOIN [dbo].[UserAnswer] ua ON ua.QuestionId = q.QuestionId
Where ua.UserId = @UserId
ORDER BY NEWID();
END