CREATE TABLE [dbo].[Language] (
    [LanguageId]   UNIQUEIDENTIFIER NOT NULL,
    [LanguageCode] NVARCHAR (45)    NOT NULL,
    [LanguageName] NVARCHAR (45)    NOT NULL,
    CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED ([LanguageId] ASC)
);

