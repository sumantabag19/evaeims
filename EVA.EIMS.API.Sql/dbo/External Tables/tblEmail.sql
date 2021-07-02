CREATE EXTERNAL TABLE [dbo].[tblEmail] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [EmailFrom] NVARCHAR (100) NULL,
    [EmailSubject] NVARCHAR (100) NULL,
    [EmailBody] NVARCHAR (MAX) NULL,
    [EmailConfidentialMsg] NVARCHAR (MAX) NULL,
    [EmailFooter] NVARCHAR (MAX) NULL,
    [EmailLanguageId] UNIQUEIDENTIFIER NULL
)
    WITH (
    DATA_SOURCE = [RefIMSValUat]
    );

