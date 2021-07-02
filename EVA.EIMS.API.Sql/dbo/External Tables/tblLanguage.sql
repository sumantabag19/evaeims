CREATE EXTERNAL TABLE [dbo].[tblLanguage] (
    [LanguageId] UNIQUEIDENTIFIER NOT NULL,
    [LanguageCode] NVARCHAR (100) NULL,
    [LanguageName] NVARCHAR (160) NULL
)
    WITH (
    DATA_SOURCE = [RefIMSValUat]
    );

