CREATE TABLE [dbo].[UserAnswer] (
    [AnswerId]       INT              IDENTITY (1, 1) NOT NULL,
    [QuestionId]     INT              NOT NULL,
    [UserId]         UNIQUEIDENTIFIER NOT NULL,
    [UserAnswerText] NVARCHAR (MAX)   NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]      DATETIME         NOT NULL,
    [ModifiedBy]     UNIQUEIDENTIFIER NULL,
    [ModifiedOn]     DATETIME         NULL,
    CONSTRAINT [PK_UserAnswer] PRIMARY KEY CLUSTERED ([AnswerId] ASC),
    CONSTRAINT [FK_UserAnswer_SecurityQuestion ] FOREIGN KEY ([QuestionId]) REFERENCES [dbo].[SecurityQuestion] ([QuestionId]),
    CONSTRAINT [FK_UserAnswer_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([UserId])
);

