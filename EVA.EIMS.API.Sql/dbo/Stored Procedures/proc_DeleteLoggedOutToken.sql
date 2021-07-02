-- =============================================
-- Author: Muskan Sethia
-- Create Date: 26th March,2019
-- Description: This procedure deletes all tokens generated based on their validation period
-- =============================================
CREATE PROCEDURE [dbo].[proc_DeleteLoggedOutToken]
AS
BEGIN
    
    Delete from [dbo].[IMSLogOutToken] where TokenValidationPeriod <= GetUTCDate()
END