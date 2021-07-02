CREATE TABLE [dbo].[RefreshToken] (
    [RefreshTokenId]              UNIQUEIDENTIFIER NOT NULL,
    [RefreshAuthenticationTicket] VARBINARY (MAX)  NOT NULL,
    [TokenExpirationDateTime]     DATETIME         NOT NULL,
    [AppId]                       INT              NOT NULL,
    [OrgId]                       INT              NOT NULL,
    [UserId]                      UNIQUEIDENTIFIER NULL,
    [ClientId] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_RefreshToken] PRIMARY KEY CLUSTERED ([RefreshTokenId] ASC)
);

