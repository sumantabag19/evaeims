CREATE TABLE [dbo].[ForgotPasswordFlowManagement] (
    [UserId]                      UNIQUEIDENTIFIER NOT NULL,
    [VerifiedEmail]               BIT              CONSTRAINT [DF_ForgotPasswordFlowManagement_VerifiedEmail] DEFAULT ((0)) NOT NULL,
    [VerifiedEmailOn]             DATETIME         NOT NULL,
    [VerifiedOTP]                 BIT              CONSTRAINT [DF_ForgotPasswordFlowManagement_VerifiedOTP] DEFAULT ((0)) NOT NULL,
    [VerifiedOTPOn]               DATETIME         NULL,
    [VerifiedSecurityQuestions]   BIT              CONSTRAINT [DF_ForgotPasswordFlowManagement_VerifiedSecurityQuestions] DEFAULT ((0)) NOT NULL,
    [VerifiedSecurityQuestionsOn] DATETIME         NULL,
    CONSTRAINT [PK_ForgotPasswordFlowManagement] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [FK_User_ForgotPasswordFlowManagement] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([UserId])
);

