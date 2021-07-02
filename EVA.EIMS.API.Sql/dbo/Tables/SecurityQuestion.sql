CREATE TABLE [dbo].[SecurityQuestion] (
    [QuestionId] INT              IDENTITY (1, 1) NOT NULL,
    [Question]   NVARCHAR (MAX)   NOT NULL,
    [CreatedBy]  UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]  DATETIME         NOT NULL,
    [ModifiedBy] UNIQUEIDENTIFIER NULL,
    [ModifiedOn] DATETIME         NULL,
    [IsActive]   BIT              NOT NULL,
    CONSTRAINT [PK_Question] PRIMARY KEY CLUSTERED ([QuestionId] ASC)
);

