CREATE TABLE [dbo].[UserOTP] (
    [OTPId]                 INT              IDENTITY (1, 1) NOT NULL,
    [OTPHash]               VARCHAR (50)     NOT NULL,
    [UserId]                UNIQUEIDENTIFIER NOT NULL,
    [OTPCreationDatetime]   DATETIME         NOT NULL,
    [OTPVerificationCount]  INT              NULL,
    [OTPExpirationDatetime] DATETIME         NOT NULL,
    [OTPTypeId]             INT              NULL,
    CONSTRAINT [PK_UserOTP1] PRIMARY KEY CLUSTERED ([OTPId] ASC),
    FOREIGN KEY ([OTPTypeId]) REFERENCES [dbo].[OTPType] ([OTPTypeId]),
    CONSTRAINT [FK_UserOTP_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([UserId])
);

