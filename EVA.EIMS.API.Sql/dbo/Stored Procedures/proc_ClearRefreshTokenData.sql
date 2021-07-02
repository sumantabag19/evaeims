-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[proc_ClearRefreshTokenData]
as
BEGIN

DELETE FROM dbo.[RefreshToken] where TokenExpirationDateTime <= DATEADD(Hour, -2, GETDATE());
END;