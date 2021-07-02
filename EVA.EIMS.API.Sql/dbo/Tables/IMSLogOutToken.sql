CREATE TABLE [dbo].[IMSLogOutToken] (
    [LogOutTokenId]         UNIQUEIDENTIFIER NOT NULL,
    [LogOutToken]           NVARCHAR (MAX)   NOT NULL,
    [LogoutOn]              DATETIME         NOT NULL,
    [TokenValidationPeriod] DATETIME         NOT NULL,
    [OTP]                   VARCHAR (200)    NULL,
    CONSTRAINT [PK_IMSLogOutToken] PRIMARY KEY CLUSTERED ([LogOutTokenId] ASC)
);

