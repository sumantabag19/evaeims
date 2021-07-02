-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[proc_DeleteRefreshTokenById]
(
 @p_RefreshTokenId nvarchar(50)
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	Delete from RefreshToken where RefreshTokenId = @p_RefreshTokenId;
END