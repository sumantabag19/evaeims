CREATE TABLE [dbo].[EmailTemplate] (
    [EmailTemplateId]      INT              IDENTITY (1, 1) NOT NULL,
	[EmailTemplateName]	   VARCHAR(100)     NULL,
    [EmailFrom]            NVARCHAR (160)   NULL,
    [EmailSubject]         NVARCHAR (250)   NULL,
    [EmailBody]            NVARCHAR (MAX)   NULL,
    [EmailConfidentialMsg] NVARCHAR (MAX)   NULL,
    [EmailFooter]          NVARCHAR (MAX)   NULL,
    [LanguageId]           UNIQUEIDENTIFIER NOT NULL,
    [AppId]                INT              NOT NULL,
    [CreatedBy]            UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]            DATETIME         NOT NULL,
    [ModifiedBy]           UNIQUEIDENTIFIER NULL,
    [ModifiedOn]           DATETIME         NULL,
    CONSTRAINT [PK_EmailTemplate] PRIMARY KEY CLUSTERED ([EmailTemplateId] ASC),
    CONSTRAINT [FK_EmailTemplate_Application] FOREIGN KEY ([AppId]) REFERENCES [dbo].[Application] ([AppId]),
    CONSTRAINT [FK_EmailTemplate_Language] FOREIGN KEY ([LanguageId]) REFERENCES [dbo].[Language] ([LanguageId])
);

