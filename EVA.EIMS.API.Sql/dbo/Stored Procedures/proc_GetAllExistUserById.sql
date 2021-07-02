

CREATE PROCEDURE [dbo].[proc_GetAllExistUserById]

@p_Roles nvarchar(50),
@p_OrgId nvarchar(50),
@p_ClientType nvarchar(100),
@p_UserName nvarchar(50),
@p_UserId uniqueidentifier

as
begin

declare @UserById as UDTUser;
	Insert INTO @UserById EXEC dbo.[proc_GetAllExistUser]
			@p_Roles = @p_Roles,
			@p_OrgId = @p_OrgId,
			@p_ClientType = @p_ClientType,
			@p_UserName = @p_UserName

		if((select top 1 1 from @UserById )>0)
		begin
			select * from @UserById where UserId= @p_UserId;
		end

	END